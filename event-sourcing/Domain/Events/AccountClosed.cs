namespace event_sourcing.Domain.Events;

public sealed record AccountClosed(Guid EventId, Guid AggregateId, Guid Version, Guid Account, DateTime ClosedAt) : Event(EventId, AggregateId, Version);