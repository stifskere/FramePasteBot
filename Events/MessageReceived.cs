﻿using System.Data.SQLite;
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
            .WithButton(new ButtonBuilder(label: "close", style: ButtonStyle.Danger, customId: "closeModMail"));

        await message.Channel.SendMessageAsync(embed: modMailEmbed.Build(), components: userControls.Build());

        SocketGuildChannel modMailChannel = Client.Guilds.First(g => g.Id == ulong.Parse(LoadConfig().GuildId.ToString())).Channels.First(c => c.Id == ulong.Parse(LoadConfig().Channels.ModMail.ToString()));

        EmbedBuilder moderatorModMailEmbed = new EmbedBuilder()
            .WithTitle($"{message.Author.Username}#{message.Author.Discriminator} opened a communication portal")
            .WithDescription("Send messages in the thread below to communicate\nThere is a control panel inside the thread.")
            .WithColor(GetEmbedColor());

        await ((ITextChannel)modMailChannel).SendMessageAsync(embed: moderatorModMailEmbed.Build());

        IMessage referenceMessage = ((ITextChannel)modMailChannel).GetMessagesAsync(limit: 1).FirstAsync().Result.First();

        ((ITextChannel)modMailChannel).CreateThreadAsync(message.Author.Id.ToString(), message: referenceMessage).Wait();

        IThreadChannel thread = ((ITextChannel)modMailChannel).Guild.GetThreadChannelsAsync().Result.First();

        EmbedBuilder threadModMailEmebd = new EmbedBuilder()
            .WithTitle("ModMail controls")
            .WithDescription("You can close or block the user from here.")
            .WithColor(GetEmbedColor());

        ComponentBuilder controlComponents = new ComponentBuilder()
            .WithButton(new ButtonBuilder(label: "close", style: ButtonStyle.Danger, customId: "closeModMail"))
            .WithButton(new ButtonBuilder(label: "block", style: ButtonStyle.Secondary, customId: "blockUserModMail"));

        await thread.SendMessageAsync(embed: threadModMailEmebd.Build(), components: controlComponents.Build());

        ThreadId = thread.Id;
        UserId = message.Author.Id;
    }

    public ulong ThreadId = 0;
    public ulong UserId = 0;
}

class ModMailComponents : InteractionModuleBase<SocketInteractionContext>
{
    [ComponentInteraction("closeModMail")]
    public async Task CloseModMailAsync()
    {
        EmbedBuilder closeEmbed = new EmbedBuilder()
            .WithTitle("This communication portal has been closed")
            .WithColor(GetEmbedColor(EmbedColors.EmbedRedColor));
        
        Console.WriteLine(Context.Channel);
        
        if (Context.Channel.GetType() == typeof(IDMChannel))
        {
            closeEmbed = closeEmbed.WithDescription("To start another simply send another message in this channel.");
            await Context.Channel.SendMessageAsync(embed: closeEmbed.Build());

            SocketThreadChannel thread = Client.Guilds.First(g => g.Id == LoadConfig().GuildId.ToString()).ThreadChannels.First(c => c.Id == MessageReceived.ActiveMailList[Context.User.Id].ThreadId);
            
            closeEmbed = closeEmbed.WithDescription("This thread has been closed by the user, it will be left by the bot and it will remain inactive, there is nothing to do to reactivate the thread.");
            await thread.SendMessageAsync(embed: closeEmbed.Build());

            await thread.LeaveAsync();
        }
        else if (Context.Channel.GetType() == typeof(SocketThreadChannel))
        {
            closeEmbed = closeEmbed.WithDescription("This thread has been closed, it will be left by the bot and it will remain inactive, there is nothing to do to reactivate the thread.");
            await Context.Channel.SendMessageAsync(embed: closeEmbed.Build());
        }
    }
}