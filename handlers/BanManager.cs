using System.Data.SQLite;
using Discord;
using Discord.Rest;
using ConnectionState = Discord.ConnectionState;

namespace FPB.handlers;

public class BanManager
{
    //key = caseId, value = CaseInfo struct
    private static Dictionary<int, CaseInfo> TimerDict = new();
    public BanManager()
    {
        SQLiteDataReader bans = DataBase.RunSqliteQueryCommand("SELECT * FROM Cases WHERE Type = 'Ban' AND RemovalTime != 0");
        while (bans.Read())
        {
            if(NowTime > (ulong)bans.GetInt64(4)) continue;
            TimerDict.Add(bans.GetInt32(0), new CaseInfo {User = Guild.Users.First(u => u.Id == (ulong)bans.GetInt64(1)), UnbanTime = (ulong)bans.GetInt64(4)});
        }
        Task.Run(BanCounter);
    }

    public void NewBanCounter(IGuildUser user, ulong time, int caseNum)
    {
       TimerDict.Add(caseNum, new CaseInfo {User = user, UnbanTime = time});
    }

    private static async void BanCounter()
    {
        while (Client.ConnectionState == ConnectionState.Connected)
        {
            foreach (var ban in TimerDict.Where(ban => ban.Value.UnbanTime < NowTime))
            {
                IEnumerable<RestBan> bans = await Guild.GetBansAsync().FlattenAsync();
                RestBan? selectBan = bans.FirstOrDefault(b => b.User.Id == ban.Value.User.Id);
                if (selectBan != null) await Guild.RemoveBanAsync(ban.Value.User.Id);
            }
            await Task.Delay(1);
        }
    }

    public async void RemoveBanFromDataBase(int caseNum, IGuildUser author)
    {
        string reason = "";
        SQLiteDataReader banData = DataBase.RunSqliteQueryCommand($"SELECT * FROM Cases WHERE Id == {caseNum}");
        while (banData.Read()) reason = banData.GetString(3);
        DataBase.RunSqliteNonQueryCommand($"UPDATE Cases SET RemovalTime = 0, Reason = '{reason}(unbanned - case dismissed)' WHERE Id = {caseNum}");
        DataBase.RunSqliteNonQueryCommand($"INSERT INTO Cases(UserId, ModeratorId, Reason, RemovalTime, Type, PunishmentTime) VALUES({author.Id}, {Client.CurrentUser.Id}, '')");
    }

    private struct CaseInfo
    {
        public IGuildUser User { get; init; }
        public ulong UnbanTime { get; init; }
    }
}