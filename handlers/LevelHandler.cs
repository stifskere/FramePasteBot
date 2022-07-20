using System.Data.SQLite;
using Discord;

namespace FPB.handlers;

public class LevelHandler
{
    private static Dictionary<string, ulong> dataDict = new();
    public void CheckLevelAndCountMessage(IGuildUser? user)
    {
        bool canLevelUpMessages = false;
        bool canLevelUpDays = false;
        SQLiteDataReader readUserData = DataBase.RunSqliteQueryCommand($"SELECT * FROM Levels WHERE Id = {user!.Id}");
        if (!readUserData.HasRows)
        {
            DataBase.RunSqliteNonQueryCommand($"INSERT INTO Levels(Id, Messages, Level) VALUES({user.Id}, 1, 0)");
            return;
        }
        while (readUserData.Read())
        {
            dataDict.Add("Id", (ulong)readUserData.GetInt64(0));
            dataDict.Add("Messages", (ulong)readUserData.GetInt64(1));
            dataDict.Add("Level", (ulong)readUserData.GetInt64(2));
            break;
        }
        dataDict["Messages"]++;
        dynamic nextLevel = LoadConfig().Levels[(dataDict["Level"] + 1).ToString()];
        if ((int) NowTime - user.JoinedAt!.Value.Millisecond > TimeSpan.FromDays(10).Milliseconds) canLevelUpDays = true;
        if (dataDict["Messages"] > ulong.Parse(nextLevel.Messages.ToString())) canLevelUpMessages = true;
        if (canLevelUpDays && canLevelUpMessages) dataDict["Level"]++;
        DataBase.RunSqliteNonQueryCommand($"UPDATE Levels SET Messages = {dataDict["Messages"]}, Level = {dataDict["Level"]} WHERE Id == {user.Id}");
        dataDict.Clear();
    }

    public Embed CheckLevelEmbed(IGuildUser user, bool isExecutingUser)
    {

        return new EmbedBuilder().Build();
    }
}