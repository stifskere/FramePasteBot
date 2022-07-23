using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace FPB.handlers;

public class ReactionRolesHandler : InteractionModuleBase<SocketInteractionContext>
{
    [ComponentInteraction("ReactionRolesMenu")]
    public async Task ReactionRolesSelectMenuExecutedAsync(string id, string[] roles)
    {
        string roleID = roles[0];
    }
}

//TODO make reaction roles
public static class ReactionRolesCreator
{
    private static readonly SocketTextChannel ReactionRolesChannel = (SocketTextChannel)Guild.Channels.First(c => c.Id == ulong.Parse(LoadConfig().Channels.ReactionRoles.ToString()));
    public static async Task CreateEmbed()
    {
        SelectMenuBuilder reactionRolesMenu = new SelectMenuBuilder()
            .WithPlaceholder("Select a role")
            .WithCustomId("ReactionRolesMenu");
        string embedString = "";
        foreach (dynamic value in LoadConfig().ReactionRoles)
        {
            reactionRolesMenu = reactionRolesMenu.AddOption(label: value.RoleName.ToString(), description: value.Description.ToString(), value: value.RoleId.ToString());
            embedString += $"🔹 **Select {value.RoleName.ToString()} to get the** <@&{value.RoleId.ToString()}> **role**\n{value.Description.ToString()}\n\n";
        }
        EmbedBuilder reactionRolesEmbed = new EmbedBuilder().WithTitle("Choose some of the following roles from the list").WithDescription($"{embedString}").WithColor(GetEmbedColor());
        IReadOnlyCollection<IMessage> messages = await ReactionRolesChannel.GetMessagesAsync().FirstAsync();
        if (messages.Count is < 1 or > 1)
        {
            await ReactionRolesChannel.DeleteMessagesAsync(messages);
            await ReactionRolesChannel.SendMessageAsync(embed: reactionRolesEmbed.Build(), components: new ComponentBuilder().WithSelectMenu(reactionRolesMenu).Build());
        }
    }
}