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
            .WithDescription($"<@{user.Id}>\n`{user.Id}`")
            .AddField("🔹 Account creation date",$"<t:{user.CreatedAt.ToUnixTimeMilliseconds() / 1000}:f>\n<t:{user.CreatedAt.ToUnixTimeMilliseconds() / 1000}:R>")
            .AddField("🔹 **Invite data**", "invite: ``")
            .WithColor(GetEmbedColor());

        await SendLog(embed: joinEmbed.Build());
    }
}