
using event_sourcing.Domain.Objects;
using event_sourcing.Infrastructure;

namespace event_sourcing.Application;

public class ProcessMoneyDepositRequestHandler(EventStore eventStore)
{
    public void Handle(Guid accountId, decimal amount, string currency)
    {
        Tracer.Log($"=== ProcessMoneyDepositRequestHandler ({amount} {currency}) ===");
        Tracer.Log("1. GetEvents → load full event history from EventStore");
        var events = eventStore.GetEvents(accountId);

        Tracer.Log("2. Replay → rebuild current state from events");
        var account = BankAccount.Replay(events);

        Tracer.Log("3. Deposit → validate invariants, Raise MoneyDeposited");
        account.Deposit(amount, currency);

        Tracer.Log($"4. Append {account.UncommitedEvents.Count} uncommitted event(s) to EventStore");
        eventStore.Append(account.UncommitedEvents);
        account.ClearUncommitedEvents();
        Tracer.Log("5. ClearUncommittedEvents → in-memory list flushed");
    }
}