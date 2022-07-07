using Discord;
using Discord.Interactions;

namespace FPB.Commands;

public class Eval : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("eval", "Evaluates a c# expression"), DefaultMemberPermissions(GuildPermission.ManageMessages)]
    public async Task EvalAsync(string expresion)
    {
        try
        {
            object result = Z.Expressions.Eval.Execute(expresion);
            
            EmbedBuilder evalEmbed = new EmbedBuilder()
                .WithTitle("Eval")
                .AddField("Input", $"```cs\n{expresion}```")
                .AddField("Output", $"```xl\n{result ?? "Empty output"}```")
                .WithColor(GetEmbedColor(EmbedColors.EmbedGreenColor));

            await RespondAsync(embed: evalEmbed.Build());
        }
        catch(Exception error)
        {
            EmbedBuilder evalEmbed = new EmbedBuilder()
                .WithTitle("Eval")
                .AddField("Input", $"```cs\n{expresion}```")
                .AddField("Error", $"```xl\n{error.Message.Replace("Contact our support team for more information or if you believe it's an error on our part: info@zzzprojects.com.", "")}```")
                .WithColor(GetEmbedColor(EmbedColors.EmbedRedColor));

            await RespondAsync(embed: evalEmbed.Build());
        }
    }
}