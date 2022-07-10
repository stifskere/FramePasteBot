global using static FPB.Config;
global using static FPB.handlers.CustomMethods;
global using static FPB.Bot;
using Discord;
using Discord.WebSocket;
using FPB.handlers;
using Newtonsoft.Json;

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

public static class Config
{
    public static dynamic LoadConfig()
    {
        if (!File.Exists("Config.json")) File.Create("Config.json");
        using StreamReader file = new StreamReader("Config.json");
        return JsonConvert.DeserializeObject(file.ReadToEnd())!;
    }

    public static readonly DataBaseHandler DataBase = new DataBaseHandler($"./Databases/{LoadConfig().GuildId.ToString()}.db");
    public static BanManager? BanHandler;
}
