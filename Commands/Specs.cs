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
        EmbedBuilder infoEmbed = new EmbedBuilder();
        await RespondAsync(text: "xd");
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
        Dictionary<string, string> specsRead = DataBaseReader(Context.User) ?? new Dictionary<string, string>();

        EmbedBuilder removeEmbed = new EmbedBuilder()
            .WithTitle("Specs removal")
            .WithColor(GetEmbedColor());

        if (!specsRead.ContainsKey(key))
        {
            int minDistance = 3;
            string closestKey = "";
            foreach (string dictKey in specsRead.Keys)
            {
                int distance = StringDistance(key, dictKey);
                Console.WriteLine(distance);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestKey = dictKey;
                }
            }

            removeEmbed = removeEmbed.WithColor(GetEmbedColor(EmbedColors.EmbedRedColor));

            removeEmbed = removeEmbed.WithDescription(closestKey == "" 
                ? "**No key found with this name**\n\ntry searching with other words\n remember: SeArCh Is CaSe SeNsItIvE" 
                : $"**No key found with this name**\n\ndid you mean {closestKey}?\n remember: SeArCh Is CaSe SeNsItIvE");

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
        
    }
}