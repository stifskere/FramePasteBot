using Discord;
using Discord.WebSocket;

namespace FPB;

public static class Bot
{
    public static Task Main()
    {
        return MainAsync("");
    }

    public static readonly DiscordSocketClient Client = new(new DiscordSocketConfig {GatewayIntents = GatewayIntents.All});

    private static async Task MainAsync(string token)
    {
        //events
        
        await Client.LoginAsync(TokenType.Bot, token);
        await Client.StartAsync();
        await Task.Delay(-1);
    }
}
