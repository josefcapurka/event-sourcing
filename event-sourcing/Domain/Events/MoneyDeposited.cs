namespace event_sourcing.Domain.Events;

public sealed record MoneyDeposited(Guid EventId, Guid AggregateId, long Version, Guid Account, decimal Amount, string Currency, DateTime DepositedAt) : Event(EventId, AggregateId, Version);