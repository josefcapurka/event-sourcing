using event_sourcing.Domain.Events;

namespace event_sourcing.Infrastructure;

public class EventStore
{
    private Dictionary<Guid, List<Event>> Events = new();

    public List<Event> GetEvents(Guid aggregateId)
    {
        return Events.TryGetValue(aggregateId, out var list) ? list : [];
    }

    public void Append(Event @event)
    {
        Events[@event.AggregateId].Add(@event);
    }

    public void Append(List<Event> events)
    {
        foreach (var @event in events)
        {
            Append(@event);
        }
    }
}