using System.Net;
using Discord;
using Discord.Interactions;

namespace FPB.Commands;

[Group("info", "Info commands")]
public class Info : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("80plus", "Info card about 80plus psu ratings")]
    public async Task EightyPlusAsync()
    {
        await RespondAsync(text: "https://www.guru3d.com/index.php?ct=articles&action=file&id=10061", allowedMentions: new AllowedMentions(allowedTypes: null));
    }

    [SlashCommand("ask", "Links to don't ask to ask")]
    public async Task AskAsync()
    {
        EmbedBuilder askEmbed = new EmbedBuilder()
            .WithTitle("Don't ask to ask")
            .WithDescription("Don't expect someone to take responsibility for your question before they know what it is. Ask first. Someone will respond if they can and want to help.")
            .WithFooter(text: "cloned from discord.gg/buildapc", iconUrl: "https://cdn.discordapp.com/icons/286168815585198080/a_e1016a9b8d8f7c97dafef6b655e0d1b1.webp")
            .WithColor(GetEmbedColor());

        await RespondAsync(embed: askEmbed.Build(), allowedMentions: new AllowedMentions(allowedTypes: null));
    }
}