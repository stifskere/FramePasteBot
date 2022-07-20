using Discord;
using Discord.WebSocket;
using FPB.handlers;

namespace FPB.Events;

public static class MessageUpdated
{
    private static IUserMessage? _oldMessage;
    private static IUserMessage? _newMessage;
    private static IMessageChannel? _channel;
    public static async Task Event(Cacheable<IMessage, ulong> oldMessage, SocketMessage message, ISocketMessageChannel channel)
    {
        _oldMessage = (IUserMessage) await oldMessage.GetOrDownloadAsync();
        _newMessage = (IUserMessage) message;
        _channel = channel;

        if (!_newMessage.Author.IsBot) await LogEdit();
    }

    private static async Task LogEdit()
    {
        EmbedBuilder logEditEmbed = new EmbedBuilder()
            .WithTitle("Message Edited!")
            .WithAuthor($"{_newMessage!.Author.GetTag()} - {_newMessage.Author.Id}", iconUrl: _newMessage.Author.GetAvatarUrl())
            .WithDescription($"<@{_newMessage.Author.Id}> edited a message\nwhich content was:\n```{_oldMessage!.Content.Replace("`", "")}```\nand now is:\n```{_newMessage.Content.Replace("`", "")}```\nin channel: {_channel!.GetMention()}")
            .WithCurrentTimestamp()
            .WithColor(GetEmbedColor(EmbedColors.EmbedRedColor));

        ComponentBuilder logComponents = new ComponentBuilder()
            .WithButton(new ButtonBuilder().WithLabel("Jump to message").WithStyle(ButtonStyle.Link).WithUrl(_newMessage.GetJumpUrl()));
        
        await SendLog(embed: logEditEmbed.Build(), components: logComponents.Build());
    }
}