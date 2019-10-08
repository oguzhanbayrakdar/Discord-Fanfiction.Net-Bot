using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DiscordFFNetBot
{
    class ServerConfig
    {
        Variables variables = new Variables();
        private static DirectoryInfo directoryInfo = new DirectoryInfo(Environment.CurrentDirectory);

        private string configPath = directoryInfo.Parent.Parent.Parent + "\\config.json";

        public Variables ReadConfigData()
        {
            string jsonFromFile;
            using (var reader  = new StreamReader(configPath))
            {
                jsonFromFile = reader.ReadToEnd();
            }
            
            variables = JsonConvert.DeserializeObject<Variables>(jsonFromFile);

            return variables;
        }

    }

    internal class Variables
    {
        [JsonProperty("UseOnAllChannels")]
        public bool UseOnAllChannels { get; set; }

        [JsonProperty("ChannelList")]
        public ulong[] ChannelList { get; set; }
    }
}
