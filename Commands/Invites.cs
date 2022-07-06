using Discord;
using Discord.Interactions;
using Discord.Rest;

namespace FPB.Commands;

public class Invites : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("invites", "Check how many invites a user has")]
    public async Task InvitesAsync(IUser user)
    {
        int num = Context.Guild.GetInvitesAsync().Result.Where(invite => invite.Inviter.Id == user.Id).Sum(invite => invite.Uses!.Value);

        EmbedBuilder invitesEmbed = new EmbedBuilder()
            .WithTitle("Invites")
            .WithDescription($"<@{user.Id}> has {num} invites.")
            .WithColor(GetEmbedColor());

        await RespondAsync(embed: invitesEmbed.Build());
    }
}