using event_sourcing.Domain.Events;

namespace event_sourcing.Domain.Objects;

public sealed class BankAccount
{
    public Guid Id { get; private set; }
    public decimal Balance { get; private set; }
    public long Version { get; private set; }
    public bool IsClosed { get; private set; }
    public List<Event> UncommitedEvents { get; } = [];

    private BankAccount()
    {
    }

    public static BankAccount OpenAccount(string firstName, string secondName, string emailAddress, string telephoneNumber)
    {
        var account = new BankAccount();
        var accountId = Guid.NewGuid();

        account.Raise(new AccountOpened(
            EventId: Guid.NewGuid(),
            AggregateId: accountId,
            Version: 1,
            Account: accountId,
            FirstName: firstName,
            SecondName: secondName,
            EmailAddress: emailAddress,
            TelephoneNumber: telephoneNumber));

        return account;
    }

    public static BankAccount Replay(List<Event> events)
    {
        var account = new BankAccount();

        foreach (var @event in events)
        {
            account.Apply(@event);
        }

        return account;
    }

    public void Deposit()
    {
        // krok 3: kontrola invariantu (ucet neni zavreny, castka > 0) -> Raise(new MoneyDeposited(...))
        throw new NotImplementedException();
    }

    public void ClearUncommitedEvents()
    {
        UncommitedEvents.Clear();
    }

    private void Raise(Event @event)
    {
        Apply(@event);
        UncommitedEvents.Add(@event);
    }

    private void Apply(Event @event)
    {
        switch (@event)
        {
            case AccountOpened opened:
                Id = opened.AggregateId;
                Balance = 0;
                break;
            case MoneyDeposited deposited:
                Balance += deposited.Amount;
                break;
            case MoneyWithdrawn withdrawn:
                Balance -= withdrawn.Amount;
                break;
            case AccountClosed:
                IsClosed = true;
                break;
        }

        Version = @event.Version;
    }
}
