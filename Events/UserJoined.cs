using Discord;
using Discord.Rest;
using Discord.WebSocket;
using FPB.handlers;

namespace FPB.Events;

public static class UserJoined
{
    public static Dictionary<string, int> InviteCounts = new();
    public static Dictionary<ulong, string> InviteCodes = new();
    public static async Task Event(SocketGuildUser user)
    {
        foreach (RestInviteMetadata invite in await user.Guild.GetInvitesAsync())
        {
            if (InviteCounts.ContainsKey(invite.Code) && InviteCounts[invite.Code] == invite.Uses) continue;
            InviteCodes[user.Id] = invite.Id;
            break;
        }

        EmbedBuilder joinEmbed = new EmbedBuilder()
            .WithTitle($"Member joined: {user.GetTag()}")
            .WithDescription($"<@{user.Id}>\n`{user.Id}`")
            .AddField("🔹 Account creation date",$"<t:{user.CreatedAt.ToUnixTimeMilliseconds() / 1000}:f>\n<t:{user.CreatedAt.ToUnixTimeMilliseconds() / 1000}:R>")
            .AddField("🔹 **Invite data**", $"invite: `{InviteCodes[user.Id]}`")
            .WithColor(GetEmbedColor());

        await SendLog(embed: joinEmbed.Build());
    }
}