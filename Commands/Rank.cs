using Discord;
using Discord.Interactions;
using FPB.handlers;

namespace FPB.Commands;

[Group("rank", "Contains all the rank related commands")]
public class Rank : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("view", "View your rank or someone else's rank")]
    public async Task ViewRankAsync(IGuildUser user)
    {
        LevelHandler levelHandlerConstructorGetter = LevelHandlerDictGetter(user);
    }

    [SlashCommand("stats", "View some server rank stats")]
    public async Task ViewRankStatsAsync()
    {
        
    }

    [SlashCommand("leaderboard", "View the server leaderboard")]
    public async Task ViewRankLeaderBoardAsync()
    {
        
    }
}