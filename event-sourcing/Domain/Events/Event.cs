namespace event_sourcing.Domain.Events;

public abstract record Event(Guid EventId, Guid AggregateId, long Version);