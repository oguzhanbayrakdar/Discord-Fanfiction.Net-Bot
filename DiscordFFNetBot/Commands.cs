using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;


namespace DiscordFFNetBot
{

    public class Commands : ModuleBase
    {
        private bool _isInChannel;
        private DataModel _dataModel = new DataModel();
        private static bool _isStarted;

        private List<ulong> discordChannelList;

        ServerConfig config = new ServerConfig();
        
        private readonly FfNetRequest _ffNetRequest = new FfNetRequest();

        public Commands()
        {
            if (!config.ReadConfigData().UseOnAllChannels)//If you want to use the bot on all channels, make UseOnAllChannels true. 
            {
                discordChannelList = config.ReadConfigData().ChannelList.ToList();
            }
            else
            {
                _isInChannel = true;
            }
        }

        [Command("last", false)] // Posts last updated 3 stories as Embed 
        public async Task LastUpdated()
        {
            //If you have made UseOnAllChannels true, _isInChannel will always be true otherwise it will check whether the message sent from appropriate channel or not.
            _isInChannel = config.ReadConfigData().UseOnAllChannels || discordChannelList.Contains(Context.Channel.Id);

            if (!_isInChannel)
            {
                await ReplyAsync("You can't use this command in this channel.");
            }
            else
            {
                var stories = await _ffNetRequest.RecentlyUpdatedStories();
                stories =  stories.Take(3).ToList();
                if (stories != null)
                {
                    await CreateEmbed(stories);
                }
            }
        }

        [Command("start", false)]
        public async Task StartCommand()
        {
            //If you have made UseOnAllChannels true, _isInChannel will always be true otherwise it will check whether the message sent from the appropriate channels or not.
            _isInChannel = config.ReadConfigData().UseOnAllChannels || discordChannelList.Contains(Context.Channel.Id);

            if (!_isInChannel)
            {
                await ReplyAsync("You can't use this command in this channel.");
            }
            else
            {
                if (_isStarted)
                {
                    await ReplyAsync("The program has already been started.");
                }
                else
                {
                    await ReplyAsync("The program has started.");
                    _isStarted = true;
                    LoadAsync();
                }
            }
        }

        public async Task CreateEmbed(List<Story> stories)
        {
            if (_isStarted)
            {
                if (stories.Count > 1)
                {
                    var builder = new EmbedBuilder();
                    int multipleStoryHighlightCount = 5;

                    for (int i = 0; i < stories.Count; i++)
                    {
                        if (i < multipleStoryHighlightCount)
                        {
                            string isStoryNewOrUpdated = stories[i].IsUpdated ? ":up:" : ":new:";

                            builder.AddField(isStoryNewOrUpdated + stories[i].StoryName, stories[i].Rated + " | " + stories[i].Language + " | " + stories[i].Genre + " | " + stories[i].Words + " | " + stories[i].Chapter);
                        }
                    }
                    builder.WithAuthor(author =>
                    {
                        author
                            .WithName("Multiple Stories Updated or Published")
                            .WithUrl(_ffNetRequest.storyWebPage);
                    })
                      .WithColor(255)
                      .WithCurrentTimestamp();

                    if (stories.Count > multipleStoryHighlightCount)
                    {
                        builder.WithFooter($"And {stories.Count - multipleStoryHighlightCount} more.");
                    }

                    Embed embed = builder.Build();

                    await ReplyAsync("", false, embed);
                }
                else if (stories.Count == 1)
                {
                    Story story = stories[0];

                    string characters = story.Characters != null ? ":busts_in_silhouette: " + story.Characters
                        : "There isn't any character info.";

                    var builder = new EmbedBuilder()
                    .WithDescription(
                        $"[{story.StoryName}]({story.StoryLink})")
                    .WithColor(new Color(255))
                    .WithFooter(footer =>
                    {
                        footer
                            .WithText(story.PublishDate);
                    })
                    .WithThumbnailUrl(story.StoryPicUrl)
                    .WithAuthor(author =>
                    {
                        author
                            .WithName(story.IsUpdated
                                ? "A Story Updated" + " by " + story.AuthorName
                                : "A Story Published" + " by " + story.AuthorName)
                            .WithUrl("https://www.fanfiction.net" + story.AuthorLink);

                    })
                    .WithCurrentTimestamp()
                    .AddField("Info", story.Rated + " | " + story.Language + " | " + story.Genre + " | " + story.Words + " | " + story.Chapter)
                    .AddField("Characters", characters);

                    var embed = builder.Build();

                    await ReplyAsync("", false, embed, RequestOptions.Default);

                }
            }
            else
            {
                await ReplyAsync("The program has not started yet. For start the program write ff!start command. ");
            }
        }


        //Request to Fanfiction.net every 30 seconds.
        private Timer timer;

        private List<Story> _embedStories = new List<Story>();
        private int interval = 30;
        void LoadAsync()
        {
            timer = new Timer(CallEmbedAsync,
                null,
                0, 1000 * interval); 
        }

        async void CallEmbedAsync(object state)
        {
            List<Story> stories = await _ffNetRequest.RecentlyUpdatedStories();
            List<Story> embedStories =  await _dataModel.WriteDataAsync(stories);

            _embedStories = null;
            _embedStories = embedStories;

            if (_embedStories != null)
            {
                await CreateEmbed(_embedStories);
            }
        }


    }
}