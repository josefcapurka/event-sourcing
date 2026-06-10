
using event_sourcing.Domain.Objects;
using event_sourcing.Infrastructure;

namespace event_sourcing.Application;

public class ProcessMoneyDepositRequestHandler(EventStore eventStore)
{
    public void Handle(Guid accountId, decimal amount, string currency)
    {
        var events = eventStore.GetEvents(accountId);
        var account = BankAccount.Replay(events);
        account.Deposit(amount, currency);

        eventStore.Append(account.UncommitedEvents);
        account.ClearUncommitedEvents();
    }
}