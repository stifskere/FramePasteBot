using System.Data.SQLite;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace FPB.Events;

public static class MessageReceived
{
    public static async Task Event(SocketMessage message)
    {
        if (message.Author.IsBot) return;
        if (message.Channel.GetType() == typeof(SocketDMChannel))
        {
            await DmCase(message);
            return;
        }

        if (message.Channel.GetType() == typeof(SocketThreadChannel))
        {
            await ThreadCase(message);
            return;
        }
        
        SQLiteDataReader insultsQuerry = DataBase.RunSqliteQueryCommand("SELECT * FROM BannedWords");
        while (insultsQuerry.Read())
        {
            if (message.Content.Contains(insultsQuerry.GetString(0)))
            {
                await message.DeleteAsync();
                break;
            }
        }
    }

    private static async Task ThreadCase(SocketMessage message)
    {
        
    }

    private static async Task DmCase(SocketMessage message)
    {
        SQLiteDataReader blockedUserQuery = DataBase.RunSqliteQueryCommand("SELECT UserId FROM BlockedUsers");
        while (blockedUserQuery.Read()) if (blockedUserQuery.GetInt64(0) == (long)message.Author.Id)
        {
            await message.Channel.SendMessageAsync("You are blocked from sending ModMail requests.", messageReference: new MessageReference(message.Id));
            return;
        }

        if (!ActiveMailList.ContainsKey(message.Author.Id))
        {
            ActiveMailList.Add(message.Author.Id, new ModMail(message));
        }
        else
        {
            Console.WriteLine(message.Channel.GetType());
            if (message.Channel.GetType() == typeof(IThreadChannel))
            {
                Console.WriteLine("thread");
                ActiveMailList[ulong.Parse(message.Channel.Name)].SendMessageAsync(false, message);
            }
            else
            {
                Console.WriteLine("user");
                ActiveMailList[message.Author.Id].SendMessageAsync(true, message);
            }
        }
    }

    public static Dictionary<ulong, ModMail> ActiveMailList = new();
}

public class ModMail
{
    public ModMail(IMessage message)
    {
        GeneratePortal(message);
    }

    private async void GeneratePortal(IMessage message)
    {
        EmbedBuilder modMailEmbed = new EmbedBuilder()
            .WithTitle("A communication portal has been opened")
            .WithDescription("Start writing messages in this channel to communicate with the moderators.\n\nYou can press the button below to close the communication thread with the moderators.")
            .WithColor(GetEmbedColor());

        ComponentBuilder userControls = new ComponentBuilder().WithButton(new ButtonBuilder().WithCustomId("closeModMailUser").WithLabel("Close").WithStyle(ButtonStyle.Danger));

        IUserMessage firstUserMessage = await message.Channel.SendMessageAsync(embed: modMailEmbed.Build(), components: userControls.Build());

        SocketGuildChannel modMailChannel = Client.Guilds.First(g => g.Id == ulong.Parse(LoadConfig().GuildId.ToString())).Channels.First(c => c.Id == ulong.Parse(LoadConfig().Channels.ModMail.ToString()));

        EmbedBuilder moderatorModMailEmbed = new EmbedBuilder()
            .WithTitle($"{message.Author.Username}#{message.Author.Discriminator} opened a communication portal")
            .WithDescription("Send messages in the thread below to communicate\nThere is a control panel inside the thread.")
            .WithColor(GetEmbedColor());

        await ((ITextChannel)modMailChannel).SendMessageAsync(embed: moderatorModMailEmbed.Build());

        IMessage referenceMessage = ((ITextChannel)modMailChannel).GetMessagesAsync(limit: 1).FirstAsync().Result.First();
        
        IThreadChannel thread = await ((ITextChannel)modMailChannel).CreateThreadAsync(message.Author.Id.ToString(), message: referenceMessage);
        
        EmbedBuilder threadModMailEmebd = new EmbedBuilder()
            .WithTitle("ModMail controls")
            .WithDescription("You can close or block the user from here.")
            .WithColor(GetEmbedColor());

        ComponentBuilder controlComponents = new ComponentBuilder()
            .WithButton(new ButtonBuilder().WithCustomId("closeModMailModerator").WithLabel("Close").WithStyle(ButtonStyle.Danger))
            .WithButton(new ButtonBuilder().WithCustomId("blockUserModMail").WithLabel("Block user").WithStyle(ButtonStyle.Secondary));

        await thread.SendMessageAsync(embed: threadModMailEmebd.Build(), components: controlComponents.Build());

        ThreadId = thread.Id;
        UserId = message.Author.Id;
        FirstUserMessage = firstUserMessage;
    }

