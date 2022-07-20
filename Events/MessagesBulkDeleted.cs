using System.Collections.ObjectModel;
using Discord;
using FPB.handlers;

namespace FPB.Events;

public static class MessagesBulkDeleted
{
    public static async Task Event(IReadOnlyCollection<Cacheable<IMessage,ulong>> m , Cacheable<IMessageChannel,ulong> c)
    {
        List<IMessage?> mC = new();
        foreach (Cacheable<IMessage, ulong> cacheableMessage in m) mC.Add(await cacheableMessage.GetOrDownloadAsync());
        await RunEvent(new ReadOnlyCollection<IMessage?>(mC), await c.GetOrDownloadAsync());
    }

    private static async Task RunEvent(IReadOnlyCollection<IMessage?> messages, IMessageChannel channel)
    {
        string messageLog = messages.Aggregate("", (current, message) => current + $"{message?.Author.GetTag() ?? "User not found"} - {message?.Content ?? "[Embed or not found]"}\n");
        if (messageLog.Length > 4000) messageLog = string.Concat(messageLog.AsSpan(0, 3500), "\n\nThe messages were too large to display them all...");
        EmbedBuilder logEmbed = new EmbedBuilder()
            .WithTitle($"{messages.Count} Messages were bulk deleted")
            .WithDescription($"**in:** {channel.GetMention()}\n**Message log:**\n```{messageLog.Replace("`", "")}```")
            .WithColor(GetEmbedColor())
            .WithCurrentTimestamp();

        await SendLog(embed: logEmbed.Build());
    }
}