using System;

namespace DynamoDb.EventSourcing.Events
{
	/// <summary>
	/// Interface for an application domain event.
	/// </summary>
	public interface IDomainEvent
	{
		/// <summary>
		/// Gets or sets the aggregate id the event relates to.
		/// </summary>
		string AggregateId { get; set; }

		/// <summary>
		/// Gets or sets the event version.
		/// </summary>
		long Version { get; set; }

		/// <summary>
		/// Gets or sets the id of the event.
		/// </summary>
		Guid EventId { get; set; }

		/// <summary>
		/// Gets or sets the date the event was raised.
		/// </summary>
		DateTime EventRaised { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the event is replayed.
		/// </summary>
		public bool IsReplay { get; set; }

		/// <summary>
		/// Gets the event name.
		/// </summary>
		public string EventName { get; }
	}
}