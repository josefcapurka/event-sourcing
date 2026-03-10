using event_sourcing.Domain.Events;

namespace event_sourcing.Domain.Objects;

public sealed class BankAccount
{
    public Guid Id { get; private set; }
    public decimal Balance { get; private set; }
    public List<Event> UncommitedEvents { get; private set; }

    public static BankAccount OpenAccount()
    {
        // zde se zkontroluji business invariant pravidla
        // nakonec pokud pravidla ok -> ulozeni eventu do uncommited events, ale to uz dela app vrstva
        // po ulozeni uncommited events clear uncommited events na entite
        throw new NotImplementedException();
    }

    public static BankAccount Replay(List<Event> events)
    {
        // Zde se prehraji vsechny eventy, ktere kdy v systemu byly pro dany BankAccount
        // naplni se zde properties pro BankAccount
        throw new NotImplementedException();
    }

    public void Deposit()
    {
        throw new NotImplementedException();
    }

    public void ClearUncommitedEvents()
    {
        UncommitedEvents.Clear();
    }
}