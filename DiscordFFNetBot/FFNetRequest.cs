using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Discord;
using Discord.Commands;
namespace DiscordFFNetBot
{
    class FfNetRequest
    {

        public string storyWebPage =
            "http://www.fanfiction.net/book/Harry-Potter/?&srt=1&lan=1&r=10";

        private int storyCount = 5;

        private static List<string> genreList = new List<string>
        {
            "Adventure",
            "Angst",
            "Crime",
            "Drama",
            "Family",
            "Fantasy",
            "Friendship",
            "General",
            "Horror",
            "Humor",
            "Hurt/Comfort",
            "Mystery",
            "Parody",
            "Poetry",
            "Romance",
            "Sci-fi",
            "Spiritual",
            "Supernatural",
            "Suspense",
            "Tragedy",
            "Western"
        };

        private async Task<HtmlDocument> Response()
        {
            HtmlDocument mainDoc = new HtmlDocument();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(storyWebPage);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            request.Credentials = CredentialCache.DefaultCredentials;

            WebResponse response = await request.GetResponseAsync();
                        
            Stream stream = response.GetResponseStream();

            StreamReader reader = new StreamReader(stream ?? throw new InvalidOperationException());

            string data = reader.ReadToEnd();

            mainDoc.LoadHtml(data);

            return mainDoc;
        }

        public async Task<List<Story>> RecentlyUpdatedStories()
        {
            //Gets Html Content
            HtmlDocument document = await Response();

            if (document == null)
            {
                return null;
            }

            List<Story> stories = new List<Story>();

            for (int i = 1; i <= storyCount; i++)
            {
                Story story = new Story();

                List<string> storyInfoStrList = new List<string>();

                //These nodes gets necessary info from HtmlDocument. Like story link, story name, author name and other story infos. And it checks whether a story is new or updated.
                #region Nodes

                HtmlNode storyLinkNode =
                    document.DocumentNode.SelectSingleNode(
                        $"//*[@id=\"content_wrapper_inner\"]/div[contains(@class,\"z-list\")][{i}]/a[@class=\"stitle\"]");

                HtmlNode storyNameNode =
                    document.DocumentNode.SelectSingleNode(
                        $"//*[@id=\"content_wrapper_inner\"]/div[contains(@class,\"z-list\")][{i}]/a[@class=\"stitle\"]/text()");

                HtmlNode storyNewOrUpdatedNode =
                    document.DocumentNode.SelectSingleNode(
                        $"//*[@id=\"content_wrapper_inner\"]/div[contains(@class,\"z-list\")][{i}]/a[2]/span");

                HtmlNode authorNameNode = null;

                HtmlNode storyInfoNode =
                    document.DocumentNode.SelectSingleNode(
                        $"//*[@id=\"content_wrapper_inner\"]/div[contains(@class,\"z-list\")][{i}]/div/div");
                #endregion

                //storyInfoStrList contains informations(rating, language, genre, chapter, words, characters; publish and update date) that gets from storyInfoNode via string split method.
                storyInfoStrList = storyInfoNode.InnerText.Split("-").ToList();

                //Story Name
                story.StoryName = storyNameNode.InnerText;

                //URL of last updated story
                story.StoryLink = "https://www.fanfiction.net" + storyLinkNode.GetAttributeValue("href", null);

                //Gets ID of the story.
                story.StoryId = storyLinkNode.GetAttributeValue("href", null).Split("/")[2];

                //Picture of last updated story
                story.StoryPicUrl = "https:" + storyLinkNode.ChildNodes["img"].GetAttributeValue("data-original",
                    "//ff74.b-cdn.net/static/images/d_60_90.jpg");

                //If a story is new. It doesn't get any update date but It does get publish date.
                //If a story is not new but updated. It does get publish and update dates.
                //Gets author name, sets author name node and checks is updated or not.
                if (storyNewOrUpdatedNode == null)
                {
                    story.IsUpdated = false;

                    authorNameNode =
                        document.DocumentNode.SelectSingleNode(
                            $"//*[@id=\"content_wrapper_inner\"]/div[contains(@class,\"z-list\")][{i}]/a[2]");
                    story.AuthorName = authorNameNode.InnerText;
                }
                else
                {
                    story.IsUpdated = true;

                    authorNameNode =
                        document.DocumentNode.SelectSingleNode(
                            $"//*[@id=\"content_wrapper_inner\"]/div[contains(@class,\"z-list\")][{i}]/a[3]");
                    story.AuthorName = authorNameNode.InnerText;

                }

                //Gets author's Fanfiction.net Profile URL
                story.AuthorLink = authorNameNode.GetAttributeValue("href", null);

                //Rating, Language, Words Count, Chapters Count, Publish Date.
                #region informations that every story had

                story.Rated = storyInfoStrList.Find(w => w.Contains("Rated"));//Rating
                story.Language = storyInfoStrList[1]; //Language
                story.Words = storyInfoStrList.Find(w => w.Contains("Words"));//Words
                story.Chapter = storyInfoStrList.Find(c => c.Contains("Chapters"));//Chapters
                story.PublishDate = storyInfoStrList.Find(p => p.Contains("Published"));//Published

                #endregion

                //Update Date
                story.UpdateDate = story.IsUpdated
                    ? storyInfoStrList.Find(u => u.Contains("Updated"))+" ago."
                    : null;

                //Genres
                for (var j = 0; j < storyInfoStrList.Count; j++)
                {
                    if (genreList.Any(str => storyInfoStrList[j].IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0)) //If story has a genre that the genre list has.
                    {
                        story.Genre = storyInfoStrList[j];
                        break;
                    }
                    else
                    {
                        story.Genre = "No Genre Info";
                    }
                }

                //Characters
                #region Characters

                if (storyInfoStrList.Any(x => x.Contains("Complete")))
                {

                    string characters = storyInfoStrList[storyInfoStrList.Count - 2];
                    if (characters.Contains("Published") || characters.Contains("Updated"))
                    {
                        story.Characters = null;
                    }
                    else
                    {
                        story.Characters = characters;
                    }
                    story.IsCompleted = true;
                }
                else
                {
                    string characters = storyInfoStrList[storyInfoStrList.Count - 1];
                    if (characters.Contains("Published") || characters.Contains("Updated"))
                    {
                        story.Characters = null;
                    }
                    else
                    {
                        story.Characters = characters;
                    }
                }


                #endregion

                stories.Add(story);
                
            }
            
            return stories;
        }
    }
}