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

    public void Deposit(decimal amount, string currency)
    {
        if (IsClosed)
        {
            throw new InvalidOperationException("Cannot deposit to a closed account.");
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Deposit amount must be positive.");
        }

        Raise(new MoneyDeposited(
            EventId: Guid.NewGuid(),
            AggregateId: Id,
            Version: Version + 1,
            Account: Id,
            Amount: amount,
            Currency: currency,
            DepositedAt: DateTime.UtcNow));
    }

    public void Withdraw(decimal amount, string currency)
    {
        if (IsClosed)
        {
            throw new InvalidOperationException("Cannot withdraw from a closed account.");
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Withdrawal amount must be positive.");
        }

        if (Balance - amount < 0)
        {
            throw new InvalidOperationException($"Insufficient funds: balance {Balance}, requested {amount}.");
        }

        Raise(new MoneyWithdrawn(
            EventId: Guid.NewGuid(),
            AggregateId: Id,
            Version: Version + 1,
            Account: Id,
            Amount: amount,
            Currency: currency,
            WithdrawnAt: DateTime.UtcNow));
    }

    public void Close()
    {
        if (IsClosed)
        {
            throw new InvalidOperationException("Account is already closed.");
        }

        Raise(new AccountClosed(
            EventId: Guid.NewGuid(),
            AggregateId: Id,
            Version: Version + 1,
            Account: Id,
            ClosedAt: DateTime.UtcNow));
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
