using event_sourcing.Domain.Events;

namespace event_sourcing.Infrastructure;

public class EventStore
{
    private Dictionary<Guid, List<Event>> Events = new();

    public List<Event> GetEvents(Guid aggregateId)
    {
        // kopie - interni list nesmi jit zmutovat zvenci ("prepsani historie")
        return Events.TryGetValue(aggregateId, out var list) ? [.. list] : [];
    }

    public void Append(Event @event)
    {
        if (!Events.TryGetValue(@event.AggregateId, out var list))
        {
            list = [];
            Events[@event.AggregateId] = list;
        }

        list.Add(@event);
    }

    public void Append(List<Event> events)
    {
        foreach (var @event in events)
        {
            Append(@event);
        }
    }
}