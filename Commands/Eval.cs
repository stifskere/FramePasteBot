using Discord;
using Discord.Interactions;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace FPB.Commands;

public class Eval : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("eval", "Evaluates a c# expression"), DefaultMemberPermissions(GuildPermission.ManageMessages)]
    public async Task EvalAsync([Summary("expression", "The expression to eval")]string expression, [Summary("namespaces", "Add a namespace ended with ;")]string? namespaces = null)
    {
        try
        {
            List<string> namespacesToAdd = new(){"Discord.Net.Core"};

            if (namespaces != null)
            {
                string[] gettedNamespaces = namespaces.Split(";");
                namespacesToAdd.AddRange(from nmspace in gettedNamespaces where !string.IsNullOrEmpty(nmspace) select nmspace.Replace(" ", ""));
            }

            object? result = await CSharpScript.EvaluateAsync(expression, globals: this, options: ScriptOptions.Default.WithReferences(namespacesToAdd));
            
            EmbedBuilder evalEmbed = new EmbedBuilder()
                .WithTitle("Eval")
                .AddField("Input", $"```cs\n{expression}```")
                .AddField("Output", $"```xl\n{result ?? "Empty output"}```")
                .WithColor(GetEmbedColor(EmbedColors.EmbedGreenColor));

            await RespondAsync(embed: evalEmbed.Build());
        }
        catch(Exception error)
        {
            EmbedBuilder evalEmbed = new EmbedBuilder()
                .WithTitle("Eval")
                .AddField("Input", $"```cs\n{expression}```")
                .AddField("Error", $"```xl\n{error.Message}```")
                .WithColor(GetEmbedColor(EmbedColors.EmbedRedColor));

            await RespondAsync(embed: evalEmbed.Build());
        }
    }
}