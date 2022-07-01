using System.Data.SQLite;
using Discord;
using Newtonsoft.Json;
using static FPB.Bot;

namespace FPB;

public class CustomMethods
{
    public static async Task<dynamic> HttpRequest(string url, Dictionary<string, string>? headers = null, string method = "GET")
    {
        using HttpClient client = new HttpClient();
        if(headers != null) foreach (KeyValuePair<string, string> header in headers) client.DefaultRequestHeaders.Add(header.Key, header.Value);
        dynamic retObj = null!;
        switch (method)
        {
            case "GET":
                retObj =  JsonConvert.DeserializeObject(await client.GetStringAsync(url))!;
                break;
            case "DELETE":
                try {await client.DeleteAsync(url); retObj = JsonConvert.SerializeObject("{\"status\": \"deleted\"}"); } catch { Console.WriteLine("Error on deleting."); }
                break;
            default:
                throw new Exception("not valid http method.");
        }

        return retObj;
    }

    public static IEnumerable<string> SplitChunks(string str, int maxChunkSize) {
        for (int i = 0; i < str.Length; i += maxChunkSize) yield return str.Substring(i, Math.Min(maxChunkSize, str.Length-i));
    }

    public static Random Random = new Random();

    public int NowTime = (int) (new TimeSpan(DateTime.Now.Ticks) - new TimeSpan(new DateTime(1970, 1, 1).Ticks)).TotalSeconds;

    public static uint GetEmbedColor()
    {
        uint color = 0;
        using SQLiteDataReader embedColorRead = DataBase.RunSqliteQueryCommand("SELECT * FROM Configuration WHERE key = 'EmbedColor'");
        while (embedColorRead.Read())
        {
            color = uint.Parse(embedColorRead.GetString(1), System.Globalization.NumberStyles.HexNumber);
            break;
        }

        return color;
    }

    public static async void SendLog(Embed? embed = null, string? text = null)
    {
        ITextChannel logsChannel = (ITextChannel)Client.Guilds.First(g => g.Id == ulong.Parse(LoadConfig().GuildId.ToString())).Channels.First(c => c.Id == ulong.Parse(LoadConfig().Channels.Logs.ToString()));
        if (embed == null && text == null) throw new Exception("Log can't be empty");
        await logsChannel.SendMessageAsync(embed: embed, text: text);
    }
}