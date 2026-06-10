namespace event_sourcing.Domain.Events;

public sealed record AccountOpened(Guid EventId, Guid AggregateId, long Version, Guid Account, string FirstName, string SecondName, string EmailAddress, string TelephoneNumber) : Event(EventId, AggregateId, Version);