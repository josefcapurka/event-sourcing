namespace event_sourcing.Infrastructure;

public static class Tracer
{
    public static bool Verbose { get; set; } = false;

    public static void Log(string message, int indent = 0)
    {
        if (!Verbose) return;
        Console.WriteLine($"{new string(' ', indent * 2)}[TRACE] {message}");
    }
}
