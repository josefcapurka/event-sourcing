using event_sourcing.Domain.Objects;
using event_sourcing.Infrastructure;

namespace event_sourcing.Application;

public class ProcessAccountOpenedHandler(EventStore eventStore)
{
    public Guid Handle(string firstName, string secondName, string emailAddress, string telephoneNumber)
    {
        var account = BankAccount.OpenAccount(firstName, secondName, emailAddress, telephoneNumber);

        eventStore.Append(account.UncommitedEvents);
        account.ClearUncommitedEvents();

        return account.Id;
    }
}