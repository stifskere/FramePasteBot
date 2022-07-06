using System.Data.SQLite;
using Discord;
using Discord.WebSocket;

namespace FPB.Events;

public static class MessageReceived
{
    public static async Task Event(SocketMessage message)
    {
        if (message.Author.IsBot) return;
        if (message.Channel.GetType() == typeof(SocketDMChannel) || message.Channel.GetType() == typeof(SocketThreadChannel))
        {
            await DmCase(message);
            return;
        }
        
        if(message.Author.Id == ulong.Parse(LoadConfig().YeesterId.ToString())) DataBase.RunSqliteNonQueryCommand($"UPDATE Configuration SET value = {NowTime} WHERE Key = 'YeesterCounter'");

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

    private static async Task DmCase(SocketMessage message)
    {
        SQLiteDataReader blockedUserQuery = DataBase.RunSqliteQueryCommand("SELECT UserId FROM BlockedUsers");
        while (blockedUserQuery.Read()) if (blockedUserQuery.GetInt64(0) == (long)message.Author.Id)
        {
            await message.Channel.SendMessageAsync("You are blocked from sending ModMail requests.", messageReference: new MessageReference(message.Id));
            return;
        }
    }
}