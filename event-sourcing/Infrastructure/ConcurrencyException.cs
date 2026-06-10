namespace event_sourcing.Infrastructure;

public class ConcurrencyException(Guid aggregateId, long expectedVersion, long actualVersion)
    : Exception($"Concurrency conflict on aggregate {aggregateId}: expected version {expectedVersion}, got {actualVersion}.");
