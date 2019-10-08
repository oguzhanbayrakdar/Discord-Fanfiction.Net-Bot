using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DiscordFFNetBot
{

    public class StoryData
    {
        [JsonProperty("StoryId")]
        public string StoryId { get; set; }

        [JsonProperty("ChapterCount")]
        public string ChapterCount { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("SumOfIdAndChapter")]
        public string SumOfIdAndChapter { get; set; }

    }
    public class RootObject
    {
        public List<StoryData> StoryData { get; set; }
    }

    class DataModel
    {
        private static DirectoryInfo directoryInfo = new DirectoryInfo(Environment.CurrentDirectory);
        private string dataPath = directoryInfo.Parent.Parent.Parent + "\\data.json";

        FfNetRequest _ffNetRequest = new FfNetRequest();

        public async Task<List<StoryData>> ReadDataAsync()
        {
            List<StoryData> data = new List<StoryData>();
            string fromJsonFile;
            //try catch
            using (var reader = new StreamReader(dataPath))
            {
                fromJsonFile = await reader.ReadToEndAsync();
            }

            RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(fromJsonFile);

            if (rootObject.StoryData == null)
            {
                var firstDataList = await _ffNetRequest.RecentlyUpdatedStories();

                foreach (var first in firstDataList)
                {
                    data.Add(new StoryData
                    {
                        Name = first.StoryName,
                        ChapterCount = first.Chapter,
                        StoryId = first.StoryId,
                        SumOfIdAndChapter = first.StoryId.Trim() + first.Chapter.Trim()
                    });
                }

                rootObject.StoryData = data;
                string serializedJson = JsonConvert.SerializeObject(rootObject);

                using (var writer = new StreamWriter(dataPath))
                {
                    writer.Write(serializedJson);
                    writer.Close();
                }
            }

            else
            {
                data = JsonConvert.DeserializeObject<RootObject>(fromJsonFile).StoryData;
            }

            return data;
        }

        public async Task<List<Story>> WriteDataAsync(List<Story> stories)
        {
            List<StoryData> newDataList = new List<StoryData>();
            List<StoryData> oldDataList = await ReadDataAsync();
            List<Story> returnStoryList = new List<Story>();

            foreach (var story in stories)
            {
                newDataList.Add(new StoryData
                {
                    ChapterCount = story.Chapter,
                    Name = story.StoryName,
                    StoryId = story.StoryId,
                    SumOfIdAndChapter = story.StoryId.Trim() + story.Chapter.Trim()
                });
            }


            List<string> strNewDataList = newDataList.Select(n => n.SumOfIdAndChapter).ToList();
            List<string> strOldDataList = oldDataList.Select(n => n.SumOfIdAndChapter).ToList();

            var result = strNewDataList.Except(strOldDataList).ToList();

            if (!result.Any())
            {
                return null;
            }

            foreach (var newData in newDataList)
            {
                if (result.Any())
                {
                    var data = result.Find(x => x == newData.SumOfIdAndChapter);

                    if (data != null)
                    {
                        var story = stories.Find(f => f.StoryId == newDataList.Find(n => n.SumOfIdAndChapter == data).StoryId);
                        if (story != null)
                        {
                            returnStoryList.Add(story);
                        }
                    }
                }
            }
            
            using (var writer = new StreamWriter(dataPath))
            {
                writer.Write(string.Empty);

                RootObject rootObject = new RootObject();
                rootObject.StoryData = newDataList;

                string jsonWrite = JsonConvert.SerializeObject(rootObject);

                writer.Write(jsonWrite);

                writer.Flush();
                writer.Close();
            }

            return returnStoryList;
        }
    }

}
