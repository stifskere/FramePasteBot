using Discord;

namespace FPB.Events;

public static class MessageDeleted
{
    private static IUserMessage? _message;
    private static IMessageChannel? _channel;
    public static async Task Event(Cacheable<IMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel)
    {
        _channel = (IMessageChannel) Client.Guilds.First(g => g.Id == ulong.Parse(LoadConfig().GuildId.ToString())).Channels.First(c => c.Id == cachedChannel.Id);
        _message = (IUserMessage) await _channel.GetMessageAsync(cachedMessage.Id, CacheMode.AllowDownload, RequestOptions.Default);
        
        
    }
}