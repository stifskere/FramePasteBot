using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.Rest;
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
        #pragma warning disable CS4014
        Task.Run(UpTimeUpdater);
        #pragma warning restore CS4014
        await ReactionRolesCreator.CreateEmbed();
        foreach (RestInviteMetadata invite in await Guild.GetInvitesAsync()) if(!UserJoined.InviteCounts.ContainsKey(invite.Id)) UserJoined.InviteCounts.Add(invite.Id, invite.Uses!.Value);
        await Guild.DownloadUsersAsync();
    }
    
    private static async Task InteractionCreated(SocketInteraction interaction)
    {
        await _commands!.ExecuteCommandAsync(new SocketInteractionContext(Client, interaction), null);
    }
    
    private static Task SlashCommandExecuted(SlashCommandInfo info, IInteractionContext context, IResult result)
    {
        if (result.Error != null) Console.WriteLine(result.ErrorReason);
        if (!CommandUses.ContainsKey(info.Module.Name)) CommandUses.Add(info.Module.Name, 0);
        CommandUses[info.Module.Name]++;
        return Task.CompletedTask;
    }

    private static void CreateDataBaseTables(DataBaseHandler db)
    {
        db.RunSqliteNonQueryCommand("CREATE TABLE IF NOT EXISTS BannedWords(BannedWord TEXT, UNIQUE(BannedWord))");
        db.RunSqliteNonQueryCommand("CREATE TABLE IF NOT EXISTS Cases(Id INTEGER PRIMARY KEY AUTOINCREMENT, UserId INTEGER, ModeratorId INTEGER, Reason TEXT, RemovalTime INT, Type TEXT, PunishmentTime INT)");
        db.RunSqliteNonQueryCommand("CREATE TABLE IF NOT EXISTS Configuration(key TEXT, value TEXT, UNIQUE(key))");
        db.RunSqliteNonQueryCommand("CREATE TABLE IF NOT EXISTS BlockedUsers(UserId INT, unique(UserId))");
        db.RunSqliteNonQueryCommand("CREATE TABLE IF NOT EXISTS Specs(userId INTEGER, list TEXT, unique(userId))");
        db.RunSqliteNonQueryCommand("CREATE TABLE IF NOT EXISTS CpuList(name TEXT, brand TEXT, price TEXT, cores TEXT ,threads TEXT, base TEXT, boost TEXT, socket TEXT, tdp TEXT, unique(name))");
        db.RunSqliteNonQueryCommand("CREATE TABLE IF NOT EXISTS GpuList(name TEXT, price TEXT, memoryInterface TEXT, vram TEXT, powerConnectors TEXT, tdp TEXT, imageUrl TEXT, pcieLink TEXT)");
        db.RunSqliteNonQueryCommand("CREATE TABLE IF NOT EXISTS Levels(Id INTEGER, Messages INTEGER, Days INTEGER, Level INTEGER)");
        try{db.RunSqliteNonQueryCommand("INSERT INTO Configuration(key, value) VALUES('EmbedColor', 'ffff00')");}catch{/*exists*/}
        try{db.RunSqliteNonQueryCommand("INSERT INTO Configuration(key, value) VALUES('EmbedRedColor', 'ff0000')");}catch{/*exists*/}
        try{db.RunSqliteNonQueryCommand("INSERT INTO Configuration(key, value) VALUES('EmbedGreenColor', '00ff00')");}catch{/*exists*/}
        try{db.RunSqliteNonQueryCommand("INSERT INTO Configuration(key, value) VALUES('YeesterCounter', '0')");}catch{/*exists*/}
    }

    private static async void UpTimeUpdater()
    {
        while (Client.ConnectionState == ConnectionState.Connected)
        {
            await Task.Delay(1);
            UpTime++;
            NowTime++;
        }
    }
}