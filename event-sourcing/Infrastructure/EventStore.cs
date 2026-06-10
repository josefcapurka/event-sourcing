using event_sourcing.Domain.Events;

namespace event_sourcing.Infrastructure;

public class EventStore
{
    private Dictionary<Guid, List<Event>> Events = new();

    // odber pro projekce - v realu message bus / subscription na event store
    public event Action<Event>? EventAppended;

    public List<Event> GetEvents(Guid aggregateId)
    {
        // kopie - interni list nesmi jit zmutovat zvenci ("prepsani historie")
        return Events.TryGetValue(aggregateId, out var list) ? [.. list] : [];
    }

    public List<Event> GetAllEvents()
    {
        return Events.Values.SelectMany(list => list).ToList();
    }

    public void Append(Event @event)
    {
        if (!Events.TryGetValue(@event.AggregateId, out var list))
        {
            list = [];
            Events[@event.AggregateId] = list;
        }

        var expectedVersion = list.Count + 1;
        if (@event.Version != expectedVersion)
            throw new ConcurrencyException(@event.AggregateId, expectedVersion, @event.Version);

        Tracer.Log($"EventStore.Append: {{{@event.GetType().Name}}} v{@event.Version} → stored (total={list.Count + 1})");
        list.Add(@event);
        EventAppended?.Invoke(@event);
    }

    public void Append(List<Event> events)
    {
        foreach (var @event in events)
        {
            Append(@event);
        }
    }
}