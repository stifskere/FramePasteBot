using Discord;
using Discord.Rest;
using Discord.WebSocket;
using FPB.handlers;

namespace FPB.Events;

public static class UserJoined
{
    public static async Task Event(SocketGuildUser user)
    {
        EmbedBuilder joinEmbed = new EmbedBuilder()
            .WithTitle($"Member joined: {user.GetTag()}")
            .WithDescription($"<@{user.Id}>\n`{user.Id}`\n\n🔹 **Account creation date**\n<t:{user.CreatedAt.ToUnixTimeMilliseconds() / 1000}:f>\n<t:{user.CreatedAt.ToUnixTimeMilliseconds() / 1000}:R>\n\n🔹 **Invite data**\ninvite: `{((IGuildUser)user).}`");
    }
}