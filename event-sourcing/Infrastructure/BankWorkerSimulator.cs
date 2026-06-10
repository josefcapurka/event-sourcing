using event_sourcing.Application;
using event_sourcing.Domain.Events;
using event_sourcing.Domain.Objects;

namespace event_sourcing.Infrastructure;

public class BankWorkerSimulator
{
    public Task RunAsync()
    {
        var eventStore = new EventStore();
        var projection = new AccountBalanceProjection(eventStore); // odebira eventy od ted
        var openAccountHandler = new ProcessAccountOpenedHandler(eventStore);
        var depositHandler = new ProcessMoneyDepositRequestHandler(eventStore);
        var withdrawHandler = new ProcessMoneyWithdrawRequestHandler(eventStore);
        var closeHandler = new ProcessAccountCloseRequestHandler(eventStore);

        // scenar pro otevreni uctu
        var accountId = openAccountHandler.Handle("Josef", "Capurka", "capurkajos@gmail.com", "+420 777 123 456");
        Console.WriteLine($"Ucet otevren: {accountId}");
        PrintState(eventStore, accountId);

        // scenar pro vlozeni penez
        depositHandler.Handle(accountId, 1000m, "CZK");
        depositHandler.Handle(accountId, 250m, "CZK");
        PrintState(eventStore, accountId);

        // scenar pro vyber penez
        withdrawHandler.Handle(accountId, 300m, "CZK");
        PrintState(eventStore, accountId);

        // pokus o precerpani - invariant musi vyber odmitnout
        try
        {
            withdrawHandler.Handle(accountId, 5000m, "CZK");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Vyber odmitnut: {ex.Message}");
            Console.WriteLine();
        }

        // scenar pro uzavreni uctu
        closeHandler.Handle(accountId);
        PrintState(eventStore, accountId);

        // pokus o vklad na zavreny ucet - invariant musi vklad odmitnout
        try
        {
            depositHandler.Handle(accountId, 100m, "CZK");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Vklad odmitnut: {ex.Message}");
            Console.WriteLine();
        }

        // cteni z read modelu - zadny replay, jen lookup
        Console.WriteLine("--- read model (projekce) ---");
        Console.WriteLine($"  {projection.Get(accountId)}");

        // rebuild - smaz a znovu postav z event logu, musi vyjit stejne
        projection.Rebuild(eventStore);
        Console.WriteLine("--- read model po rebuildu z event logu ---");
        Console.WriteLine($"  {projection.Get(accountId)}");

        return Task.CompletedTask;
    }

    private static void PrintState(EventStore eventStore, Guid accountId)
    {
        var events = eventStore.GetEvents(accountId);

        Console.WriteLine("--- event log ---");
        foreach (var @event in events)
        {
            var detail = @event switch
            {
                AccountOpened opened => $"{opened.FirstName} {opened.SecondName}",
                MoneyDeposited deposited => $"+{deposited.Amount} {deposited.Currency}",
                MoneyWithdrawn withdrawn => $"-{withdrawn.Amount} {withdrawn.Currency}",
                AccountClosed closed => $"zavreno {closed.ClosedAt:u}",
                _ => string.Empty
            };

            Console.WriteLine($"  v{@event.Version} {@event.GetType().Name} {detail}");
        }

        var account = BankAccount.Replay(events);
        Console.WriteLine($"  => Balance: {account.Balance}, Version: {account.Version}, IsClosed: {account.IsClosed}");
        Console.WriteLine();
    }
}
