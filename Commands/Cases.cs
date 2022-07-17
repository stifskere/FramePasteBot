using System.Data.SQLite;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using FPB.handlers;

namespace FPB.Commands;

public class Cases : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ButtonBuilder _buttonRight = new ButtonBuilder().WithStyle(ButtonStyle.Primary).WithEmote(new Emoji("➡️")).WithCustomId("ButtonRight").WithLabel("Next");
    private readonly ButtonBuilder _buttonLeft = new ButtonBuilder().WithStyle(ButtonStyle.Primary).WithEmote(new Emoji("⬅️")).WithCustomId("ButtonLeft").WithLabel("Back");
    
    private readonly ButtonBuilder _buttonRightDisabled = new ButtonBuilder().WithStyle(ButtonStyle.Primary).WithEmote(new Emoji("➡️")).WithCustomId("ButtonRight").WithLabel("Next").WithDisabled(true);
    private readonly ButtonBuilder _buttonLeftDisabled = new ButtonBuilder().WithStyle(ButtonStyle.Primary).WithEmote(new Emoji("⬅️")).WithCustomId("ButtonLeft").WithLabel("Back").WithDisabled(true);
    
    private static readonly List<List<string[]>> CasesList = new();
    private static readonly Dictionary<ulong, EmbedCounter> EmbedCounters = new();
    
    [SlashCommand("cases", "View all cases")]
    public async Task ViewCasesAsync(IGuildUser? user = null, [Choice("Warn", "Warn"), Choice("Kick", "Kick"), Choice("Ban", "Ban"), Choice("Time Out", "Mute")]string? type = null, [Choice("From top to bottom", "ASC"), Choice("From bottom to top", "DESC")]string order = "ASC")
    {
        SocketGuild guild = Client.Guilds.First(g => g.Id == ulong.Parse(LoadConfig().GuildId.ToString()));
        SQLiteDataReader casesData = DataBase.RunSqliteQueryCommand("SELECT * FROM Cases");

        await Context.Guild.DownloadUsersAsync();
        
        if (user != null && type != null)
        {
            casesData = DataBase.RunSqliteQueryCommand($"SELECT * FROM Cases WHERE UserId = {user.Id} AND Type = '{type}' ORDER BY Id {order}");
        }
        else if (user != null && type == null)
        {
            casesData = DataBase.RunSqliteQueryCommand($"SELECT * FROM Cases WHERE UserId = {user.Id} ORDER BY Id {order}");
        }
        else if (user == null && type != null)
        {
            casesData = DataBase.RunSqliteQueryCommand($"SELECT * FROM Cases WHERE Type = '{type}' ORDER BY Id {order}");
        }
        else if (user == null && type == null)
        {
            casesData = DataBase.RunSqliteQueryCommand($"SELECT * FROM Cases ORDER BY Id {order}");
        }
        
        int listOneIndex = -1;
        int listTwoIndex = 5;

        CasesList.Clear();
        while (casesData.Read())
        {
            if (listTwoIndex == 5)
            {
                CasesList.Add(new List<string[]>());
                listOneIndex++;
                listTwoIndex = 0;
            }

            IGuildUser? caseUser = null;
            IGuildUser? caseModerator = null;

            try
            {
                caseUser = guild.Users.FirstOrDefault(u => u.Id == ulong.Parse(casesData.GetInt64(1).ToString()));
                caseModerator = guild.Users.FirstOrDefault(u => u.Id == ulong.Parse(casesData.GetInt64(2).ToString()));
            }
            catch
            {
                //not found.
            }

            CasesList[listOneIndex].Add(new[] {$"Case {casesData.GetInt32(0)} | {casesData.GetString(5)}", $"**User:** {(caseUser != null? caseUser.GetTag():"User not found.")} - `{casesData.GetInt64(1)}`\n**Moderator:** {(caseModerator != null? caseModerator.GetTag():"User not found.")} - `{casesData.GetInt64(2)}`\n**Reason:** {casesData.GetString(3)}\n{(casesData.GetInt64(4) == 0 ? "" : $"**Removed punishment at:** <t:{casesData.GetInt64(4)}:f>\n")}**Punishment time:** <t:{casesData.GetInt64(6)}:f>"});

                listTwoIndex++;
        }

        EmbedBuilder casesEmbed = new EmbedBuilder()
            .WithTitle("Cases matching your search")
            .WithColor(GetEmbedColor());

        if (CasesList.ElementAtOrDefault(0) == null)
        {
            casesEmbed = casesEmbed.WithDescription("**There are no cases matching your search.**");
            await RespondAsync(embed: casesEmbed.Build());
            return;
        }
        
        casesEmbed = CasesList[0].Aggregate(casesEmbed, (current, modCase) => current.AddField(modCase[0], modCase[1]));

        await RespondAsync(embed: casesEmbed.Build(), components: CasesList.Count == 1 ? null : new ComponentBuilder().WithButton(_buttonRight).Build());

        ulong messageId = GetOriginalResponseAsync().Result.Id;
        EmbedCounters.Add(messageId, new EmbedCounter());
        EmbedCounters[messageId].InteractionAuthorId = Context.User.Id;
    }

    [ComponentInteraction("ButtonRight")]
    public async Task ButtonRightAsync()
    {
       await DeferAsync(ephemeral: true);
        
        IUserMessage message = await GetOriginalResponseAsync();

        if (EmbedCounters[message.Id].InteractionAuthorId != Context.User.Id)
        {
            await FollowupAsync("You can't use this button, get your own interaction to do so.", ephemeral: true);
            return;
        }

        EmbedBuilder casesEmbed = new EmbedBuilder()
            .WithTitle("Cases matching your search")
            .WithColor(GetEmbedColor());
        
        EmbedCounters[message.Id].Page++;

        casesEmbed = CasesList[EmbedCounters[message.Id].Page].Aggregate(casesEmbed, (current, modCase) => current.AddField(modCase[0], modCase[1]));

        ComponentBuilder newButtons = new ComponentBuilder();
        newButtons = CasesList.ElementAtOrDefault(EmbedCounters[message.Id].Page + 1) != null ? newButtons.WithButton(_buttonLeft).WithButton(_buttonRight) : newButtons.WithButton(_buttonLeft);

        ComponentBuilder newButtonsDisabled = new ComponentBuilder();
        newButtonsDisabled = CasesList.ElementAtOrDefault(EmbedCounters[message.Id].Page + 1) != null ? newButtonsDisabled.WithButton(_buttonLeftDisabled).WithButton(_buttonRightDisabled) : newButtonsDisabled.WithButton(_buttonLeftDisabled);
        
        await message.ModifyAsync(m => m.Components = new Optional<MessageComponent>(newButtonsDisabled.Build()));
        await message.ModifyAsync(m => m.Embed = casesEmbed.Build());
        await message.ModifyAsync(m => m.Components = new Optional<MessageComponent>(newButtons.Build()));
    }

    [ComponentInteraction("ButtonLeft")]
    public async Task ButtonLeftASync()
    {
        await DeferAsync(ephemeral: true);
        
        IUserMessage message = await GetOriginalResponseAsync();

        if (EmbedCounters[message.Id].InteractionAuthorId != Context.User.Id)
        {
            await FollowupAsync("You can't use this button, get your own interaction to do so.", ephemeral: true);
            return;
        }
        
        EmbedBuilder casesEmbed = new EmbedBuilder()
            .WithTitle("Cases matching your search")
            .WithColor(GetEmbedColor());
        
        EmbedCounters[message.Id].Page--;

        casesEmbed = CasesList[EmbedCounters[message.Id].Page].Aggregate(casesEmbed, (current, modCase) => current.AddField(modCase[0], modCase[1]));

        ComponentBuilder newButtons = new ComponentBuilder();
        newButtons = CasesList.ElementAtOrDefault(EmbedCounters[message.Id].Page - 1) != null ? newButtons.WithButton(_buttonLeft).WithButton(_buttonRight) : newButtons.WithButton(_buttonRight);

        ComponentBuilder newButtonsDisabled = new ComponentBuilder();
        newButtonsDisabled = CasesList.ElementAtOrDefault(EmbedCounters[message.Id].Page - 1) != null ? newButtonsDisabled.WithButton(_buttonLeftDisabled).WithButton(_buttonRightDisabled) : newButtonsDisabled.WithButton(_buttonLeftDisabled);

        
        await message.ModifyAsync(m => m.Components = new Optional<MessageComponent>(newButtonsDisabled.Build()));
        await message.ModifyAsync(m => m.Embed = casesEmbed.Build());
        await message.ModifyAsync(m => m.Components = new Optional<MessageComponent>(newButtons.Build()));
    }
}

public class EmbedCounter
{
    public int Page;
    public ulong InteractionAuthorId;
}
