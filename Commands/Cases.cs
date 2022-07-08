using System.Data.SQLite;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using FPB.handlers;

namespace FPB.Commands;

public class Cases : InteractionModuleBase<SocketInteractionContext>
{
    private ButtonBuilder buttonRight = new ButtonBuilder().WithStyle(ButtonStyle.Primary).WithEmote(new Emoji("➡️")).WithCustomId("ButtonRight").WithLabel("Next");
    private ButtonBuilder buttonLeft = new ButtonBuilder().WithStyle(ButtonStyle.Primary).WithEmote(new Emoji("⬅️")).WithCustomId("ButtonLeft").WithLabel("Back");
    
    private ButtonBuilder buttonRightDisabled = new ButtonBuilder().WithStyle(ButtonStyle.Primary).WithEmote(new Emoji("➡️")).WithCustomId("ButtonRight").WithLabel("Next").WithDisabled(true);
    private ButtonBuilder buttonLeftDisabled = new ButtonBuilder().WithStyle(ButtonStyle.Primary).WithEmote(new Emoji("⬅️")).WithCustomId("ButtonLeft").WithLabel("Back").WithDisabled(true);
    
    private static List<List<string[]>> CasesList = new();
    private static Dictionary<ulong, EmbedCounter> EmbedCounters = new();
    
    [SlashCommand("cases", "View all cases")]
    public async Task ViewCasesAsync(IGuildUser? user = null, [Choice("Warn", "Warn"), Choice("Kick", "Kick"), Choice("Ban", "Ban"), Choice("Time Out", "Mute")]string? type = null)
    {
        SocketGuild guild = Client.Guilds.First(g => g.Id == ulong.Parse(LoadConfig().GuildId.ToString()));
        SQLiteDataReader casesData = DataBase.RunSqliteQueryCommand("SELECT * FROM Cases");
        
        if (user != null && type != null)
        {
            casesData = DataBase.RunSqliteQueryCommand($"SELECT * FROM Cases WHERE UserId = {user.Id} AND Type = '{type}'");
        }
        else if (user != null && type == null)
        {
            casesData = DataBase.RunSqliteQueryCommand($"SELECT * FROM Cases WHERE UserId = {user.Id}");
        }
        else if (user == null && type != null)
        {
            casesData = DataBase.RunSqliteQueryCommand($"SELECT * FROM Cases WHERE Type = '{type}'");
        }
        else if (user == null && type == null)
        {
            casesData = DataBase.RunSqliteQueryCommand($"SELECT * FROM Cases");
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

            IGuildUser caseUser = guild.Users.First(u => u.Id == ulong.Parse(casesData.GetInt64(1).ToString()));
            IGuildUser caseModerator = guild.Users.First(u => u.Id == ulong.Parse(casesData.GetInt64(2).ToString()));

            CasesList[listOneIndex].Add(new [] {$"Case {casesData.GetInt32(0)} | {casesData.GetString(5)}", $"**User:** {caseUser.GetTag()} - `{caseUser.Id}`\n**Moderator:** {caseModerator.GetTag()} - `{caseModerator.Id}`\n**Reason:** {casesData.GetString(3)}\n{(casesData.GetInt32(4) == 0 ? "":$"**Removed punishment at:** <t:{casesData.GetInt32(4)}:f>\n")}**Punishment time:** <t:{casesData.GetInt32(6)}:f>"});
            
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

        await RespondAsync(embed: casesEmbed.Build(), components: CasesList.Count == 1 ? null : new ComponentBuilder().WithButton(buttonRight).Build());

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
        newButtons = CasesList.ElementAtOrDefault(EmbedCounters[message.Id].Page + 1) != null ? newButtons.WithButton(buttonLeft).WithButton(buttonRight) : newButtons.WithButton(buttonLeft);

        ComponentBuilder newButtonsDisabled = new ComponentBuilder();
        newButtonsDisabled = CasesList.ElementAtOrDefault(EmbedCounters[message.Id].Page + 1) != null ? newButtonsDisabled.WithButton(buttonLeftDisabled).WithButton(buttonRightDisabled) : newButtonsDisabled.WithButton(buttonLeftDisabled);
        
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
        newButtons = CasesList.ElementAtOrDefault(EmbedCounters[message.Id].Page - 1) != null ? newButtons.WithButton(buttonLeft).WithButton(buttonRight) : newButtons.WithButton(buttonRight);

        ComponentBuilder newButtonsDisabled = new ComponentBuilder();
        newButtonsDisabled = CasesList.ElementAtOrDefault(EmbedCounters[message.Id].Page - 1) != null ? newButtonsDisabled.WithButton(buttonLeftDisabled).WithButton(buttonRightDisabled) : newButtonsDisabled.WithButton(buttonLeftDisabled);

        
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
