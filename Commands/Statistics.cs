using System.Text.RegularExpressions;
using Discord;
using Discord.Interactions;

namespace FPB.Commands;

public class Statistics : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("statistics", "Shows bot statistics like used commands and uptime")]
    public async Task StatisticsAsync()
    {
        string commandCount = "Command Name    | Count\n----------------------\n";
        int totalCommandUses = 0;
        IOrderedEnumerable<KeyValuePair<string, int>> sortedDict = from entry in CommandUses orderby entry.Value descending select entry;
        foreach (KeyValuePair<string, int> entry in sortedDict)
        {
            totalCommandUses += entry.Value;
            string countAddition = "......................";
            countAddition = $"{Regex.Replace(entry.Key, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0")}{countAddition.Substring(entry.Key.Length)}";
            countAddition = $"{countAddition.Substring(0, countAddition.Length-entry.Value.ToString().Length)}{entry.Value.ToString()}";
            commandCount += $"{countAddition}\n";
        }

        EmbedBuilder statisticsEmbed = new EmbedBuilder()
            .WithTitle("Statistics: Command Usage")
            .WithDescription($"List of commands that have been used in this session, ordered by amount of uses\n**Total amount of commands used in this session:** {totalCommandUses}\n**Total Up time:** {TimeSpan.FromSeconds(UpTime).ToString(@"dd\:hh\:mm\:ss")}\n\n```{commandCount}```")
            .WithColor(GetEmbedColor());

        await RespondAsync(embed: statisticsEmbed.Build());
    }
}