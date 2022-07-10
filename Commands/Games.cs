using Discord;
// ReSharper disable once RedundantUsingDirective
using Discord.Commands;
using Discord.Interactions;
using FPB.handlers;
using Group = Discord.Interactions.GroupAttribute;

namespace FPB.Commands;

[Group("games", "bot games commands")]
public class Games : InteractionModuleBase<SocketInteractionContext>
{
    //rock paper scissors
    private static readonly Dictionary<ulong, Task> RpsChannel = new();
    private static readonly Dictionary<ulong, IUser> RpsPlayers = new();
    private static readonly Dictionary<ulong, string> RpsMoves = new();
    private static readonly Dictionary<ulong, IUserMessage> RpsMessage = new();
    private static readonly Dictionary<ulong, bool> CancellationRequest = new();

    [SlashCommand("rps", "Rock - Paper - scissors")]
    public async Task RpsAsync([Choice("Rock - It's gray, you can hold it with your hand", "rock"), Choice("Paper - It folds, it's white... it has a unreadable message.", "paper"), Choice("Scissors - That scissors are sharp", "scissors")]string move)
    {
        if (!RpsChannel.ContainsKey(Context.Channel.Id))
        {
            RpsPlayers.Remove(Context.Channel.Id);
            RpsMoves.Remove(Context.Channel.Id);
            RpsMessage.Remove(Context.Channel.Id);
            CancellationRequest.Remove(Context.Channel.Id);
            
            CancellationRequest.Add(Context.Channel.Id, false);
            RpsPlayers.Add(Context.Channel.Id, Context.User);
            RpsMoves.Add(Context.Channel.Id, move);
            EmbedBuilder rpsEmbed = new EmbedBuilder()
                .WithTitle("RPS")
                .WithDescription($"**{Context.User.GetTag()} started a RPS game**\n someone else has to re run the command to play, the interaction will end in 30 seconds.")
                .WithColor(GetEmbedColor());
            var firstMessage = await Context.Channel.SendMessageAsync(embed: rpsEmbed.Build());
            RpsMessage.Add(Context.Channel.Id, firstMessage);
            RpsChannel.Add(Context.Channel.Id, Task.Run(async () => RemoveRpsAsync(await GetOriginalResponseAsync())));
            await DeferAsync();
            await DeleteOriginalResponseAsync();
        }
        else
        {
            if (RpsPlayers[Context.Channel.Id].Id == Context.User.Id)
            {
                await RespondAsync("You can't play alone goblin.", ephemeral: true);
                RpsChannel.Remove(Context.Channel.Id);
                RpsMoves.Remove(Context.Channel.Id);
                RpsMessage.Remove(Context.Channel.Id);
                RpsPlayers.Remove(Context.Channel.Id);
                return;
            }
            CancellationRequest[Context.Channel.Id] = true;

            EmbedBuilder rpsEmbed = new EmbedBuilder()
                .WithTitle("RPS")
                .WithDescription($"Rps game ended, all players submitted\n\n**{Context.User.GetTag()}** - used `{move}`\n**{RpsPlayers[Context.Channel.Id].GetTag()}** - used `{RpsMoves[Context.Channel.Id]}`")
                .WithColor(GetEmbedColor());
            await RpsMessage[Context.Channel.Id].ModifyAsync(m => m.Embed = new Optional<Embed>(rpsEmbed.Build()));
            RpsChannel.Remove(Context.Channel.Id);
            RpsMoves.Remove(Context.Channel.Id);
            RpsMessage.Remove(Context.Channel.Id);
            RpsPlayers.Remove(Context.Channel.Id);
            await DeferAsync();
            await DeleteOriginalResponseAsync();
        }
    }

    private async Task RemoveRpsAsync(IUserMessage message)
    {
        await Task.Delay(30000);
        if(CancellationRequest[message.Channel.Id])
        {
            CancellationRequest.Remove(message.Channel.Id);
            return;
        }
        EmbedBuilder cancelRpsAsync = new EmbedBuilder()
            .WithTitle("Rps cancelled")
            .WithDescription("Nobody wants to play with you.")
            .WithColor(GetEmbedColor(EmbedColors.EmbedRedColor));

        await message.ModifyAsync(m => m.Embed = new Optional<Embed>(cancelRpsAsync.Build()));

        RpsMessage.Remove(message.Channel.Id);
        RpsChannel.Remove(message.Channel.Id);
        RpsMoves.Remove(message.Channel.Id);
        RpsPlayers.Remove(message.Channel.Id);
    }
    
    //Tic tac toe
}

public static class GameMessageEvents
{
    public static Task Event(IMessage message)
    {
        
        return Task.CompletedTask;
    }
}