using System.Data.SQLite;
using Discord;
using Discord.WebSocket;
using FPB.handlers;

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
            ModMailDictionary.Add(message.Author.Id, new ModMailHandler(message));
        }
        else
        {
           await ModMailDictionary[message.Author.Id].SendMessageAsync(message, true);
        }
    }
}