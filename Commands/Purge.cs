using Discord;
using Discord.Interactions;

namespace FPB.Commands;

public class Purge : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("purge", "Deletes the selected num of messages from the current channel")]
    public async Task PurgeAsync([MaxValue(100), MinValue(1)]int messages, IGuildUser? user = null)
    {
        await DeferAsync();
        var firstMessage = await GetOriginalResponseAsync();
        IEnumerable<IMessage> cachedMessages = await Context.Channel.GetMessagesAsync(fromMessageId: firstMessage.Id, Direction.Before).FlattenAsync();
        cachedMessages = cachedMessages.Where(m => (DateTimeOffset.UtcNow - m.Timestamp).TotalDays <= 14);
        if (user != null) cachedMessages = cachedMessages.Where(m => m.Author.Id == user.Id);
        List<IMessage> enumerable = cachedMessages.ToList();
        enumerable.RemoveRange(messages, enumerable.Count - messages);
        EmbedBuilder purgeEmbed = new EmbedBuilder()
            .WithTitle("Message deletion")
            .WithDescription($"**I've deleted:** {enumerable.Count} messages\n{(messages == enumerable.Count? "" : $"**But you asked to delete:** {messages} messages\n")}**In:** {Context.Channel.GetMention()}{(user == null ? "" : $"\n**From:** {user.GetTag()}")}")
            .WithColor(GetEmbedColor());
        await ((ITextChannel)Context.Channel).DeleteMessagesAsync(enumerable);
        await ModifyOriginalResponseAsync(m => m.Embed = new Optional<Embed>(purgeEmbed.Build()));
    }
}