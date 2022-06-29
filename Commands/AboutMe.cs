using Discord;
using Discord.Interactions;

namespace FPB.Commands;

[Group("about", "about a user")]
public class AboutMe : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("me", "Show info about you or a user")]
    public async Task AboutMeAsync([Summary("user", "User whose info")]IGuildUser? user = null)
    {
        user ??= (IGuildUser) Context.User;

        string roles = "";
        bool space = true;
        foreach (ulong roleId in user.RoleIds)
        {
            if(Context.Guild.EveryoneRole.Id == roleId) continue;
            if (space) roles += $"\n<@&{roleId}> ";
            else roles += $"<@&{roleId}> ";
            space = !space;
        }
        
        EmbedBuilder infoEmbed = new EmbedBuilder()
            .WithTitle($"Member info: {user.Username}#{user.Discriminator}")
            .WithUrl($"https://discord.com/users/{user.Id}/")
            .WithThumbnailUrl(user.GetAvatarUrl())
            .WithDescription($"🔹 **Overall info**\n{user.Mention}\n`{user.Id}`")
            .AddField("🔹 Account creation date", $"<t:{user.CreatedAt.ToUnixTimeMilliseconds() / 1000}:f>\n<t:{user.CreatedAt.ToUnixTimeMilliseconds() / 1000}:R>")
            .AddField("🔹 Joined FramePaste", $"<t:{user.JoinedAt.Value.ToUnixTimeMilliseconds() / 1000}:f>\n<t:{user.JoinedAt.Value.ToUnixTimeMilliseconds() / 1000}:R>")
            .AddField("🔹 Roles", roles)
            .WithImageUrl($"https://cdn.discordapp.com/banners/{user.Id}/{((dynamic)await HttpRequest(url: $"https://discord.com/api/v8/users/{user.Id}", new Dictionary<string, string> {{"Authorization", $"Bot {LoadConfig().Token.ToString()}"}})).banner}.gif")
            .WithColor(GetEmbedColor());

        await RespondAsync(embed: infoEmbed.Build());
    }
}