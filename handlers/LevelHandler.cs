using System.Data.SQLite;
using Discord;

namespace FPB.handlers;

public static class LevelHandler
{
    private static readonly Dictionary<string, ulong> DataDict = new();

    public enum Method { Add, Remove }
    public static async void CheckLevelAndCountMessage(IUser user, Method method = Method.Add)
    {
        bool canLevelUpMessages = false;
        bool canLevelUpDays = false;
        SQLiteDataReader readUserData = DataBase.RunSqliteQueryCommand($"SELECT * FROM Levels WHERE Id = {user.Id}");
        if (!readUserData.HasRows)
        {
            DataBase.RunSqliteNonQueryCommand($"INSERT INTO Levels(Id, Messages, Level) VALUES({user.Id}, 1, 0)");
            return;
        }
        while (readUserData.Read())
        {
            DataDict.Add("Id", (ulong)readUserData.GetInt64(0));
            DataDict.Add("Messages", (ulong)readUserData.GetInt64(1));
            DataDict.Add("Level", (ulong)readUserData.GetInt64(2));
            break;
        }
        if (method == Method.Add) DataDict["Messages"]++;
        else if (method == Method.Remove) DataDict["Messages"]--;
        dynamic nextLevel = LoadConfig().Levels[(DataDict["Level"] + 1).ToString()];
        dynamic nextNextLevel = LoadConfig().Levels[(DataDict["Level"] + 2).ToString()];
        dynamic actualLevel = LoadConfig().Levels[DataDict["Level"].ToString()];
        if ((int) NowTime - ((IGuildUser)user).JoinedAt!.Value.ToUnixTimeSeconds() > TimeSpan.FromDays(int.Parse((string)nextLevel.Time.ToString())).TotalSeconds) canLevelUpDays = true;
        if (DataDict["Messages"] >= ulong.Parse(nextLevel.Messages.ToString())) canLevelUpMessages = true;
        if (canLevelUpDays && canLevelUpMessages)
        {
            DataDict["Level"]++;
            ITextChannel botCommandsChannel = (ITextChannel)Guild.Channels.First(c => c.Id == ulong.Parse(LoadConfig().Channels.BotCommands.ToString()));
            int remainingDays = int.Parse(nextNextLevel.Time.ToString()) - (int)TimeSpan.FromSeconds(NowTime - (ulong)((IGuildUser)user).JoinedAt!.Value.ToUnixTimeSeconds()).TotalDays;
            EmbedBuilder levelUpEmbed = new EmbedBuilder()
                .WithTitle($"Yo {user.GetTag()} you leveled up")
                .WithDescription($"You leveled up from <@&{actualLevel.RoleId.ToString()}> to <@&{nextLevel.RoleId.ToString()}>")
                .AddField("Your progress", $"You need {(nextNextLevel.Messages - DataDict["Messages"] >= 0 ? $"{nextNextLevel.Messages - DataDict["Messages"]} more messages ❌" : "0 more messages ✔️")}\nand you need {(remainingDays >= 0 ? $"{remainingDays} more days ❌" : "0 more days ✔️")}\n\nWhen you complete this progress you will be able to level up.")
                .WithColor(GetEmbedColor())
                .WithFooter(text: "Use /rank to check your or others rank and progress");
            IUserMessage messageSent = await botCommandsChannel.SendMessageAsync(text: $"<@{user.Id}>", embed: levelUpEmbed.Build());
            await messageSent.ModifyAsync(m => m.Content = null);
        }
        DataBase.RunSqliteNonQueryCommand($"UPDATE Levels SET Messages = {DataDict["Messages"]}, Level = {DataDict["Level"]} WHERE Id == {user.Id}");
        DataDict.Clear();
    }
}