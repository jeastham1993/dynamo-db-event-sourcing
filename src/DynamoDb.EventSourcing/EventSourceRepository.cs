using System.Reflection;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using DynamoDb.EventSourcing.Events;
using DynamoDb.EventSourcing.Extensions;
using DynamoDb.EventSourcing.Models;
using Newtonsoft.Json;

namespace DynamoDb.EventSourcing
{
	/// <inheritdoc />
	public class EventSourceRepository
	{
        private static string TABLE_NAME = Environment.GetEnvironmentVariable("TABLE_NAME");

		private readonly AmazonDynamoDBClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="EventSourceRepository"/> class.
		/// </summary>
		/// <param name="client">A <see cref="AmazonDynamoDBClient"/>.</param>
		public EventSourceRepository(
			AmazonDynamoDBClient client)
		{
			this._client = client;
		}

		/// <inheritdoc />
		public async Task<Order> GetByIdAsync(
			string identifier)
		{
			var baseAggregate = CreateEmptyAggregate();

			foreach (var evt in (await this.GetEventsAsync(identifier).ConfigureAwait(false)).OrderBy(p => p.EventRaised))
			{
				if (evt.GetType() == typeof(Snapshot))
				{
					baseAggregate = (evt as Snapshot).Data;
					continue;
				}

				baseAggregate.ApplyEvent(
					evt,
					evt.Version);
			}

			return baseAggregate;
		}

        /// <inheritdoc />
        public async Task SaveAsync(
			Order aggregate,
            IEnumerable<IDomainEvent> evts)
        {
            var transactWrite = new TransactWriteItemsRequest();

            var counter = 1;

            foreach (var evt in evts)
            {
	            aggregate.ApplyEvent(evt, evt.Version);

	            transactWrite.TransactItems.Add(
                    new TransactWriteItem()
                    {
                        Put = new Put()
                        {
                            Item = new StoredEvent()
                            {
                                AggregateId = evt.AggregateId,
                                DateRaised = evt.EventRaised,
                                EventData = JsonConvert.SerializeObject(evt),
                                EventType = evt.GetType().Name,
                                Id = evt.EventId,
                                Version = evt.Version,
                                OrganizationName = string.Empty
                            }.AsAttributeMap(),
                            TableName = TABLE_NAME,
                            ConditionExpression = "attribute_not_exists(PK)"
                        }
                    });

                counter++;

                if (counter == 10)
                {
	                await this._client.TransactWriteItemsAsync(transactWrite).ConfigureAwait(false);
	                counter = 0;
	                transactWrite.TransactItems = new List<TransactWriteItem>();
                }

                if (aggregate.EventCounter % 9 != 0)
                {
	                continue;
                }

                transactWrite.TransactItems.Add(new TransactWriteItem()
                {
	                Put = new Put()
	                {
		                Item = new StoredEvent()
		                {
			                AggregateId = evt.AggregateId,
			                DateRaised = evt.EventRaised,
			                EventData = JsonConvert.SerializeObject(new Snapshot()
			                {
				                AggregateId = evt.AggregateId,
				                EventRaised = DateTime.Now,
				                Data = aggregate,
				                EventId = Guid.NewGuid(),
				                IsReplay = false,
			                }),
			                EventType = "Snapshot",
			                Id = evt.EventId,
			                Version = evt.Version
		                }.AsAttributeMap(),
		                TableName = TABLE_NAME,
		                ConditionExpression = "attribute_not_exists(PK)"
	                }
                });

                counter++;
            }

            if (transactWrite.TransactItems.Any())
            {
                await this._client.TransactWriteItemsAsync(transactWrite).ConfigureAwait(false);
                counter = 0;
                transactWrite.TransactItems = new List<TransactWriteItem>();
            }
        }

		/// <inheritdoc />
		public async Task<IEnumerable<IDomainEvent>> GetEventsAsync(
			string id)
		{
			var allItems = await this._client.QueryAsync(
				new QueryRequest()
				{
					TableName = TABLE_NAME,
					KeyConditionExpression = "PK = :pk",
					ExpressionAttributeValues =
					{
						{ ":pk", new AttributeValue($"EVENT#{id.ToUpper()}") }
					},
					ConsistentRead = true,
					Limit = 10,
					ScanIndexForward = false
				}).ConfigureAwait(false);

			var allEvents = new List<IDomainEvent>(allItems.Items.Count);

			allItems.Items.Reverse();

			foreach (var item in allItems.Items)
			{
				if (item["Type"].S.Equals(
					"Snapshot",
					StringComparison.OrdinalIgnoreCase))
				{
					allEvents.Clear();
				}

				// Check to see if event has been stored in binary.
				if (item.ContainsKey("StoredAs") &&
					item["StoredAs"].S.Equals(
						"Binary",
						StringComparison.OrdinalIgnoreCase))
				{
					var data = item.FirstOrDefault(p => p.Key == "Data");

					var evt = DynamoDbHelper.CreateFromBinaryItem<StoredEvent>(item);

					allEvents.Add(EventFactory.CreateFrom(evt));
				}
				else
				{
					var evt = DynamoDbHelper.CreateFromItem<StoredEvent>(item);

					allEvents.Add(EventFactory.CreateFrom(evt));
				}
			}

			return allEvents;
		}

		private static Order CreateEmptyAggregate()
		{
			return (Order)typeof(Order).GetConstructor(
					BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
					null,
					new Type[0],
					new ParameterModifier[0])
				?.Invoke(new object[0]);
		}
	}
}