global using static FPB.Config;
global using static FPB.handlers.CustomMethods;
global using static FPB.Bot;
using Discord;
using Discord.WebSocket;

namespace FPB;

public static class Bot
{
    public static Task Main()
    {
        return MainAsync(LoadConfig().Token.ToString());
    }

    public static readonly DiscordSocketClient Client = new(new DiscordSocketConfig {GatewayIntents = GatewayIntents.All, MessageCacheSize = 100});

    private static async Task MainAsync(string token)
    {
        Client.Ready += Events.Ready.Event;
        Client.Log += Events.Log.Event;
        Client.MessageReceived += Events.MessageReceived.Event;
        Client.MessageReceived += Commands.GameMessageEvents.Event;
        Client.MessageDeleted += Events.MessageDeleted.Event;
        Client.MessageUpdated += Events.MessageUpdated.Event;

        await Client.LoginAsync(TokenType.Bot, token);
        await Client.StartAsync();
        await Task.Delay(-1);
    }
}