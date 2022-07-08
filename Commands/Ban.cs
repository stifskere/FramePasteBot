using Discord;
using Discord.Interactions;
using FPB.handlers;

namespace FPB.Commands;

public class Ban : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ban", "Bans a user from the guild"), DefaultMemberPermissions(GuildPermission.BanMembers)]
    public async Task BanAsync(IGuildUser user, string reason = "A reason was not defined", string time = "")
    {
        string bannedTime = "undefined time";
        int banTime = 0;
        
        reason = reason.Replace("'", "");
        
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
        
        await user.BanAsync(reason: $"Case {GetLastCaseId()} - reason: {reason}, time: {time}");

        EmbedBuilder banEmbed = new EmbedBuilder()
            .WithTitle($"{user.GetTag()} was banned")
            .WithDescription($"**The user was banned for:** `{bannedTime}`\n**With the reason:** `{reason}`\n**Case Id:** `{GetLastCaseId()}`\n**UserID:** `{user.Id}`")
            .WithColor(GetEmbedColor(EmbedColors.EmbedRedColor))
            .WithFooter(text: "Ban")
            .WithCurrentTimestamp();
        
        ulong banTimeStamp = NowTime + (ulong)banTime;

        DataBase.RunSqliteNonQueryCommand($"INSERT INTO Cases(UserId, ModeratorId, Reason, RemovalTime, Type, PunishmentTime) VALUES({user.Id}, {Context.User.Id}, '{reason}', {(banTime == 0 ? 0 : banTimeStamp)}, 'Ban', {NowTime})");
        SendLog(embed: banEmbed.Build(), caseLog: true);
        banHandler.NewBanCounter(user, (long)banTimeStamp);

        banEmbed = banEmbed.WithThumbnailUrl("https://c.tenor.com/tkJk3Ui_OBIAAAAC/ban-hammer.gif");
        
        await RespondAsync(embed: banEmbed.Build());
    }
}