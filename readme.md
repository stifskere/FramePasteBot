# Framepaste Bot

A discord bot recoding in c# for the original framepaste bot made in JS

### Downloading the bot

to download the bot you can download the code and [compile](https://docs.microsoft.com/en-us/troubleshoot/developer/visualstudio/csharp/language-compilers/compile-code-using-compiler) it by yourself

or go to the releases page and download a pre released version of the bot.

### Downloading the code

there are some packages used in this bot, some of them are private but you can ask Memw#6969 for them in the [framepaste](https://www.discord.gg/WbCcVYpebY) server.

the following packages are needed to edit the bot:

| Package name       | Description                                                          |
|--------------------|----------------------------------------------------------------------|
| Discord.net        | The discord api wraper used for the bot.                             |
| ColorCat (private) | A color converting class library made by my friend SenyorGato#7759   |
| System.Data.Sqlite | The database utility package used to write / read databases.         |
| Z.Expresions.Eval  | Some eval utility, there is no much use for it so it may be removed. |

### Usage

after the bot is compiled/downloaded

If the bot is ran for the first time, it will create a config file, there isn't much config to do more than assign channels
but it is needed for the bot to work.

after you fill the config you can just leave the bot running, if there is any problem with it just ask Memw#6969 trough the [framepaste](https://www.discord.gg/WbCcVYpebY) discord server.

### Config

here is a description of what every config value does.

| Value           | Type   | Description                                                                                                                           |
|-----------------|--------|---------------------------------------------------------------------------------------------------------------------------------------|
| Token           | String | This is the token of the bot you can get in https://dev.discord.com                                                                   |
| Guild ID        | Ulong  | The guild id the bot will be running on.                                                                                              |
| Yeester ID      | Ulong  | This is the ID that mrYeester has, you can set it to 0 if you don't want to have any "yeester counter" (the comand won't disable tho) |
| Channels.Logs   | Ulong  | This is the logs channel.                                                                                                             |
| Channels.Case   | Ulong  | This is the cases channel, can be set to the same as logs and it will send the cases in the logs channel.                             |
| Channel.ModMail | Ulong  | This is where all the ModMails will be received.                                                                                      |



