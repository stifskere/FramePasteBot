using Discord;
using Discord.Interactions;
using FPB.handlers;

namespace FPB.Commands;

public class Mute : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("mute", "Mutes a specified user"), DefaultMemberPermissions(GuildPermission.MuteMembers)]
    public async Task MuteAsync(IGuildUser user, string time, string reason = "A reason was not defined")
    {
        string mutedTime;
        int muteTime;

        reason = reason.Replace("'", "");
        
        switch (time.ToLower().Last())
        {
            case 'm':
                muteTime = (int)TimeSpan.FromMinutes(int.Parse(time.ToLower().Replace("m", ""))).TotalSeconds;
                mutedTime = $"{TimeSpan.FromSeconds(muteTime).Minutes} minutes";
                break;
            case 'h':
                muteTime = (int)TimeSpan.FromHours(int.Parse(time.ToLower().Replace("h", ""))).TotalSeconds;
                mutedTime = $"{TimeSpan.FromSeconds(muteTime).Hours} hours";
                break;
            case 'd':
                muteTime = (int)TimeSpan.FromDays(int.Parse(time.ToLower().Replace("d", ""))).TotalSeconds;
                mutedTime = $"{TimeSpan.FromSeconds(muteTime).Days} days";
                break;
            case 'w':
                muteTime = (int)TimeSpan.FromDays(int.Parse(time.ToLower().Replace("d", "")) * 7).TotalSeconds;
                mutedTime = $"{TimeSpan.FromSeconds(muteTime).Days * 7} weeks";
                break;
            default:
                await RespondAsync("Invalid timespan, you must use \"M\" minutes, \"H\" hours, \"d\" days, \"W\" weeks, \"none\" undefined time", ephemeral: true);
                return;
        }

        await user.SetTimeOutAsync(TimeSpan.FromSeconds(muteTime));
        
        EmbedBuilder muteEmbed = new EmbedBuilder()
            .WithTitle($"{user.GetTag()} was muted")
            .WithDescription($"**The user was muted for:** `{mutedTime}`\n**With the reason:** `{reason}`\n**Case Id:** `{GetLastCaseId()}`\n**UserID:** `{user.Id}`")
            .WithColor(GetEmbedColor(EmbedColors.EmbedRedColor))
            .WithFooter(text: "Mute")
            .WithCurrentTimestamp();
        
        await SendLog(embed: muteEmbed.Build(), caseLog: true);

        muteEmbed = muteEmbed.WithThumbnailUrl("https://tenor.com/view/turn-down-volume-mute-volume-gif-14268149");
        
        await RespondAsync(embed: muteEmbed.Build());

        DataBase.RunSqliteNonQueryCommand($"INSERT INTO Cases(UserId, ModeratorId, Reason, RemovalTime, Type, PunishmentTime) VALUES({user.Id}, {Context.User.Id}, '{reason}', {(muteTime == 0 ? 0 : NowTime + (ulong)muteTime)}, 'Mute', {NowTime})");
    }
}