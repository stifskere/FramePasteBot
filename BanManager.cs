using System.Data.SQLite;
using Discord;
using static FPB.Bot;

namespace FPB;

public class BanManager
{
    //key = userId & content = Time
    private static Dictionary<long, long> TimerDict = new();
    public BanManager()
    {
        SQLiteDataReader bans = DataBase.RunSqliteQueryCommand("SELECT * FROM Cases WHERE Type = 'Ban' AND Time != 0");
        while (bans.Read())
        {
            if(TimerDict.ContainsKey(bans.GetInt64(1))) continue;
            TimerDict.Add(bans.GetInt64(1), bans.GetInt64(4));
        }

        Task.Run(BanCounter);
    }

    public void NewBanCounter(IGuildUser user, int time)
    {
        if (!TimerDict.ContainsKey((long) user.Id))
        {
            TimerDict.Add((int)user.Id, time);
        }
    }

    private static async void BanCounter()
    {
        while (Client.ConnectionState == ConnectionState.Connected)
        {
            int actualTime = NowTime;

            foreach (KeyValuePair<long, long> entry in TimerDict)
            {
                if (entry.Value <= actualTime)
                {
                   IGuildUser user = Client.Guilds.First(g => g.Id == ulong.Parse(LoadConfig().GuildId)).Users.First(u => u.Id == ulong.Parse(entry.Key.ToString()));
                   await user.Guild.RemoveBanAsync(user);
                   SQLiteDataReader banData = DataBase.RunSqliteQueryCommand($"SELECT * FROM Cases WHERE Time = {actualTime}");
                   EmbedBuilder unBanEmbed = new EmbedBuilder();

                   while (banData.Read())
                   {
                       unBanEmbed = unBanEmbed
                           .WithTitle($"{user.Nickname}#{user.Discriminator} was unbanned")
                           .WithTitle($"**Reason:** Time expired\n**Ban reason:**{banData.GetString(3)}")
                           .WithColor(GetEmbedColor());
                   }
                   
                   SendLog(embed: unBanEmbed.Build());
                   Console.WriteLine("Ban removed");
                }
            }

            await Task.Delay(1000);
        }
    }
}