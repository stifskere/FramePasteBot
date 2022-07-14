using System.ComponentModel.DataAnnotations;
using System.Data.SQLite;
using Discord;
using Discord.Interactions;
using Newtonsoft.Json;

namespace FPB.Commands;

[Group("specs", "All commands related to own specs")]
public class Specs : InteractionModuleBase<SocketInteractionContext>
{
    private Dictionary<string, string>? DataBaseReader(IUser user)
    {
        SQLiteDataReader content = DataBase.RunSqliteQueryCommand($"SELECT * FROM Specs WHERE userId = {user.Id}");
        string readContent = "{}";
        while (content.Read()) readContent = content.GetString(1);
        return JsonConvert.DeserializeObject<Dictionary<string, string>>(readContent);
    }

    private void DataBaseSetter(Dictionary<string, string> dict, IUser user)
    {
        SQLiteDataReader check = DataBase.RunSqliteQueryCommand($"SELECT * FROM Specs WHERE userId = {user.Id}");
        DataBase.RunSqliteNonQueryCommand(check.HasRows == false
            ? $"INSERT INTO Specs(userId, list) VALUES({user.Id}, '{JsonConvert.SerializeObject(dict).Replace("'", "")}')"
            : $"UPDATE Specs SET list = '{JsonConvert.SerializeObject(dict).Replace("'", "")}' WHERE userId = {user.Id}");
    }
    
    [SlashCommand("info", "Info about specs commands")]
    public async Task InfoAsync()
    {
        EmbedBuilder infoEmbed = new EmbedBuilder()
            .WithTitle("Specs command")
            .WithDescription("The specs command group has a basic usage, it is meant to store your specs in lists and showcase it to others, the following subcommands are available\n")
            .AddField("🔹 View", "This command lets you view your or other user specs\n**Params:**\n**User** (optional): The user whose specs you want to see\n")
            .AddField("🔹 Add", "This command lets you add specs to your own list\n**Params:**\n**Key:** This is the title of the spec you want to add\n**Value:** This is the brand name and model of the hardware you are adding to your list\n")
            .AddField("🔹 Remove", "This command lets you remove some key in your specs list\n**Params:**\n**Key:** This is the key or title of the saved spec you want to remove\n")
            .AddField("🔹 Edit", "This command lets you edit some key in your specs list\n**Params:**\n**Key:** This is the key or title of the saved spec you want to edit\n**Value:** This is the new value you want to replace your spec for\n")
            .WithColor(GetEmbedColor());

        await RespondAsync(embed: infoEmbed.Build());
    }

    [SlashCommand("view", "View your specs or from someone else")]
    public async Task ViewAsync([Summary("user", "user whose specs")]IUser? user = null)
    {
        user ??= Context.User;
        Dictionary<string, string>? partsList = DataBaseReader(user);
        EmbedBuilder specsEmbed = new EmbedBuilder()
            .WithTitle($"{user.Username}'s specs")
            .WithColor(GetEmbedColor());
        if (partsList == null || partsList.Count == 0)
        {
            specsEmbed = specsEmbed.WithDescription("No specs list found for this user");
            await RespondAsync(embed: specsEmbed.Build());
            return;
        }
        string embedDesc = partsList.Aggregate("", (current, entry) => current + $"**{entry.Key}**\n{entry.Value}\n\n");
        specsEmbed = specsEmbed.WithDescription(embedDesc);
        await RespondAsync(embed: specsEmbed.Build());
    }

    [SlashCommand("add", "Add a spec to your list")]
    public async Task AddAsync([MaxLength(20)]string key, string value)
    {
        key = key.Replace("'", "");
        value = value.Replace("'", "");
        Dictionary<string, string> specsRead = DataBaseReader(Context.User) ?? new Dictionary<string, string>();
        specsRead.Add(key, value);
        DataBaseSetter(specsRead, Context.User);
        EmbedBuilder addEmbed = new EmbedBuilder()
            .WithTitle("Specs added")
            .WithDescription($"The following specs were added to your personal list\n\n**{key}**\n{value}")
            .WithColor(GetEmbedColor());
        await RespondAsync(embed: addEmbed.Build());
    }

