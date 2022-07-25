using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using static FPB.handlers.ReactionRolesCreator;

namespace FPB.handlers;

public class ReactionRolesHandler : InteractionModuleBase<SocketInteractionContext>
{
    [ComponentInteraction("ReactionRolesMenu")]
    public async Task ReactionRolesSelectMenuExecutedAsync(string id, string[] r)
    {
        ulong roleId = ulong.Parse(r[0]);
        IReadOnlyCollection<ulong> roles = ((IGuildUser) Context.User).RoleIds;
        if (!roles.Contains(roleId))
        {
            await ((IGuildUser) Context.User).AddRoleAsync(roleId: roleId);
            await RespondAsync(text: $"New role added: <@&{roleId.ToString()}>, to remove it simply press again the same button.", ephemeral: true);
        }
        else
        {
            await ((IGuildUser) Context.User).RemoveRoleAsync(roleId: roleId);
            await RespondAsync(text: $"Role removed: <@&{roleId.ToString()}>, to get the role again press the same button.", ephemeral: true);
        }
        await ReactionRolesOrignalMessage!.ModifyAsync(m => m.Components = new Optional<MessageComponent>(ReactionRolesComponent.Build()));
    }
}

public static class ReactionRolesCreator
{
    public static IUserMessage? ReactionRolesOrignalMessage;
    public static ComponentBuilder ReactionRolesComponent = new();
    private static readonly SocketTextChannel ReactionRolesChannel = (SocketTextChannel)Guild.Channels.First(c => c.Id == ulong.Parse(LoadConfig().Channels.ReactionRoles.ToString()));
    public static async Task CreateEmbed()
    {
        SelectMenuBuilder reactionRolesMenu = new SelectMenuBuilder()
            .WithPlaceholder("Select a role")
            .WithCustomId("ReactionRolesMenu");
        string embedString = "";
        foreach (dynamic value in LoadConfig().ReactionRoles)
        {
            reactionRolesMenu = reactionRolesMenu.AddOption(label: value.RoleName.ToString(), description: value.MenuDescription.ToString(), value: value.RoleId.ToString());
            embedString += $"🔹 **Select {value.RoleName.ToString()} to get the** <@&{value.RoleId.ToString()}> **role**\n{value.Description.ToString()}\n\n";
        }

        ReactionRolesComponent = new ComponentBuilder().WithSelectMenu(reactionRolesMenu);
        EmbedBuilder reactionRolesEmbed = new EmbedBuilder().WithTitle("Choose some of the following roles from the list").WithDescription($"{embedString}").WithColor(GetEmbedColor());
        IReadOnlyCollection<IMessage> messages = await ReactionRolesChannel.GetMessagesAsync().FirstAsync();
        if(messages.Count == 1 && messages.First().Author.Id == Client.CurrentUser.Id)
        {
            ReactionRolesOrignalMessage = (IUserMessage) messages.First();
        }
        else
        {
            await ReactionRolesChannel.DeleteMessagesAsync(messages);
            ReactionRolesOrignalMessage = await ReactionRolesChannel.SendMessageAsync(embed: reactionRolesEmbed.Build(), components: ReactionRolesComponent.Build());
        }
    }
}