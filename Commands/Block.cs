using Discord;
using Discord.Interactions;
using static FPB.Bot;

namespace FPB.Commands;

public class Block : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("block", "Blocks a user from sending private messages to the bot."), DefaultMemberPermissions(GuildPermission.MuteMembers)]
    public static async Task BlockAsync(IUser user)
    {
        EmbedBuilder BlockEmbed = new EmbedBuilder()
            .WithTitle($"{user.Username}#{user.Discriminator} has been blocked")
            .WithDescription("The user can't send any more ModMail messages, user /unblock to unblock the user")
            .WithColor(GetEmbedColor("EmbedRedColor"));
    }
}