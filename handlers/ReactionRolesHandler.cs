using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace FPB.handlers;

public class ReactionRolesHandler : InteractionModuleBase<SocketInteractionContext>
{
    
}

//TODO make reaction roles
public static class ReactionRolesCreator
{
    private static readonly SocketTextChannel ReactionRolesChannel = (SocketTextChannel)Guild.Channels.First(c => c.Id == ulong.Parse(LoadConfig().ReactionRoles.ToString()));
    private static readonly Dictionary<ulong, SocketRole> RolesDict = new();
    private static readonly Dictionary<ulong, string> RolesDescDict = new();

    public static void CreateEmbed()
    {
        foreach (dynamic role in LoadConfig().ReactionRoles) RolesDict.Add(role.RoleName.ToString() , Guild.Roles.First(r => r.Id == ulong.Parse(role.RoleId.ToString())));

        SelectMenuBuilder reactionRolesMenu = new SelectMenuBuilder()
            .WithPlaceholder("Select a role");
        reactionRolesMenu = RolesDict.Aggregate(reactionRolesMenu, (current, entry) => current.AddOption(entry.Value.Name, entry.Value.Id.ToString()));

        
        
        ComponentBuilder reactionRolesComponent = new ComponentBuilder();
    }
}