using System;

namespace DynamoDb.EventSourcing.Events
{
	/// <summary>
	/// Base class for a domain event.
	/// </summary>
	public abstract class DomainEventBase : IDomainEvent
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DomainEventBase"/> class.
		/// </summary>
		protected DomainEventBase()
		{
			this.EventRaised = DateTime.Now;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DomainEventBase"/> class.
		/// </summary>
		/// <param name="aggregateId">The aggregate id the domain event relates to.</param>
		protected DomainEventBase(
			string aggregateId)
		{
			this.EventId = Guid.NewGuid();
			this.AggregateId = aggregateId;
			this.EventRaised = DateTime.Now;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DomainEventBase"/> class.
		/// </summary>
		/// <param name="aggregateId">The aggregate id the domain event relates to.</param>
		/// <param name="version">The version of the event.</param>
		protected DomainEventBase(
			string aggregateId,
			long version)
			: this(aggregateId)
		{
			this.Version = version;
		}

		/// <inheritdoc />
		public string AggregateId { get; set; }

		/// <inheritdoc />
		public long Version { get; set; }

		/// <inheritdoc />
		public Guid EventId { get; set; }

		/// <inheritdoc />
		public DateTime EventRaised { get; set; }

		/// <inheritdoc />
		public bool IsReplay { get; set; }

		/// <summary>
		/// Gets or sets the event name.
		/// </summary>
		public string TraceId { get; set; }

		/// <inheritdoc />
		public abstract string EventName { get; }

		/// <inheritdoc />
		public abstract string EventSource { get; }

		/// <summary>
		/// Generate a domain event using an aggregate.
		/// </summary>
		/// <param name="aggregateId">The id of the aggregate.</param>
		/// <param name="aggregateVersion">The version of the aggregate.</param>
		/// <returns>A new domain event.</returns>
		public abstract IDomainEvent WithAggregate(
			string aggregateId,
			long aggregateVersion);
	}
}