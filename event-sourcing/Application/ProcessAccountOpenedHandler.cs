using event_sourcing.Domain.Objects;
using event_sourcing.Infrastructure;

namespace event_sourcing.Application;

public class ProcessAccountOpenedHandler(EventStore eventStore)
{
    public Guid Handle(string firstName, string secondName, string emailAddress, string telephoneNumber)
    {
        Tracer.Log("=== ProcessAccountOpenedHandler ===");
        Tracer.Log("1. OpenAccount → no prior events, creates AccountOpened v1");
        var account = BankAccount.OpenAccount(firstName, secondName, emailAddress, telephoneNumber);

        Tracer.Log($"2. Append {account.UncommitedEvents.Count} uncommitted event(s) to EventStore");
        eventStore.Append(account.UncommitedEvents);
        account.ClearUncommitedEvents();
        Tracer.Log("3. ClearUncommittedEvents → in-memory list flushed");

        return account.Id;
    }
}