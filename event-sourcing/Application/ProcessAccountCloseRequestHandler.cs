using event_sourcing.Domain.Objects;
using event_sourcing.Infrastructure;

namespace event_sourcing.Application;

public class ProcessAccountCloseRequestHandler(EventStore eventStore)
{
    public void Handle(Guid accountId)
    {
        var events = eventStore.GetEvents(accountId);
        var account = BankAccount.Replay(events);
        account.Close();

        eventStore.Append(account.UncommitedEvents);
        account.ClearUncommitedEvents();
    }
}
