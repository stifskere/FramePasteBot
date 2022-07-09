using System.Data.SQLite;
using Discord;
using Discord.Interactions;
using FPB.handlers;

namespace FPB.Commands;

[Group("misc", "Misc commands")]
public class Misc : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("8ball", "You ask a question and it will answer his thoughts")]
    public async Task EightBallAsync([Summary("question", "What do you want to ask this saviour?")]string question)
    {
        string[] answers = { "Without a doubt. Nah, I’m just messing with you.", "My sources say no.", "They also tell me they hate you.", "Yes, definitely. Unless...", "As If", "Ask Me If I Care", "Dumb Question Ask Another", "Forget About It", "In Your Dreams", "Not A Chance", "Obviously", "Oh Please", "Sure", "That\'s Ridiculous", "Well Maybe", "What Do You Think?", "Who Cares?", "Yeah Right", "You Wish", "You\'ve Got To Be Kidding...", "Yes", "It is certain", "It is decidedly so", "Without a doubt", "Yes definitely", "You may rely on it", "As I see it, yes", "Most likely", "Outlook good", "Signs point to yes", "Reply hazy try again", "Ask again later", "Better not tell you now", "Cannot predict now", "Concentrate and ask again", "Don\'t count on it", "My reply is no", "My sources say no", "Outlook not so good", "Very doubtful", "my dad said I cant answer that, try again later", "*yawn*"};

        EmbedBuilder eightBallEmbed = new EmbedBuilder()
            .WithTitle("8ball")
            .WithDescription($"> {question}\n\n**{(question.Length < 4 ? "Ask a real question you dumbass" : answers[CustomMethods.Random.Next(answers.Length)])}**")
            .WithColor(GetEmbedColor());

        await RespondAsync(embed: eightBallEmbed.Build());
    }

    [SlashCommand("band", "Honorary ban")]
    public async Task BandAsync([Summary("User", "Who will get honorary banned")]IUser user)
    {
        await RespondAsync("k", ephemeral: true);
        await Context.Channel.SendMessageAsync($"<@{user.Id}> has received an honorary ban!");
    }

    [SlashCommand("unband", "Remove honorary ban")]
    public async Task UnBandAsync([Summary("User", "The lucky person that's gonna get unbanned")]IUser user)
    {
        await RespondAsync("k", ephemeral: true);
        await Context.Channel.SendMessageAsync($"<@{user.Id}> had their honorary ban revoked!");
    }
    
    [SlashCommand("yeestercounter", "View the last time timestamp Mr.Yeester sent a message")]
    public async Task YessterCounterAsync()
    {
        SQLiteDataReader counter = DataBase.RunSqliteQueryCommand("SELECT value FROM Configuration WHERE key = 'YeesterCounter'");
        long timeStamp = 0;
        while (counter.Read()) timeStamp = counter.GetInt64(0);
        EmbedBuilder counterEmbed = new EmbedBuilder()
            .WithTitle("Mr.Yeester counter")
            .WithDescription($"The last time yeester talked was <t:{timeStamp}:R>")
            .WithColor(GetEmbedColor());

        await RespondAsync(embed: counterEmbed.Build());
    }

    [SlashCommand("sus", "Tells if you are enough sus to be sus.")]
    public async Task SusAsync()
    {
        if (CustomMethods.Random.NextDouble() < 0.2f)
        {
            await RespondAsync($"{Context.User} is {(CustomMethods.Random.NextDouble() < 0.2f ? "is a :b:ingus" : "is sus!")}");
        }
        else
        {
            await DeferAsync();
            await DeleteOriginalResponseAsync();
        }
    }

    [SlashCommand("say", "You are the bot.")]
    public async Task SayAsync([Summary("wisdom", "What are your thoughts?")]string text)
    {
        await DeferAsync();
        await DeleteOriginalResponseAsync();
        await Context.Channel.SendMessageAsync(text);
    }

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
            CancellationRequest.Add(Context.Channel.Id, false);
            RpsPlayers.Add(Context.Channel.Id, Context.User);
            RpsMoves.Add(Context.Channel.Id, move);
            EmbedBuilder rpsEmbed = new EmbedBuilder()
                .WithTitle("RPS")
                .WithDescription("Rps game started, someone else has to re run the command to play, this will end in 30 seconds.")
                .WithColor(GetEmbedColor());
            await RespondAsync(embed: rpsEmbed.Build());
            RpsMessage.Add(Context.Channel.Id, await GetOriginalResponseAsync());
            RpsChannel.Add(Context.Channel.Id, Task.Run(async () => RemoveRpsAsync(await GetOriginalResponseAsync())));
        }
        else
        {
            if (RpsPlayers[Context.Channel.Id].Id == Context.User.Id)
            {
                await RespondAsync("You can't play alone goblin.");
                return;
            }
            CancellationRequest[Context.Channel.Id] = true;
            RpsChannel.Remove(Context.Channel.Id);
            EmbedBuilder rpsEmbed = new EmbedBuilder()
                .WithTitle("RPS")
                .WithDescription($"Rps game ended, all players submitted\n\n**{Context.User.GetTag()}** - used `{move}`\n**{RpsPlayers[Context.Channel.Id].GetTag()}** - used `{RpsMoves[Context.Channel.Id]}`")
                .WithColor(GetEmbedColor());
            await RpsMessage[Context.Channel.Id].ModifyAsync(m => m.Embed = new Optional<Embed>(rpsEmbed.Build()));
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
}