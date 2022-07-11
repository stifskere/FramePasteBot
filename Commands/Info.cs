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
        EmbedBuilder installWinEmbed = new EmbedBuilder()
            .WithTitle("How to create a windows 10 installation media")
            .WithDescription("This method only works on Windows 10 PCs. Please ask for other methods if using a Mac, Linux, or earlier versions of Windows.\nhttps://www.microsoft.com/software-download/windows10\n**1.** Insert an 8GB or higher USB stick into your PC. Format the USB to FAT32 or exFAT before installation. Formatting will ERASE ALL DATA on the USB. (Right click the USB > Format > File System: FAT32 > Quick Format > Start.)\n**2.** Download the Windows MCT (Media Creation Tool) from the link above.\n**3.** Run the MCT. Select \"Create installation media for another PC.\" Follow the prompts to select your USB flash drive as your media and create the installer.\n**4.** Once complete, your USB is now ready to install Windows on another PC. Power down that PC. Insert the USB installer. Boot the PC.\n**WARNING:** When installing Windows on a new PC, ensure only one drive (the drive you want the OS on) is plugged in. Unplug all other drives prior to installing Windows.")
            .WithColor(GetEmbedColor())
            .WithFooter(text: "cloned from discord.gg/buildapc", iconUrl: "https://cdn.discordapp.com/icons/286168815585198080/a_e1016a9b8d8f7c97dafef6b655e0d1b1.webp");

        await RespondAsync(embed: installWinEmbed.Build());
    }

    [SlashCommand("ssd", "Shows all times of ssds")]
    public async Task SsdAsync()
    {
        await RespondAsync(text: "https://media.discordapp.net/attachments/873064255690264596/917983590225166346/SSD_1.png", allowedMentions: new AllowedMentions(allowedTypes: null));
    }

    [SlashCommand("moboff", "Shows a GIF of each motherboard form factor")]
    public async Task MoboffAsync()
    {
        await RespondAsync(text: "https://cdn.discordapp.com/attachments/778848112588095559/873406468483862528/20210807_102414.gif", allowedMentions: new AllowedMentions(allowedTypes: null));
    }
}