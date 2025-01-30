namespace Sockets.Logging;

public static class Logger {
    public static void Log(string user, string message) => Console.WriteLine($"[{DateTime.Now}] {user}: {message}");
    public static void LogError(string user, string message) {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[{DateTime.Now}] {user}: ERROR: {message}");
        Console.ResetColor();
    }
    public static void LogSuccess(string user, string message) {
        Console.ForegroundColor = ConsoleColor.Green;
        Log(user, message);
        Console.ResetColor();
    }
}

public abstract class Program {
    public static int Main() => 0;
}