    public async void SendMessageAsync(bool isUser, IMessage message)
    {
        IGuild guild = Client.Guilds.First(g => g.Id == ulong.Parse(LoadConfig().GuildId.ToString()));
        
        if (isUser)
        {
            IThreadChannel thread = guild.GetThreadChannelsAsync().Result.First(t => t.Id == ThreadId);
            await thread.SendMessageAsync($"From {message.Author.Username}:\n{message.Content}");
        }
        else
        {
            IUser user = guild.GetUsersAsync().Result.First(u => u.Id == UserId);
            await user.SendMessageAsync($"from {message.Author}:\n{message.Content}");
        }
    }
    
    public async void CloseModMailAsync(string who, bool blocked = false)
    {
        IGuild guild = Client.Guilds.First(g => g.Id == ulong.Parse(LoadConfig().GuildId.ToString()));
        IThreadChannel thread = guild.GetThreadChannelsAsync().Result.First(t => t.Id == ThreadId);
        Console.WriteLine(thread.Name);
        IUser user = guild.GetUsersAsync().Result.First(u => u.Id == UserId);

        EmbedBuilder closeEmbed = new EmbedBuilder()
            .WithTitle($"Communication portal closed by {who}")
            .WithColor(GetEmbedColor(EmbedColors.EmbedRedColor))
            .WithDescription("If you want to open a new communication portal simply write another message in this channel.");

        await user.SendMessageAsync(embed: closeEmbed.Build());

        if (blocked) closeEmbed = closeEmbed.WithTitle("User blocked and communication portal closed.");
        closeEmbed = closeEmbed.WithDescription("This thread will be archived, reopening it won't re enable the communication portal");

        await thread.SendMessageAsync(embed: closeEmbed.Build());

        int threadCount = thread.GetMessagesAsync(limit: Int32.MaxValue).FirstAsync().Result.Count;
        await thread.GetMessagesAsync(limit: threadCount - 1).FirstAsync().Result.Last().DeleteAsync();
        
        await thread.ModifyAsync(t => t.Archived = true);

        await ((IUserMessage)FirstUserMessage).ModifyAsync(m => m.Components = new ComponentBuilder().Build());
    }

    public static IMessage FirstUserMessage;
    public static ulong ThreadId;
    public static ulong UserId;
}

public class ModMailComponents : InteractionModuleBase<SocketInteractionContext>
{
    [ComponentInteraction("closeModMailModerator")]
    public async Task CloseModMailModeratorAsync()
    {
        await DeferAsync();
        await DeleteOriginalResponseAsync();
        MessageReceived.ActiveMailList[ulong.Parse(Context.Channel.Name)].CloseModMailAsync("the moderators");
    }

    [ComponentInteraction("closeModMailUser")]
    public async Task CloseModMailUserAsync()
    {
        await DeferAsync();
        await DeleteOriginalResponseAsync();
        MessageReceived.ActiveMailList[ulong.Parse(Context.User.Id.ToString())].CloseModMailAsync("the user");
    }

    [ComponentInteraction("blockUserModMail")]
    public async Task BlockUserAsync()
    {
        DataBase.RunSqliteNonQueryCommand($"INSERT INTO BlockedUsers(UserId) VALUES({Context.Channel.Name})");
        MessageReceived.ActiveMailList[ulong.Parse(Context.User.Id.ToString())].CloseModMailAsync("the user", true);
    }
}