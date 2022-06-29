using Discord;
using Discord.Interactions;

namespace FPB.Commands;

public class AddBannedWord : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("addbannedword", "Add a banned word in the banned words database"), DefaultMemberPermissions(GuildPermission.Administrator)]
    public async Task AddBannedWordAsync()
    {
        await RespondWithModalAsync<BannedWordsModal>("bwmd");
    }

    [ModalInteraction("bwmd")]
    public async Task modalResponseAsync(BannedWordsModal modal)
    {
        string[] words = modal.words.Split(",");
        List<string> validWords = new();
        List<string> invalidWords = new();

        foreach (string word in words)
        {
            try {DataBase.RunSqliteNonQueryCommand($"INSERT INTO BannedWords(BannedWord) VALUES('{word}')"); validWords.Add(word);}catch{invalidWords.Add(word);}
        }

        await RespondAsync($"{(validWords.Count > 0 ? $"Added these words\n```{validWords.Aggregate("", (current, validWord) => current + $"{(current.Length == 0 ? validWord : $", {validWord}")}")}```\n\n":"")}{(invalidWords.Count > 0 ? $"Didn't add this words because they were invalid or already existed\n```{invalidWords.Aggregate("", (current, invalidWord) => current + $"{(current.Length == 0 ? invalidWord : $", {invalidWord}")}")}```\n\n":"")}", ephemeral: true);
    }
    
    public class BannedWordsModal : IModal
    {
        public string Title { get; } = "Add banned words";

        [InputLabel("Banned words (separate by , no spaces)"), ModalTextInput("wordsInput", maxLength: 50)]
        public string words { get; set; }
    }
}