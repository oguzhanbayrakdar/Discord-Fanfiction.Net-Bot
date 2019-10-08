using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordFFNetBot
{
    public class Story
    {
        public string StoryId { get; set; }
        public string StoryName { get; set; }
        public string StoryLink { get; set; }
        public string StoryPicUrl { get; set; }

        public string AuthorName { get; set; }
        public string AuthorLink { get; set; }
        
        public string Rated { get; set; }
        public string Language { get; set; }
        public string Genre { get; set; }
        public string Chapter { get; set; }
        public string Words { get; set; }

        public bool IsUpdated { get; set; }
        public string UpdateDate { get; set; }

        public string PublishDate { get; set; }
        public string Characters { get; set; }

        public bool IsCompleted { get; set; }

    }
}
