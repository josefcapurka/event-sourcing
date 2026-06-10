using event_sourcing.Domain.Events;
using event_sourcing.Infrastructure;

namespace event_sourcing.Application;

public sealed record AccountReadModel(string Owner, decimal Balance, bool IsClosed);

public class AccountBalanceProjection
{
    private readonly Dictionary<Guid, AccountReadModel> _accounts = new();

    public AccountBalanceProjection(EventStore eventStore)
    {
        eventStore.EventAppended += Project;
    }

    public AccountReadModel? Get(Guid accountId)
    {
        // cteni bez replay a bez dotyku event store - predpocitany pohled
        return _accounts.TryGetValue(accountId, out var account) ? account : null;
    }

    public void Rebuild(EventStore eventStore)
    {
        // read model je zahoditelna cache - kdykoliv jde znovu postavit z event logu
        _accounts.Clear();

        foreach (var @event in eventStore.GetAllEvents())
        {
            Project(@event);
        }
    }

    private void Project(Event @event)
    {
        switch (@event)
        {
            case AccountOpened opened:
                _accounts[opened.AggregateId] = new AccountReadModel($"{opened.FirstName} {opened.SecondName}", 0, false);
                break;
            case MoneyDeposited deposited:
                _accounts[deposited.AggregateId] = _accounts[deposited.AggregateId] with { Balance = _accounts[deposited.AggregateId].Balance + deposited.Amount };
                break;
            case MoneyWithdrawn withdrawn:
                _accounts[withdrawn.AggregateId] = _accounts[withdrawn.AggregateId] with { Balance = _accounts[withdrawn.AggregateId].Balance - withdrawn.Amount };
                break;
            case AccountClosed closed:
                _accounts[closed.AggregateId] = _accounts[closed.AggregateId] with { IsClosed = true };
                break;
        }
    }
}
