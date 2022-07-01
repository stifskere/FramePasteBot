using System.Data.SQLite;
using Discord;
using Discord.WebSocket;

namespace FPB.Events;

public static class MessageReceived
{
    public static async Task Event(SocketMessage message)
    {
        if (message.Channel.GetType() == typeof(SocketDMChannel))
        {
            await DmCase(message);
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

    private static async Task DmCase(SocketMessage message)
    {
        
    }
}