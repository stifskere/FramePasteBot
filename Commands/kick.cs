using Discord;
using Discord.Interactions;
using FPB.handlers;

namespace FPB.Commands;

public class Kick : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("kick", "Kicks a specified user"), DefaultMemberPermissions(GuildPermission.KickMembers)]
    public async Task KickAsync(IGuildUser user, string reason = "A reason was not defined")
    {
        reason = reason.Replace("'", "");
        
        DataBase.RunSqliteNonQueryCommand($"INSERT INTO Cases(UserId, ModeratorId, Reason, RemovalTime, Type, PunishmentTime) VALUES({user.Id}, {Context.User.Id}, '{reason}', 0, 'Kick', {NowTime})");
        
        await user.SendMessageAsync(text: $"You were kicked from {Context.Guild.Name}\n {(reason.Length != 0 ? $"Because of the reason: {reason}" : "With no reason specified")}\nYou can rejoin if you find another invite, read rules to avoid confusion next time.");
        await user.KickAsync(reason: $"Case {GetLastCaseId()} - {reason}");
        
        EmbedBuilder kickEmbed = new EmbedBuilder()
            .WithTitle($"{user.GetTag()} was kicked")
            .WithDescription($"\n**With the reason:** `{reason}`\n**Case Id:** `{GetLastCaseId()}`\n**UserID:** `{user.Id}`")
            .WithColor(GetEmbedColor(EmbedColors.EmbedRedColor))
            .WithFooter(text: "Kick")
            .WithCurrentTimestamp();
        
        await SendLog(embed: kickEmbed.Build(), caseLog: true);

        kickEmbed = kickEmbed.WithThumbnailUrl("https://tenor.com/view/oh-yeah-high-kick-take-down-fight-gif-14272509");
        
        await RespondAsync(embed: kickEmbed.Build());
    }
}