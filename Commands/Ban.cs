using Discord;
using Discord.Interactions;

namespace FPB.Commands;

public class Ban : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ban", "Bans a user from the guild"), DefaultMemberPermissions(GuildPermission.BanMembers)]
    public async Task BanAsync(IGuildUser user, string reason = "A reason was not defined", string? time = null)
    {
        string bannedTime = "undefined time";
        int banTime = 0;
        
        if (time != null)
        {
            switch (time.ToLower().Last())
            {
                case 'm':
                    banTime = (int)TimeSpan.FromMinutes(int.Parse(time.ToLower().Replace("m", ""))).TotalMilliseconds;
                    bannedTime = $"{TimeSpan.FromMilliseconds(banTime).Minutes} minutes";
                    break;
                case 'h':
                    banTime = (int)TimeSpan.FromHours(int.Parse(time.ToLower().Replace("h", ""))).TotalMilliseconds;
                    bannedTime = $"{TimeSpan.FromMilliseconds(banTime).Hours} hours";
                    break;
                case 'd':
                    banTime = (int)TimeSpan.FromDays(int.Parse(time.ToLower().Replace("d", ""))).TotalMilliseconds;
                    bannedTime = $"{TimeSpan.FromMilliseconds(banTime).Days} days";
                    break;
                case 'w':
                    banTime = (int)TimeSpan.FromDays(int.Parse(time.ToLower().Replace("d", "")) * 7).TotalMilliseconds;
                    bannedTime = $"{TimeSpan.FromMilliseconds(banTime).Days * 7} weeks";
                    break;
                default:
                    await RespondAsync("Invalid timespan, you must use \"M\" minutes, \"H\" hours, \"d\" days, \"W\" weeks, \"none\" undefined time", ephemeral: true);
                    return;
            }
        }
        
        await user.BanAsync(reason: reason);

        EmbedBuilder banEmbed = new EmbedBuilder()
            .WithTitle($"{user.DisplayName}#{user.Discriminator} was banned")
            .WithDescription($"**:The user was banned for:** {bannedTime}\n**with the reason:** {reason}")
            .WithColor(GetEmbedColor());

        //gives 16, i'l change later.
        DataBase.RunSqliteNonQueryCommand($"INSERT INTO Cases(UserId, ModeratorId, Reason, Time) VALUES({user.Id}, {Context.User.Id}, '{reason}', {(banTime == 0 ? null : TimeSpan.FromMilliseconds(DateTimeOffset.Now.ToUnixTimeMilliseconds() + banTime).Seconds)})");
        
        await RespondAsync(embed: banEmbed.Build());
    }
}