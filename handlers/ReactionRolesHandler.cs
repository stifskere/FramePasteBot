using Discord.Interactions;
using Discord.WebSocket;

namespace FPB.handlers;

public class ReactionRolesHandler : InteractionModuleBase<SocketInteractionContext>
{
    
}

//TODO make reaction roles
public static class ReactionRolesCreator
{
    private static SocketTextChannel ReactionRolesChannel = (SocketTextChannel)Guild.Channels.First(c => c.Id == ulong.Parse(LoadConfig().ReactionRoles.ToString()));
    
}