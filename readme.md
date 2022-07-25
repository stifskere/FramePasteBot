![Sin título-1](https://user-images.githubusercontent.com/79871802/179412198-91f21e85-6bfe-4cd1-85d7-e29928c1be3d.png)


A discord bot recoding in c# for the original FramePaste bot made in JS

### Downloading the bot

to download the bot you can download the code and [compile](https://docs.microsoft.com/en-us/troubleshoot/developer/visualstudio/csharp/language-compilers/compile-code-using-compiler) it by yourself

or go to the releases page and download a pre released version of the bot.

### Downloading the code

there are some packages used in this bot, some of them are private but you can find them in a folder inside the bot files preaplied to the project.

the following packages are needed to edit the bot:

| Package name       | Description                                                          |
|--------------------|----------------------------------------------------------------------|
| Discord.net        | The discord api wrapper used for the bot.                            |
| ColorCat (private) | A color converting class library made by my friend SenyorGato#7759   |
| System.Data.Sqlite | The database utility package used to write / read databases.         |
| Z.Expressions.Eval | Some eval utility, there is no much use for it so it may be removed. |

### Usage

after the bot is compiled/downloaded

If the bot is ran for the first time, it will create a config file, there isn't much config to do more than assign channels
but it is needed for the bot to work.

after you fill the config you can just leave the bot running, if there is any problem with it just ask Memw#6969 trough the [framepaste](https://www.discord.gg/WbCcVYpebY) discord server.

### Config

here is a description of what every config value does.

Values are displayed dynamically typed

| Value                 | Type   | Description                                                                                                                                   |
|-----------------------|--------|-----------------------------------------------------------------------------------------------------------------------------------------------|
| Token                 | String | This is the token of the bot you can get in https://dev.discord.com                                                                           |
| GuildId               | Ulong  | The guild id the bot will be running on.                                                                                                      |
| YeesterId             | Ulong  | This is the ID that MrYeester has, you can set it to 0 if you don't want to have any "Yeester counter" (if set to 0 the command will disable) |
| Channels.Logs         | Ulong  | This is the logs channel.                                                                                                                     |
| Channels.Case         | Ulong  | This is the cases channel, can be set to the same as logs and it will send the cases in the logs channel.                                     |
| Channel.ModMail       | Ulong  | This is where all the ModMails will be received.                                                                                              |
| Channel.BotCommands   | Ulong  | This channel is where the bot commands go                                                                                                     |
| Channel.ReactionRoles | Ulong  | This is the ReactionRoles channel where the message and reactions will be created.                                                            |
| Roles.Mod             | Ulong  | The server moderator role.                                                                                                                    |

---

For the reaction roles it is a simple array with objects inside that contain 2 values

| Value           | Type   | Description                                                  |
|-----------------|--------|--------------------------------------------------------------|
| RoleId          | Ulong  | The role ID used to fetch the role.                          |
| RoleName        | String | The role name shown in the list selection title              |
| Description     | String | The role description shown in the embed                      |
| MenuDescription | String | The role description shown in the list selection description |


it should look something like this

```json
{
  "ReactionRoles": [{"RoleId": 0, "RoleName": "Role name", "Description": "Some nice description", "MenuDescription" : "Some nice description"}]
}
```

---

In the config there is also some level roles, since this is a port from another bot the level roles are well, pre made roles that have to be assigned.

all the level roles follow this dynamic standard inside Levels object, there is up to 15 levels

| Value    | Type  | Description                                                                     |
|----------|-------|---------------------------------------------------------------------------------|
| RoleId   | Ulong | This is the level role id                                                       |
| Time     | Int   | This is the time in days the user needs to be in the server to reach the role   |
| Messages | Ulong | This is the number of messages that the user needs to have to level up to there |

---

Your config file should look something like this

```json
{
  "Token": "Your discord bot token",
  "GuildId": 0,
  "YeesterId": 0,
  "Channels": {
    "Logs": 0,
    "Case": 0,
    "ModMail": 0,
    "BotCommands": 0,
    "ReactionRoles": 0
  },
  "Roles": {
    "Mod": 0
  },
  "ReactionRoles": [{"RoleId": 0, "RoleName": "Role name", "Description": "Some nice description", "MenuDescription" : "Some nice description"}],
  "Levels" : {
    "1":{
      "RoleId": 0,
      "Time": 0,
      "Messages": 0
    },
    "2":{
      "RoleId": 0,
      "Time": 0,
      "Messages": 0
    },
    "3":{
      "RoleId": 0,
      "Time": 0,
      "Messages": 0
    },
    "4":{
      "RoleId": 0,
      "Time": 0,
      "Messages": 0
    },
    "5":{
      "RoleId": 0,
      "Time": 0,
      "Messages": 0
    },
    "6":{
      "RoleId": 0,
      "Time": 0,
      "Messages": 0
    },
    "7":{
      "RoleId": 0,
      "Time": 0,
      "Messages": 0
    },
    "8":{
      "RoleId": 0,
      "Time": 0,
      "Messages": 0
    },
    "9":{
      "RoleId": 0,
      "Time": 0,
      "Messages": 0
    },
    "10":{
      "RoleId": 0,
      "Time": 0,
      "Messages": 0
    },
    "11":{
      "RoleId": 0,
      "Time": 0,
      "Messages": 0
    },
    "12":{
      "RoleId": 0,
      "Time": 0,
      "Messages": 0
    },
    "13":{
      "RoleId": 0,
      "Time": 0,
      "Messages": 0
    },
    "14":{
      "RoleId": 0,
      "Time": 0,
      "Messages": 0
    },
    "15":{
      "RoleId": 0,
      "Time": 0,
      "Messages": 0
    }
  }
}
```

### Disclaimer

if you touch anything in the bot that destroys your server it is not my problem, so don't come to FramePaste server talking about it.

if you need support with the bot i can help you on that, but i'm not in charge if you do something that destroys your server

the latest release may not be up to date to the repo at all
