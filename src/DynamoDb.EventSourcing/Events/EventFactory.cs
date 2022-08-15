using DynamoDb.EventSourcing.Events;
using DynamoDb.EventSourcing.Models;
using Newtonsoft.Json;

namespace DynamoDb.EventSourcing.Events
{
	internal static class EventFactory
	{
		internal static IDomainEvent CreateFrom(
			StoredEvent evt)
		{
			switch (evt.EventType)
			{
				case "OrderCreatedEvent":
					return JsonConvert.DeserializeObject<OrderCreatedEvent>(evt.EventData);
				default:
					throw new ArgumentException($"Unknown event type {evt.EventType}");
			}
		}
	}
}