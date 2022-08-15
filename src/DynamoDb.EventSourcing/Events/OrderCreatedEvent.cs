namespace DynamoDb.EventSourcing.Events;

public class OrderCreatedEvent : DomainEventBase, IDomainEvent
{
    private OrderCreatedEvent(
        string aggregateId,
        long version,
        string orderIdentifier)
        : base(
            aggregateId,
            version)
    {
        this.OrderIdentifier = orderIdentifier;
    }

    public string AggregateId { get; set; }

    public string OrderIdentifier { get; set; }

    public override string EventName => "OrderCreatedEvent";

    public override string EventSource => "OrderService";

    /// <inheritdoc />
    public override IDomainEvent WithAggregate(
        string aggregateId,
        long aggregateVersion)
    {
        return new OrderCreatedEvent(
            aggregateId,
            aggregateVersion,
            this.OrderIdentifier);
    }
}