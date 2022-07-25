using System.Data.SQLite;
using Discord;
using Discord.Interactions;
using FPB.handlers;

namespace FPB.Commands;

[Group("rank", "Contains all the rank related commands")]
public class Rank : InteractionModuleBase<SocketInteractionContext>
{
    private static ulong _userDaysInServerS;
    private static ulong _userMessages;
    private static ulong _userLevel;
    private static int _listRank;
    
    [SlashCommand("view", "View your rank or someone else's rank")]
    public async Task ViewRankAsync(IGuildUser? user = null)
    {
        user ??= (IGuildUser)Context.User;
        string userMention = user.Id == Context.User.Id ? "Your" : $"{user.Username}'s";
        GetRankInfo(user);
        dynamic nextLevel = LoadConfig().Levels[(_userLevel + 1).ToString()];
        int remainingDays = nextLevel.Time - TimeSpan.FromSeconds(_userDaysInServerS).Days;
        int remainingMessages = nextLevel.Messages - _userMessages;
        EmbedBuilder rankEmbed = new EmbedBuilder()
            .WithTitle($"{userMention} rank")
            .AddField($"🔹 {userMention} current stats", $"**Level:** {_userLevel}\n**Messages:** {_userMessages}\n**Days in the server:** {TimeSpan.FromSeconds(_userDaysInServerS).Days}", inline: true)
            .AddField("ㅤ", "ㅤ", inline: true)
            .AddField($"🔹 {userMention} progress to next rank", $"👉 **Next level:** {_userLevel + 1}\n{(remainingMessages <= 0 ? "✅" : "❌")} **Messages until next level:** {(remainingMessages <= 0 ? 0 : remainingMessages)}\n**{(remainingDays <= 0 ? "✅" : "❌")} Days left to level up:** {(remainingDays <= 0 ? 0 : remainingDays)}", inline: true)
            .WithFooter(text: $"You are {_listRank}{(_listRank == 1 ? "st" : "th")} in a descending list of all users, ordered by their level roles interaction count.")
            .WithColor(GetEmbedColor());
        await RespondAsync(embed: rankEmbed.Build());
    }

    [SlashCommand("stats", "View some server rank stats")]
    public async Task ViewRankStatsAsync()
    {
        
    }

    [SlashCommand("leaderboard", "View the server leaderboard")]
    public async Task ViewRankLeaderBoardAsync()
    {
        
    }

    private static void GetRankInfo(IGuildUser user)
    {
        _listRank = 0;
        _userDaysInServerS = NowTime - (ulong) user.JoinedAt!.Value.ToUnixTimeSeconds();
        SQLiteDataReader rankInfo = DataBase.RunSqliteQueryCommand("SELECT * FROM Levels");
        if (!rankInfo.HasRows)
        {
            LevelHandler.CheckLevelAndCountMessage(user);
            GetRankInfo(user);
            return;
        }
        while (rankInfo.Read())
        {
            _listRank++;
            if ((ulong) rankInfo.GetInt64(0) == user.Id)
            {
                _userMessages = (ulong)rankInfo.GetInt64(1);
                _userLevel = (ulong)rankInfo.GetInt64(2);
                break;
            }
        }
        
    }
}