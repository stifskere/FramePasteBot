using System.Data.SQLite;
using Discord;
using Discord.WebSocket;
using static FPB.Bot;

namespace FPB;

public class BanManager
{
    //key = userId & content = Time
    private static Dictionary<ulong, long> TimerDict = new();
    public BanManager()
    {
        SQLiteDataReader bans = DataBase.RunSqliteQueryCommand("SELECT * FROM Cases WHERE Type = 'Ban' AND Time != 0");
        while (bans.Read())
        {
            if(TimerDict.ContainsKey((ulong)bans.GetInt64(1))) continue;
            TimerDict.Add((ulong)bans.GetInt64(1), bans.GetInt64(4));
        }

        Task.Run(BanCounter);
    }

    public void NewBanCounter(IGuildUser user, long time)
    {
        if (!TimerDict.ContainsKey(user.Id))
        {
            TimerDict.Add(user.Id, time);
        }
    }

    private static async void BanCounter()
    {
        while (Client.ConnectionState == ConnectionState.Connected)
        {
            ulong actualTime = NowTime;

            foreach (KeyValuePair<ulong, long> entry in TimerDict)
            {
                if ((ulong)entry.Value <= actualTime)
                {
                    ulong userId = ulong.Parse(entry.Key.ToString());
                    SocketGuild guild = Client.Guilds.First(g => g.Id == (ulong)LoadConfig().GuildId);
                    await guild.DownloadUsersAsync();
                    if (await guild.GetBansAsync().AnyAsync(bans => bans.All(f => f.User.Id != userId)))
                    {
                        DataBase.RunSqliteNonQueryCommand($"DELETE FROM Cases WHERE UserId = {userId}");
                        TimerDict.Remove(userId);
                        return;
                    }
                    await guild.RemoveBanAsync(userId);
                   SQLiteDataReader banData = DataBase.RunSqliteQueryCommand($"SELECT * FROM Cases WHERE UserId = {userId}");
                   EmbedBuilder unBanEmbed = new EmbedBuilder();

                   while (banData.Read())
                   {
                       unBanEmbed = unBanEmbed
                           .WithTitle($"{userId} was unbanned")
                           .WithTitle($"**Reason:** Time expired\n**Ban reason:**{banData.GetString(3)}")
                           .WithColor(GetEmbedColor());
                   }
                   
                   DataBase.RunSqliteNonQueryCommand($"DELETE FROM Cases WHERE UserId = {userId}");
                   TimerDict.Remove(userId);
                   
                   SendLog(embed: unBanEmbed.Build());
                   Console.WriteLine("Ban removed");
                }
            }

            await Task.Delay(1000);
        }
    }
}