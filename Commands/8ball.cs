using Discord;
using Discord.Interactions;

namespace FPB.Commands;

public class EightBall : InteractionModuleBase<SocketInteractionContext> 
{
    
    [SlashCommand("8ball", "You ask a question and it will answer his thoughts")]
    public async Task EightBallAsync([Summary("question", "What do you want to ask this saviour?")]string question)
    {
        string[] answers = { "Without a doubt. Nah, I’m just messing with you.", "My sources say no.", "They also tell me they hate you.", "Yes, definitely. Unless...", "As If", "Ask Me If I Care", "Dumb Question Ask Another", "Forget About It", "In Your Dreams", "Not A Chance", "Obviously", "Oh Please", "Sure", "That\'s Ridiculous", "Well Maybe", "What Do You Think?", "Who Cares?", "Yeah Right", "You Wish", "You\'ve Got To Be Kidding...", "Yes", "It is certain", "It is decidedly so", "Without a doubt", "Yes definitely", "You may rely on it", "As I see it, yes", "Most likely", "Outlook good", "Signs point to yes", "Reply hazy try again", "Ask again later", "Better not tell you now", "Cannot predict now", "Concentrate and ask again", "Don\'t count on it", "My reply is no", "My sources say no", "Outlook not so good", "Very doubtful", "my dad said I cant answer that, try again later", "*yawn*"};

        EmbedBuilder eightBallEmbed = new EmbedBuilder()
            .WithTitle("8ball")
            .WithDescription($"> {question}\n\n**{(question.Length < 4 ? "Ask a real question you dumbass" : answers[CustomMethods.Random.Next(answers.Length)])}**");

        await RespondAsync(embed: eightBallEmbed.Build());
    }
}