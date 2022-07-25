using System.Data.SQLite;
using Discord;
using Newtonsoft.Json;

namespace FPB.handlers;

public static class CustomMethods
{
    public enum HttpMethod
    {
        Get,
        Delete
    }
    public static async Task<dynamic> HttpRequest(string url, Dictionary<string, string>? headers = null, HttpMethod method = HttpMethod.Get)
    {
        using HttpClient client = new HttpClient();
        if(headers != null) foreach (KeyValuePair<string, string> header in headers) client.DefaultRequestHeaders.Add(header.Key, header.Value);
        dynamic retObj = null!;
        switch (method)
        {
            case HttpMethod.Get:
                retObj =  JsonConvert.DeserializeObject(await client.GetStringAsync(url))!;
                break;
            case HttpMethod.Delete:
                try {await client.DeleteAsync(url); retObj = JsonConvert.SerializeObject("{\"status\": \"deleted\"}"); } catch { Console.WriteLine("Error on deleting."); }
                break;
            default:
                throw new Exception("not valid http method.");
        }

        return retObj;
    }

    public static readonly Random Random = new();

    public static ulong NowTime { get; set; } = (ulong) (DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;

    public enum EmbedColors { EmbedGreenColor, EmbedRedColor, EmbedDefaultColor }

    public static uint GetEmbedColor(EmbedColors type = EmbedColors.EmbedDefaultColor)
    {
        string colorType = "";
        switch (type)
        {
            case(EmbedColors.EmbedDefaultColor):
                colorType = "EmbedColor";
                break;
            case(EmbedColors.EmbedGreenColor):
                colorType = "EmbedGreenColor";
                break;
            case(EmbedColors.EmbedRedColor):
                colorType = "EmbedRedColor";
                break;
        }
        
        uint color = 0;
        using SQLiteDataReader embedColorRead = DataBase.RunSqliteQueryCommand($"SELECT * FROM Configuration WHERE key = '{colorType}'");
        while (embedColorRead.Read())
        {
            color = uint.Parse(embedColorRead.GetString(1), System.Globalization.NumberStyles.HexNumber);
            break;
        }

        return color;
    }

    public static async Task SendLog(Embed? embed = null, string? text = null, bool caseLog = false, MessageComponent? components = null)
    {
        ITextChannel logsChannel = (ITextChannel)Client.Guilds.First(g => g.Id == ulong.Parse(LoadConfig().GuildId.ToString())).Channels.First(c => c.Id == ulong.Parse(LoadConfig().Channels.Logs.ToString()));
        ITextChannel caseLogsChannel = (ITextChannel)Client.Guilds.First(g => g.Id == ulong.Parse(LoadConfig().GuildId.ToString())).Channels.First(c => c.Id == ulong.Parse(LoadConfig().Channels.Case.ToString()));
        if (embed == null && text == null && components == null) throw new Exception("Log can't be empty");
        if (caseLog) await caseLogsChannel.SendMessageAsync(embed: embed, text: text);
        else await logsChannel.SendMessageAsync(embed: embed, text: text, components: components);
    }
    
    public static int GetLastCaseId()
    {
        int caseId = 0;
        SQLiteDataReader caseRead = DataBase.RunSqliteQueryCommand("SELECT Id FROM Cases ORDER BY Id DESC");
        while (caseRead.Read())
        {
            caseId = caseRead.GetInt32(0);
            break;
        }
        return caseId;
    }

    public static int StringDistance(string str1, string str2)
    {
        int str1L = str1.Length - 1;
        int str2L = str2.Length - 1;
        return StringDistance(str1, str1L, str2, str2L);
    }

    private static int StringDistance(string str1, int pos1, string str2, int pos2)
    {
        if (Math.Min(pos1, pos2) == 0) return Math.Max(pos1, pos2);
        return Math.Min(Math.Min(StringDistance(str1, pos1 - 1, str2, pos2) + 1, StringDistance(str1, pos1, str2, pos2 - 1)) + 1, StringDistance(str1, pos1 - 1, str2, pos2 - 1) + (str1[pos1] == str2[pos2] ? 0 : 1));
    }

    public static async Task<string> GetBannerUrlAsync(this IUser user, int size = 512)
    {
        // ReSharper disable once RedundantCast
        return $"https://cdn.discordapp.com/banners/{user.Id}/{((dynamic) await HttpRequest(url: $"https://discord.com/api/v8/users/{user.Id}", new Dictionary<string, string> {{"Authorization", $"Bot {LoadConfig().Token.ToString()}"}})).banner}.gif?size={size}";
    }

    public static string GetTag(this IUser user)
    {
        return $"{user.Username}#{user.Discriminator}";
    }

    public static string GetMention(this IChannel channel)
    {
        return $"<#{channel.Id}>";
    }
}