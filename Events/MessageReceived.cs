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

        ComponentBuilder userControls = new ComponentBuilder()
            .WithButton(new ButtonBuilder().WithCustomId("closeModMailUser").WithLabel("Close").WithStyle(ButtonStyle.Danger));

        await message.Channel.SendMessageAsync(embed: modMailEmbed.Build(), components: userControls.Build());

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
    }

    public static async void CloseModMailAsync(IGuild guild, string who)
    {
        IThreadChannel thread = guild.GetThreadChannelsAsync().Result.First(t => t.Id == ThreadId);
        IUser user = guild.GetUsersAsync().Result.First(u => u.Id == UserId);

        EmbedBuilder closeEmbed = new EmbedBuilder()
            .WithTitle($"Communication portal closed by {who}")
            .WithColor(GetEmbedColor(EmbedColors.EmbedRedColor))
            .WithDescription("If you want to open a new communication portal simply write another message in this channel.");

        await user.SendMessageAsync(embed: closeEmbed.Build());

        closeEmbed = closeEmbed.WithDescription("This thread will be archived, reopening it won't re enable the communication portal");

        await thread.SendMessageAsync(embed: closeEmbed.Build());

        await thread.GetMessagesAsync(limit: Int32.MaxValue).FirstAsync().Result.Last().DeleteAsync();
        
        await thread.ModifyAsync(t => t.Archived = true);
    }

    public static ulong ThreadId;
    public static ulong UserId;
}

public class ModMailComponents : InteractionModuleBase<SocketInteractionContext>
{
    [ComponentInteraction("closeModMailModerator")]
    public async Task CloseModMailModeratorAsync()
    {
        Console.WriteLine("Closed by moderators");
    }

    [ComponentInteraction("closeModMailUser")]
    public async Task CloseModMailUserAsync()
    {
        Console.WriteLine("Closed by user");
    }
}