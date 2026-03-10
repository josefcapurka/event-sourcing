namespace event_sourcing.Domain.Events;

public sealed record AccountOpened(Guid EventId, Guid AggregateId, Guid Version, Guid Account, string FirstName, string SecondName, string EmailAddress, string TelephoneNumber) : Event(EventId, AggregateId, Version);