    [SlashCommand("remove", "Remove specs from your list")]
    public async Task RemoveAsync([MaxLength(20)] string key)
    {
        key = key.Replace("'", "");
        Dictionary<string, string> specsRead = DataBaseReader(Context.User) ?? new Dictionary<string, string>();

        EmbedBuilder removeEmbed = new EmbedBuilder()
            .WithTitle("Specs removal")
            .WithColor(GetEmbedColor());
        
        if (specsRead.Count == 0)
        {
            removeEmbed = removeEmbed
                .WithDescription("No specs were found, start your list with `/specs add`")
                .WithColor(GetEmbedColor(EmbedColors.EmbedRedColor));
            await RespondAsync(embed: removeEmbed.Build());
            return;
        }

        if (!specsRead.ContainsKey(key))
        {
            int minDistance = 3;
            string closestKey = "";
            foreach (string dictKey in specsRead.Keys)
            {
                int distance = StringDistance(key, dictKey);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestKey = dictKey;
                }
            }

            removeEmbed = removeEmbed.WithColor(GetEmbedColor(EmbedColors.EmbedRedColor));

            removeEmbed = removeEmbed.WithDescription(closestKey == "" 
                ? "**No key found with this name**\n\ntry searching with other words" 
                : $"**No key found with this name**\n\n**did you mean:** {closestKey}?")
                .WithFooter(text: "remember: KeY Is CaSe SeNsItIvE");

            await RespondAsync(embed: removeEmbed.Build());
            return;
        }

        removeEmbed = removeEmbed
            .WithDescription($"**The following specs were removed**\n\n**{key}**\n{specsRead[key]}");

        specsRead.Remove(key);

        DataBaseSetter(specsRead, Context.User);

        await RespondAsync(embed: removeEmbed.Build());
    }

    [SlashCommand("edit", "Edit specs from your list")]
    public async Task EditAsync([MaxLength(20)]string key, [Summary("new-value")]string value)
    {
        key = key.Replace("'", "");
        value = value.Replace("'", "");
        Dictionary<string, string> specsRead = DataBaseReader(Context.User) ?? new Dictionary<string, string>();

        EmbedBuilder editEmbed = new EmbedBuilder()
            .WithTitle("Specs editing")
            .WithColor(GetEmbedColor());

        if (specsRead.Count == 0)
        {
            editEmbed = editEmbed
                .WithDescription("No specs were found, start your list with `/specs add`")
                .WithColor(GetEmbedColor(EmbedColors.EmbedRedColor));
            await RespondAsync(embed: editEmbed.Build());
            return;
        }
        
        if (!specsRead.ContainsKey(key))
        {
            int minDistance = 3;
            string closestKey = "";
            foreach (string dictKey in specsRead.Keys)
            {
                int distance = StringDistance(key, dictKey);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestKey = dictKey;
                }
            }

            editEmbed = editEmbed.WithColor(GetEmbedColor(EmbedColors.EmbedRedColor));

            editEmbed = editEmbed.WithDescription(closestKey == "" 
                    ? "**No key found with this name**\n\ntry searching with other words" 
                    : $"**No key found with this name**\n\n**did you mean:** {closestKey}?")
                .WithFooter(text: "remember: KeY Is CaSe SeNsItIvE");

            await RespondAsync(embed: editEmbed.Build());
            return;
        }

        editEmbed = editEmbed
            .WithDescription($"**Changed {key} value**")
            .AddField("Before", specsRead[key])
            .AddField("After", key);

        await RespondAsync(embed: editEmbed.Build());

        specsRead[key] = value;
        
        DataBase.RunSqliteNonQueryCommand($"UPDATE Specs SET list = '{specsRead}' WHERE userId = {Context.User.Id}");
    }
}