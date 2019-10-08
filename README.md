# Discord Fanfiction.Net Bot
 A bot that fetchs data from Fanfiction.net


# Things you have to do before running the program: 

1. Open a internet browser and go to the following link: https://discordapp.com/developers/applications
2. Create an application, add a bot and get the bot’s token.
3. In the repository you have downloaded, open “Program.cs” file and replace “Token Goes Here” text in the Start method with your Token provided by Discord. After that, open “FFNetRequest.cs” file change “storyWebPage” variable’s value to fandom link you want.
4. Open Discord, activate developer mode after that open your discord server and copy the list of channels that you want to activate bot commands.
5. In the repository you have “config.json” file, open it and paste the channel list in “ChannelList” array. If you want all channels to use bot commands, just change “UseOnAllChannels“s
value to true otherwise false.
6. Add bot to your server.
7. Start the program.

# Things you have to do AFTER running the program.

1. If you can see the “Discord Fanfiction.net Story bot is ready!” text that means you are in the right way.
2. Type “ff!start” in the main channel that you want to get update posts from bot.
3. In all the other channel/s you have activated bot commands, you can write “ff!last” and get the last updated 3 story.

# DEPENDENCIES
HtmlAgilityPack
Newtonsoft.Json
Discord.Net

# SDK
Microsoft.NETCore.App 2.1
