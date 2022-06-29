using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace FPB.Events;

public static class Ready
{
    private static InteractionService? _commands;
    public static async Task Event()
    {
        Console.WriteLine($"Bot started as {Bot.Client.CurrentUser.Username}");
        
        Bot.Client.InteractionCreated += InteractionCreated;
        _commands = new InteractionService(Bot.Client);
        _commands.SlashCommandExecuted += SlashCommandExecuted;
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
        await _commands.RegisterCommandsToGuildAsync(ulong.Parse(LoadConfig().GuildId.ToString()), true);

    }
    
    private static async Task InteractionCreated(SocketInteraction interaction)
    {
        await _commands!.ExecuteCommandAsync(new SocketInteractionContext(Bot.Client, interaction), null);
    }
    
    private static Task SlashCommandExecuted(SlashCommandInfo info, IInteractionContext context, IResult result)
    {
        if(result.Error != null) Console.WriteLine(result.ErrorReason);
        return Task.CompletedTask;
    }
}