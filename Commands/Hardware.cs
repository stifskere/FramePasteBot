using System.Data.SQLite;
using Discord;
using Discord.Interactions;

namespace FPB.Commands;

[Group("hardware", "Hardware group")]
public class Hardware : InteractionModuleBase<SocketInteractionContext>
{
    [Group("cpu", "Hardware cpu group")]
    public class cpus : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("list", "Displays a list from a keyword")]
        public async Task ListAsync(string search)
        {
            await RespondAsync(embed: ListSearchAsync(true, search).Build());
        }

        [SlashCommand("find", "Displays the exact hardware or similar part if found")]
        public async Task FindAsync(string search)
        {
            SQLiteDataReader dataList = DataBase.RunSqliteQueryCommand($"SELECT * FROM CpuList WHERE name LIKE '%{search}%'");
            EmbedBuilder dataEmbed = new EmbedBuilder();
            if (!dataList.HasRows)
            {
                dataEmbed = dataEmbed
                    .WithTitle("No cpu matches your search")
                    .WithTitle("Try again with another keyword")
                    .WithColor(GetEmbedColor(EmbedColors.EmbedRedColor));
            }
            else
            {
                while (dataList.Read())
                {
                    dataEmbed = dataEmbed
                        .WithTitle(dataList.GetString(0))
                        .AddField("Cores", dataList.GetString(3), inline: true)
                        .AddField("Threads", dataList.GetString(4), inline: true)
                        .AddField("Base clock", dataList.GetString(5), inline: true)
                        .AddField("Boost clock", dataList.GetString(6), inline: true)
                        .AddField("Socket", dataList.GetString(7), inline: true)
                        .AddField("TDP", dataList.GetString(8), inline: true)
                        .AddField("Price", dataList.GetString(2) + "$", inline: true)
                        .WithColor(dataList.GetString(1) == "AMD" ? (uint)0xFFA500 : 0x0071c5);
                
                    break;
                }
            }
            await RespondAsync(embed: dataEmbed.Build());
        }
    }

    [Group("gpu", "Hardware gpu group")]
    public class gpus : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("list", "Displays a list from a keyword")]
        public async Task ListAsync(string search)
        {
            await RespondAsync(embed: ListSearchAsync(false, search).Build());
        }

        [SlashCommand("find", "Displays the exact hardware part if found")]
        public async Task FindAsync(string search)
        {
            SQLiteDataReader dataList = DataBase.RunSqliteQueryCommand($"SELECT * FROM GpuList WHERE name LIKE '%{search}%'");
            EmbedBuilder dataEmbed = new EmbedBuilder();
            if (!dataList.HasRows)
            {
                dataEmbed = dataEmbed
                    .WithTitle("No cpu matches your search")
                    .WithTitle("Try again with another keyword")
                    .WithColor(GetEmbedColor(EmbedColors.EmbedRedColor));
            }
            else
            {
                while (dataList.Read())
                {
                    string? imgUrl;
                    try { imgUrl = dataList.GetString(6); }catch { imgUrl = null; }
                    dataEmbed = dataEmbed
                        .WithTitle(dataList.GetString(0))
                        .AddField("Memory interface", dataList.GetString(2), inline: true)
                        .AddField("VRAM", dataList.GetString(3), inline: true)
                        .AddField("Power connectors", dataList.GetString(4), inline: true)
                        .AddField("TDP", dataList.GetString(5), inline: true)
                        .AddField("PCIE Link", dataList.GetString(7), inline: true)
                        .AddField("VRAM type", dataList.GetString(8), inline: true)
                        .AddField("Price", dataList.GetString(1) + "$", inline: true)
                        .WithImageUrl(imgUrl)
                        .WithColor(dataList.GetString(9) == "AMD" ? (uint)0xFFA500 : 0x76b900);
                    break;
                }
            }
            await RespondAsync(embed: dataEmbed.Build());
        }
    }

    private static EmbedBuilder ListSearchAsync(bool gpuOrCpu, string search)
    {
        string query = $"{(gpuOrCpu? "CpuList" : "GpuList")} WHERE name LIKE '%{search}%'";
        SQLiteDataReader dataList = DataBase.RunSqliteQueryCommand("SELECT * FROM " + query);
        string hardwareListString = "";
        int index = 0;
        bool isLimit = false;
        while (dataList.Read())
        {
            if (index == 36)
            {
                isLimit = true;
                break;
            }
            hardwareListString += $"`{index++}. {dataList.GetString(0)}`\n";
        }

        return new EmbedBuilder()
            .WithTitle($"{(gpuOrCpu? "Cpu" : "Gpu")} list based on \"{search}\"")
            .WithDescription(hardwareListString.Length == 0? "Your search didn't match any results" : hardwareListString)
            .WithColor(hardwareListString.Length == 0? GetEmbedColor(EmbedColors.EmbedRedColor) : GetEmbedColor())
            .WithFooter(text : isLimit ? $"This is a list of the first 35 {(gpuOrCpu? "cpus" : "gpus")} matching your search" : null);
    }
}