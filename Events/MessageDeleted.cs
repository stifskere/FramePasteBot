using Discord;
using FPB.handlers;

namespace FPB.Events;

public static class MessageDeleted
{
    private static IUserMessage? _message;
    private static IMessageChannel? _channel;
    public static async Task Event(Cacheable<IMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel)
    {
        _channel = await cachedChannel.GetOrDownloadAsync();
        _message = (IUserMessage) await cachedMessage.GetOrDownloadAsync();
        
        if(_message.Author.IsBot) return;
        if (!string.IsNullOrEmpty(_message.Content)) await LogDeletion();
        
        LevelHandler.CheckLevelAndCountMessage(_message.Author, LevelHandler.Method.Remove);
    }

    private static async Task LogDeletion()
    {
        EmbedBuilder logDeletionEmbed = new EmbedBuilder()
            .WithTitle("Message Deleted!")
            .WithAuthor($"{_message!.Author.GetTag()} - {_message.Author.Id}", iconUrl: _message.Author.GetAvatarUrl())
            .WithDescription($"<@{_message.Author.Id}> deleted a message\nwhich content was:\n```{_message.Content.Replace("`", "")}```\nin channel: {_channel!.GetMention()}")
            .WithCurrentTimestamp()
            .WithColor(GetEmbedColor(EmbedColors.EmbedRedColor));

        await SendLog(embed: logDeletionEmbed.Build());
    }
}