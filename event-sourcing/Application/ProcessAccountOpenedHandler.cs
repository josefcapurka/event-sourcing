using event_sourcing.Domain.Objects;
using event_sourcing.Infrastructure;

namespace event_sourcing.Application;

public class ProcessAccountOpenedHandler(EventStore eventStore)
{
    public void Handle(Guid accountId, decimal amount)
    {
        var account = BankAccount.OpenAccount();

        eventStore.Append(account.UncommitedEvents);
        account.ClearUncommitedEvents();
    }
}