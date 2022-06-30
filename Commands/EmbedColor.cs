using Discord;
using Discord.Interactions;

namespace FPB.Commands;

public class EmbedColor : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("embedcolor", "Change the embed color as an rgb value"), DefaultMemberPermissions(GuildPermission.ManageGuild)]
    public async Task EmbedColorAsync([Summary("Red", "Red value for the embed color"), MaxValue(255)]int R, [Summary("Green", "Green value for the embed color"), MaxValue(255)]int G, [Summary("Blue", "Blue value for the embed color"), MaxValue(255)]int B)
    {
        DataBase.RunSqliteNonQueryCommand($"UPDATE Configuration SET value = '{ColorCat.Colors.Hex.FromRGB(R, G, B)}' WHERE Key = 'EmbedColor'");

        EmbedBuilder colorUpdated = new EmbedBuilder()
            .WithTitle("Embed color updated")
            .WithDescription($"The new color is {((dynamic)await HttpRequest($"https://www.thecolorapi.com/id?hex={ColorCat.Colors.Hex.FromRGB(R, G, B)}&format=json")).name.value.ToString()}")
            .WithColor(GetEmbedColor());

        await RespondAsync(embed: colorUpdated.Build());
    }
}