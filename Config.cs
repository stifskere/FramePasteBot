using System.Text;
using FPB.handlers;
using Newtonsoft.Json;

namespace FPB;

public static class Config
{
    public static dynamic LoadConfig()
    {
        if (!File.Exists("Config.json"))
        {
            using FileStream configFile = File.Create("Config.json");
            byte[] configFill = new UTF8Encoding(true).GetBytes("{\n\"Token\": \"\",\n\"GuildId\": 0,\n\"YeesterId\": 0,\n  \"Channels\": {\n\"Logs\": 0,\n\"Case\": 0,\n\"ModMail\": 0\n }\n}");
            configFile.Write(configFill, 0, configFill.Length);
            
            Console.Clear();
            Console.WriteLine("Please fill the config before turning the bot on again.\n\nIf you don't fill the config correctly you can expect errors.");
            Console.ReadLine();
            Environment.Exit(0);
        }
        using StreamReader file = new StreamReader("Config.json");
        return JsonConvert.DeserializeObject(file.ReadToEnd())!;
    }

    public static readonly DataBaseHandler DataBase = new DataBaseHandler($"./Databases/{LoadConfig().GuildId.ToString()}.db");
    public static BanManager? BanHandler;
    public static Dictionary<string, int> CommandUses = new();
    public static ulong UpTime = 0;
}