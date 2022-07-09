using Discord;
using Discord.Interactions;
using FPB.handlers;

namespace FPB.Commands;

public class Warn : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("warn", "Warns a user"), DefaultMemberPermissions(GuildPermission.ManageMessages)]
    public async Task WarnAsync(IGuildUser user, string reason = "A reason was not defined")
    {
        reason = reason.Replace("'", "");
        
        DataBase.RunSqliteNonQueryCommand($"INSERT INTO Cases(UserId, ModeratorId, Reason, RemovalTime, Type, PunishmentTime) VALUES({user.Id}, {Context.User.Id}, '{reason}', 0, 'Warn', {NowTime})");

        EmbedBuilder warnEmbed = new EmbedBuilder()
            .WithTitle($"{user.GetTag()} was warned")
            .WithDescription($"\n**With the reason:** `{reason}`\n**Case Id:** `{GetLastCaseId()}`\n**UserID:** `{user.Id}`")
            .WithColor(GetEmbedColor(EmbedColors.EmbedRedColor))
            .WithFooter(text: "Warn")
            .WithCurrentTimestamp();
        
        await SendLog(embed: warnEmbed.Build(), caseLog: true);

        warnEmbed = warnEmbed.WithThumbnailUrl("https://external-preview.redd.it/Dt0PTbhOIidp83QTEfTn00dVUgxmJTDSPJJSLA4JUhg.png?format=pjpg&auto=webp&s=1c0f1bbe0e74840928b059568ed4332e06e54a6f");
        
        await RespondAsync(embed: warnEmbed.Build());
    }
}