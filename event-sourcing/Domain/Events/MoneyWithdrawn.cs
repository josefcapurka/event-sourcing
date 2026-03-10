namespace event_sourcing.Domain.Events;

public sealed record MoneyWithdrawn(Guid EventId, Guid AggregateId, Guid Version, Guid Account, decimal Amount, string Currency, DateTime WithdrawnAt) : Event(EventId, AggregateId, Version);