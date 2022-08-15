using ProtoBuf;

namespace DynamoDb.EventSourcing.Models
{
    /// <summary>
    /// Encapsulates properties of an event.
    /// </summary>
    [ProtoContract]
    public class StoredEvent
    {
        /// <summary>
        /// Gets or sets the event id.
        /// </summary>
        [ProtoMember(1)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the date the event was raised.
        /// </summary>
        [ProtoMember(2)]
        public DateTime DateRaised { get; set; }

        /// <summary>
        /// Gets or sets the related aggregate id.
        /// </summary>
        [ProtoMember(3)]
        public string AggregateId { get; set; }

        /// <summary>
        /// Gets or sets the event version.
        /// </summary>
        [ProtoMember(4)]
        public long Version { get; set; }

        /// <summary>
        /// Gets or sets the type of event.
        /// </summary>
        [ProtoMember(5)]
        public string EventType { get; set; }

        /// <summary>
        /// Gets or sets the event data.
        /// </summary>
        [ProtoMember(6)]
        public string EventData { get; set; }

        /// <summary>
        /// Gets or sets the organization this event belongs to.
        /// </summary>
        [ProtoMember(7)]
        public string OrganizationName { get; set; }
    }
}