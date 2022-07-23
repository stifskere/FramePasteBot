using System.ComponentModel.DataAnnotations;
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
        if (ulong.Parse(LoadConfig().YeesterId.ToString()) == 0)
        {
            await RespondAsync("This command is disabled.", ephemeral: true);
            return;
        }
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

    [SlashCommand("google", "Search something on google")]
    public async Task SearchOnGoogleAsync([Summary("Search", "What you want to search on google"), MaxLength(100)]string query)
    {
        EmbedBuilder searchEmbed = new EmbedBuilder()
            .AddField($"Results for {query}", $"(click here to see your search results)[https://www.google.com/search?q={query.Replace(" ", "+")}]")
            .WithColor(GetEmbedColor());

        await RespondAsync(embed: searchEmbed.Build());
    }
}