global using static FPB.Config;
global using static FPB.CustomMethods;

using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace FPB;

public static class Bot
{
    public static Task Main()
    {
        return MainAsync(LoadConfig().Token.ToString());
    }

    public static readonly DiscordSocketClient Client = new(new DiscordSocketConfig {GatewayIntents = GatewayIntents.All});

    private static async Task MainAsync(string token)
    {
        Client.Ready += Events.Ready.Event;
        Client.Log += Events.Log.Event;
        
        await Client.LoginAsync(TokenType.Bot, token);
        await Client.StartAsync();
        await Task.Delay(-1);
    }
}

public static class CustomMethods
{
    public static async Task<dynamic> HttpRequest(string url, Dictionary<string, string>? headers = null)
    {
        using HttpClient client = new HttpClient();
        foreach (KeyValuePair<string, string> header in headers) client.DefaultRequestHeaders.Add(header.Key, header.Value);
        return JsonConvert.DeserializeObject(await client.GetStringAsync(url))!;

    }
    
    public static IEnumerable<string> SplitChunks(string str, int maxChunkSize) {
        for (int i = 0; i < str.Length; i += maxChunkSize) yield return str.Substring(i, Math.Min(maxChunkSize, str.Length-i));
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
}
