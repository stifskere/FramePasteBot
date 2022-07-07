using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using FPB.Events;

namespace FPB.handlers;

public class ModMailHandler
{
    private static IUser? _user;
    private static IThreadChannel? _modMailThread;
    private static IMessage? _userMessage;
    private static IMessage? _guildMessage;

    private static readonly SocketGuild FramePasteGuild = Client.Guilds.First(g => g.Id == ulong.Parse(LoadConfig().GuildId.ToString()));
    private static readonly ITextChannel ModMailChannel = (ITextChannel)FramePasteGuild.Channels.First(c => c.Id == ulong.Parse(LoadConfig().Channels.ModMail.ToString()));
    
    public ModMailHandler(IMessage message)
    {
        _user = message.Author;
        GenerateTicketAsync().Wait();
    }

    private async Task GenerateTicketAsync()
    {
        EmbedBuilder enteringModMailEmbed = new EmbedBuilder()
            .WithTitle($"New mail from {_user!.GetTag()}")
            .WithDescription("Respond to the mail with the thread below.")
            .WithColor(GetEmbedColor());

        EmbedBuilder notifyNewTicketEmbed = new EmbedBuilder()
            .WithTitle("You opened a new mail")
            .WithDescription("Keep sending messages or close the mail with the red button below.")
            .WithColor(GetEmbedColor());

        EmbedBuilder threadTicketControls = new EmbedBuilder()
            .WithTitle("Ticket controls")
            .WithColor(GetEmbedColor());
        
        ComponentBuilder ticketControls = new ComponentBuilder()
            .WithButton(new ButtonBuilder().WithCustomId("CloseModMail").WithStyle(ButtonStyle.Danger).WithLabel("Close mail"));
        
        _userMessage = await _user.SendMessageAsync(embed: notifyNewTicketEmbed.Build(), components: ticketControls.Build());
        _guildMessage = await ModMailChannel.SendMessageAsync(embed: enteringModMailEmbed.Build(), text: $"<@&{FramePasteGuild.EveryoneRole.Id}>");
        _modMailThread = await ModMailChannel.CreateThreadAsync(name: _user!.Id.ToString(), type: ThreadType.PublicThread, message: _guildMessage);
        await _modMailThread.SendMessageAsync(components: ticketControls.Build(), embed: threadTicketControls.Build());
    }

    public async Task SendMessageAsync(IMessage message, bool isUser)
    {
        if (isUser)
        {
            await _modMailThread!.SendMessageAsync($"{message.Author.GetTag()} - {message.Content}");
        }
        else
        {
            await _user.SendMessageAsync($"{message.Author.GetTag()} - {message.Content}");
        }
    }
    
    public async Task CloseMailAsync()
    {
        EmbedBuilder closeMailServerEmbed = new EmbedBuilder()
            .WithTitle("This mail was closed.")
            .WithDescription("You can't reactivate this mail, wait for the user to send another mail.")
            .WithColor(GetEmbedColor());

        EmbedBuilder closeMailUserEmbed = new EmbedBuilder()
            .WithTitle("This mail was closed.")
            .WithDescription("To make another mail simply send another message in this channel.")
            .WithColor(GetEmbedColor());

        await ((IUserMessage)_userMessage!).ModifyAsync(m => m.Components = new ComponentBuilder().Build());
        await ((IUserMessage)_guildMessage!).ModifyAsync(m => {m.Components = new ComponentBuilder().Build(); m.Embed = closeMailServerEmbed.Build();});
        await _modMailThread!.DeleteAsync();
        await _user.SendMessageAsync(embed: closeMailUserEmbed.Build());

        MessageReceived.ModMailDictionary.Remove(_user!.Id);
    }
}

public class ModMailButtons : InteractionModuleBase<SocketInteractionContext>
{
    [ComponentInteraction("CloseModMail")]
    public async Task CloseModMailAsync()
    {
        if (Context.Channel.GetType() == typeof(SocketThreadChannel))
        {
            await MessageReceived.ModMailDictionary[ulong.Parse(Context.Channel.Name)].CloseMailAsync();
        }
        else if (Context.Channel.GetType() == typeof(SocketDMChannel))
        {
            await MessageReceived.ModMailDictionary[Context.User.Id].CloseMailAsync();
        }
    }
}
