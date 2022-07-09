﻿using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using FPB.handlers;

namespace FPB.Events;

public static class Ready
{
    private static InteractionService? _commands;
    public static async Task Event()
    {
        Console.WriteLine($"Bot started as {Client.CurrentUser.Username}");
        await Task.Run(() => CreateDataBaseTables(DataBase));
        Client.InteractionCreated += InteractionCreated;
        _commands = new InteractionService(Client);
        _commands.SlashCommandExecuted += SlashCommandExecuted;
        await _commands.AddModulesAsync(Assembly.GetExecutingAssembly(), null);
        await _commands.RegisterCommandsToGuildAsync(ulong.Parse(LoadConfig().GuildId.ToString()), true);
        BanHandler = new BanManager();
    }
    
    private static async Task InteractionCreated(SocketInteraction interaction)
    {
        await _commands!.ExecuteCommandAsync(new SocketInteractionContext(Client, interaction), null);
    }
    
    private static Task SlashCommandExecuted(SlashCommandInfo info, IInteractionContext context, IResult result)
    {
        if(result.Error != null) Console.WriteLine(result.ErrorReason);
        return Task.CompletedTask;
    }

    private static void CreateDataBaseTables(DataBaseHandler db)
    {
        db.RunSqliteNonQueryCommand("CREATE TABLE IF NOT EXISTS BannedWords(BannedWord STRING, UNIQUE(BannedWord))");
        db.RunSqliteNonQueryCommand("CREATE TABLE IF NOT EXISTS Cases(Id INTEGER PRIMARY KEY AUTOINCREMENT, UserId INTEGER, ModeratorId INTEGER, Reason STRING, RemovalTime INT, Type STRING, PunishmentTime INT)");
        db.RunSqliteNonQueryCommand("CREATE TABLE IF NOT EXISTS Configuration(key STRING, value STRING, UNIQUE(key))");
        db.RunSqliteNonQueryCommand("CREATE TABLE IF NOT EXISTS BlockedUsers(UserId INT, unique(UserId))");
        try{db.RunSqliteNonQueryCommand("INSERT INTO Configuration(key, value) VALUES('EmbedColor', 'ffff00')");}catch{/*exists*/}
        try{db.RunSqliteNonQueryCommand("INSERT INTO Configuration(key, value) VALUES('EmbedRedColor', 'ff0000')");}catch{/*exists*/}
        try{db.RunSqliteNonQueryCommand("INSERT INTO Configuration(key, value) VALUES('EmbedGreenColor', '00ff00')");}catch{/*exists*/}
        try{db.RunSqliteNonQueryCommand("INSERT INTO Configuration(key, value) VALUES('YeesterCounter', '0')");}catch{/*exists*/}
    }
}