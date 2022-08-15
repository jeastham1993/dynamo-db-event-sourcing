using DynamoDb.EventSourcing.Events;
using Newtonsoft.Json;

namespace DynamoDb.EventSourcing.Models;

public class Order : EventAggregateBase
{
    /// <summary>
    /// Gets or sets the aggregate id.
    /// </summary>
    [JsonProperty]
    public string Id { get; protected set; }

    /// <summary>
    /// Gets or sets the order identifier.
    /// </summary>
    [JsonProperty("identifier")]
    public string Identifier { get; private set; }

    internal void Apply(OrderCreatedEvent evt)
    {
        this.Id = evt.AggregateId;
        this.Identifier = evt.OrderIdentifier;
    }
}