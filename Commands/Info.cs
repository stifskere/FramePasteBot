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

    [SlashCommand("hwinfo", "Gives a link to download HW-Info")]
    public async Task HwInfoAsync()
    {
        await RespondAsync("https://www.hwinfo.com/", allowedMentions: new AllowedMentions(allowedTypes: null));
    }

    [SlashCommand("timenames", "Explains the time names used in commands for this bot.")]
    public async Task TimeNamesAsync()
    {
        EmbedBuilder timesEmbed = new EmbedBuilder()
            .WithTitle("Time names")
            .WithDescription("Time names are a way to express an amount of time, eg 1 hour `1h`\n\nThe supported time names include\n1 Minute - `1m`\n1 Hour - `1h`\n1 Day - `1d`\n1 Week - `1w`\n\nSince 1 month is 4 weeks you can just add 8 weeks if you want 2 months.\nIn this bot there is no `1m 15s` yet since i used the default c# methods to achieve times and i didn't make any handler.")
            .WithColor(GetEmbedColor());

        await RespondAsync(embed: timesEmbed.Build(), allowedMentions: new AllowedMentions(allowedTypes: null));
    }

    [SlashCommand("win", "Shows how to create windows 10 installation media")]
    public async Task WindowsInstallationAsync()
    {
        await RespondAsync("Command not implemented yet", ephemeral: true);
        
        EmbedBuilder installWinEmbed = new EmbedBuilder()
            .WithTitle("How to create a windows 10 installation media")
            .WithDescription("");
    }
    
    
}