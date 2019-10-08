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
1. HtmlAgilityPack
2. Newtonsoft.Json
3. Discord.Net

# SDK
Microsoft.NETCore.App 2.1

# Images from the Bot
![ffstart](https://user-images.githubusercontent.com/17615607/66404229-b7684e80-e9f0-11e9-82e2-e480a9b22d94.PNG)

![singleupdate](https://user-images.githubusercontent.com/17615607/66404226-b6cfb800-e9f0-11e9-8563-cea83435df26.PNG)

![lastormultipleupdate](https://user-images.githubusercontent.com/17615607/66404225-b6372180-e9f0-11e9-9b4f-7a8a8fd113e6.PNG)
