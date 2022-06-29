using System.Net;
using Discord;
using Discord.Interactions;

namespace FPB.Commands;

[Group("info", "Info commands")]
public class Info : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("80plus", "Info card about 80plus psu ratings")]
    public async Task EightyPlusAsync()
    {
        await RespondAsync(text: "https://www.guru3d.com/index.php?ct=articles&action=file&id=10061", allowedMentions: new AllowedMentions(allowedTypes: null));
    }
}