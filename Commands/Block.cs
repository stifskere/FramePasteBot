using Discord;
using Discord.Interactions;

namespace FPB.Commands;

public class Block : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("block", "Blocks a user from sending private messages to the bot."), DefaultMemberPermissions(GuildPermission.MuteMembers)]
    public async Task BlockAsync(IUser user)
    {
        EmbedBuilder blockEmbed = new EmbedBuilder();

        try
        {
            blockEmbed = blockEmbed
                .WithTitle($"{user.Username}#{user.Discriminator} has been blocked")
                .WithDescription("The user can't send any more ModMail messages, user /unblock to unblock the user")
                .WithColor(GetEmbedColor(EmbedColors.EmbedRedColor));

            DataBase.RunSqliteNonQueryCommand($"INSERT INTO BlockedUsers(UserId) VALUES({user.Id})");

            await RespondAsync(embed: blockEmbed.Build());
        }
        catch
        {
            blockEmbed = blockEmbed
                .WithTitle("The user Was not blocked")
                .WithDescription("This user is already in the list or it does not exist.")
                .WithColor(GetEmbedColor(EmbedColors.EmbedRedColor));

            await RespondAsync(embed: blockEmbed.Build());
        }
    }

    [SlashCommand("unblock", "Unblocks a user that was blocked from sending DMs"), DefaultMemberPermissions(GuildPermission.MuteMembers)]
    public async Task UnBlockAsync(IUser user)
    {
        EmbedBuilder unblockedEmbed = new EmbedBuilder();

        try
        {
            unblockedEmbed = unblockedEmbed
                .WithTitle($"{user.Username}#{user.Discriminator} was unblocked")
                .WithDescription("The user can now send DM messages to the bot and use the ModMail service.")
                .WithColor(GetEmbedColor(EmbedColors.EmbedGreenColor));
            
            DataBase.RunSqliteNonQueryCommand($"DELETE FROM BlockedUsers WHERE UserId = {user.Id}");

            await RespondAsync(embed: unblockedEmbed.Build());
        }
        catch
        {
            unblockedEmbed = unblockedEmbed
                .WithTitle($"User can't be unblocked")
                .WithDescription("The user was not in the list or it does not exist.")
                .WithColor(GetEmbedColor(EmbedColors.EmbedGreenColor));

            await RespondAsync(embed: unblockedEmbed.Build());
        }
    }
}