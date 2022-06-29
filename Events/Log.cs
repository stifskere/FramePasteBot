using Discord;

namespace FPB.Events;

public static class Log
{
    public static Task Event(LogMessage log)
    {
        Console.WriteLine(log.ToString());
        return Task.CompletedTask;
    }
}