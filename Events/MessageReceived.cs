using System.ComponentModel;
using System.Data.SQLite;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using FPB.handlers;

namespace FPB.Events;

public static class MessageReceived
{
    public static IMessage eventMessage;
    public static async Task Event(SocketMessage message)
    {
        if (message.Author.IsBot) return;
        eventMessage = message;
        if (message.Channel.GetType() == typeof(SocketDMChannel))
        {
            await DmCase(message);
            return;
        }

        bool isChannelNameParseable;
        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
        try {ulong.Parse(message.Channel.Name); isChannelNameParseable = true;}catch{isChannelNameParseable = false;}
        
        if (isChannelNameParseable && ModMailDictionary.ContainsKey(ulong.Parse(message.Channel.Name)))
        {
            await ModMailDictionary[ulong.Parse(message.Channel.Name)].SendMessageAsync(message, false);
            return;
        }
        
        if(message.Author.Id == ulong.Parse(LoadConfig().YeesterId.ToString())) DataBase.RunSqliteNonQueryCommand($"UPDATE Configuration SET value = {NowTime} WHERE Key = 'YeesterCounter'");

        SQLiteDataReader insultsQuery = DataBase.RunSqliteQueryCommand("SELECT * FROM BannedWords");
        while (insultsQuery.Read())
        {
            if (message.Content.Contains(insultsQuery.GetString(0)))
            {
                await message.DeleteAsync();
                break;
            }
        }
    }

    public static Dictionary<ulong, ModMailHandler> ModMailDictionary = new();

    private static async Task DmCase(SocketMessage message)
    {
        SQLiteDataReader blockedUserQuery = DataBase.RunSqliteQueryCommand("SELECT UserId FROM BlockedUsers");
        while (blockedUserQuery.Read()) if (blockedUserQuery.GetInt64(0) == (long)message.Author.Id)
        {
            await message.Channel.SendMessageAsync("You are blocked from sending ModMail requests.", messageReference: new MessageReference(message.Id));
            return;
        }

        if (!ModMailDictionary.ContainsKey(message.Author.Id))
        {
            EmbedBuilder startQuestionEmbed = new EmbedBuilder()
                .WithTitle("You are going to start a ModMail")
                .WithDescription("Are you sure you want to start a Mail?")
                .WithColor(GetEmbedColor());

            ComponentBuilder startQuestionComponents = new ComponentBuilder()
                .WithButton(new ButtonBuilder().WithLabel("Yes").WithEmote(new Emoji("✔️")).WithStyle(ButtonStyle.Success).WithCustomId("StartModMail"))
                .WithButton(new ButtonBuilder().WithLabel("No").WithEmote(new Emoji("❌")).WithStyle(ButtonStyle.Danger).WithCustomId("DontStartModMail"));

            await message.Channel.SendMessageAsync(embed: startQuestionEmbed.Build(), components: startQuestionComponents.Build());
        }
        else
        {
           await ModMailDictionary[message.Author.Id].SendMessageAsync(message, true);
        }
    }
}

public class StartModMailComponents : InteractionModuleBase<SocketInteractionContext>
{
    [ComponentInteraction("StartModMail")]
    public async Task StartModMailAsync()
    {
        await DeferAsync();
        
        IUserMessage originalMessage = await GetOriginalResponseAsync();

        await originalMessage.ModifyAsync(m => m.Components = new Optional<MessageComponent>(new ComponentBuilder().Build()));
        
        MessageReceived.ModMailDictionary.Add(MessageReceived.eventMessage.Author.Id, new ModMailHandler(MessageReceived.eventMessage));
    }

    [ComponentInteraction("DontStartModMail")]
    public async Task DontStartModMailASync()
    {
        await DeferAsync();
        
        IUserMessage originalMessage = await GetOriginalResponseAsync();
        
        await originalMessage.ModifyAsync(m => m.Components = new Optional<MessageComponent>(new ComponentBuilder().Build()));
    }
}