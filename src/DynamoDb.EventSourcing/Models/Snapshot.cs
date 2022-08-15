using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace DynamoDb.EventSourcing.Models
{
    public class Snapshot
    {
	    /// <inheritdoc />
	    [ProtoMember(1)]
	    public string AggregateId { get; set; }

	    /// <inheritdoc />
	    [ProtoMember(2)]
	    public long Version { get; set; }

	    /// <inheritdoc />
	    [ProtoMember(3)]
	    public Guid EventId { get; set; }

	    /// <inheritdoc />
	    [ProtoMember(4)]
	    public DateTime EventRaised { get; set; }

	    /// <inheritdoc />
	    [ProtoMember(5)]
	    public bool IsReplay { get; set; }

	    /// <inheritdoc />
	    [ProtoMember(6)]
	    public string TraceId { get; set; }

	    /// <inheritdoc />
	    [ProtoMember(7)]
	    public string EventName => "snapshot";
		
	    [ProtoMember(8)]
		public Order Data { get; set; }
    }
}
