
using event_sourcing.Domain.Objects;
using event_sourcing.Infrastructure;

namespace event_sourcing.Application;

public class ProcessMoneyDepositRequestHandler(EventStore eventStore)
{
    public void Handle(Guid accountId, decimal amount)
    {
        var events = eventStore.GetEvents(accountId);
        var account = BankAccount.Replay(events);
        account.Deposit();

        eventStore.Append(account.UncommitedEvents);
        account.ClearUncommitedEvents();
    }
}