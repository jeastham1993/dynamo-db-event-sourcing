using System;
using System.Collections.Generic;
using System.Linq;
using DynamoDb.EventSourcing.Events;
using Newtonsoft.Json;

namespace DynamoDb.EventSourcing.Models
{
	/// <summary>
	/// Base class for an event sourced aggregate.
	/// </summary>
    public abstract class EventAggregateBase
    {
	    /// <summary>
	    /// Gets the default value for an aggregate version.
	    /// </summary>
	    public const long NewAggregateVersion = -1;

	    public int EventCounter = 0;

	    private readonly ICollection<IDomainEvent> _uncommittedEvents = new LinkedList<IDomainEvent>();

	    private List<IDomainEvent> _domainEvents = new List<IDomainEvent>();

	    private long _version = NewAggregateVersion;

	    /// <inheritdoc />
	    [JsonIgnore]
	    public long Version => this._version;

	    /// <summary>
	    /// Gets all domain events relating to this aggregate.
	    /// </summary>
	    [JsonIgnore]
	    public IReadOnlyCollection<IDomainEvent> DomainEvents => this._domainEvents;

	    /// <summary>
	    /// Clear all uncommitted events.
	    /// </summary>
	    public void ClearUncommittedEvents()
	    {
		    this._uncommittedEvents.Clear();
	    }

	    /// <summary>
	    /// Clear all events.
	    /// </summary>
	    public void ClearAllEvents()
	    {
		    this._uncommittedEvents.Clear();
		    this._domainEvents.Clear();
	    }

	    /// <inheritdoc />
	    public void ApplyEvent(
		    IDomainEvent evt,
		    long version)
	    {
		    if (this._uncommittedEvents.Any(p => p.EventId == evt.EventId) != false)
		    {
			    return;
		    }

		    this.EventCounter++;

		    ((dynamic)this).Apply((dynamic)evt);
		    this._version = version;
	    }

	    /// <summary>
	    /// Get all uncommitted events relating to this aggregate.
	    /// </summary>
	    /// <returns>A list of all <see cref="IDomainEvent"/>.</returns>
	    public IEnumerable<IDomainEvent> GetUncommittedEvents()
	    {
		    return this._uncommittedEvents.AsEnumerable();
	    }
    }
}