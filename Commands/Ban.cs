using System.Data.SQLite;
using ColorCat;
using Discord;
using Discord.Interactions;

namespace FPB.Commands;

public class Ban : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ban", "Bans a user from the guild"), DefaultMemberPermissions(GuildPermission.BanMembers)]
    public async Task BanAsync(IGuildUser user, string reason = "A reason was not defined", string time = "")
    {
        string bannedTime = "undefined time";
        int banTime = 0;
        
        if (time != "")
        {
            switch (time.ToLower().Last())
            {
                case 'm':
                    banTime = (int)TimeSpan.FromMinutes(int.Parse(time.ToLower().Replace("m", ""))).TotalSeconds;
                    bannedTime = $"{TimeSpan.FromSeconds(banTime).Minutes} minutes";
                    break;
                case 'h':
                    banTime = (int)TimeSpan.FromHours(int.Parse(time.ToLower().Replace("h", ""))).TotalSeconds;
                    bannedTime = $"{TimeSpan.FromSeconds(banTime).Hours} hours";
                    break;
                case 'd':
                    banTime = (int)TimeSpan.FromDays(int.Parse(time.ToLower().Replace("d", ""))).TotalSeconds;
                    bannedTime = $"{TimeSpan.FromSeconds(banTime).Days} days";
                    break;
                case 'w':
                    banTime = (int)TimeSpan.FromDays(int.Parse(time.ToLower().Replace("d", "")) * 7).TotalSeconds;
                    bannedTime = $"{TimeSpan.FromSeconds(banTime).Days * 7} weeks";
                    break;
                default:
                    await RespondAsync("Invalid timespan, you must use \"M\" minutes, \"H\" hours, \"d\" days, \"W\" weeks, \"none\" undefined time", ephemeral: true);
                    return;
            }
        }
        
        await user.BanAsync(reason: reason);

        EmbedBuilder banEmbed = new EmbedBuilder()
            .WithTitle($"{user.DisplayName}#{user.Discriminator} was banned")
            .WithDescription($"**The user was banned for:** {bannedTime}\n**with the reason:** {reason}\n**Case Id:** {GetLastCaseId()}")
            .WithColor(GetEmbedColor(EmbedColors.EmbedRedColor));
        
        ulong banTimeStamp = NowTime + (ulong)banTime;

        DataBase.RunSqliteNonQueryCommand($"INSERT INTO Cases(UserId, ModeratorId, Reason, Time, Type) VALUES({user.Id}, {Context.User.Id}, '{reason}', {(banTime == 0 ? null : banTimeStamp)}, 'Ban')");
        SendLog(embed: banEmbed.Build(), caseLog: true);
        banHandler.NewBanCounter(user, (long)banTimeStamp);

        banEmbed = banEmbed.WithThumbnailUrl("https://c.tenor.com/tkJk3Ui_OBIAAAAC/ban-hammer.gif");
        
        await RespondAsync(embed: banEmbed.Build());
    }

    private int GetLastCaseId()
    {
        int caseId = 0;
        SQLiteDataReader caseRead = DataBase.RunSqliteQueryCommand("SELECT * FROM sqlite_sequence WHERE name = 'Cases'");
        while (caseRead.Read())
        {
            caseId = caseRead.GetInt32(0);
            break;
        }
        return caseId;
    }
}