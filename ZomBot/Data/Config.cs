using Newtonsoft.Json;
using System.IO;

namespace ZomBot.Data {
    class Config {
        private const string configFolder = "Resources";
        private const string configFile = "config.json";
        private const string configFolderAndFile = configFolder + "/" + configFile;

        public static BotConfig bot;

        static Config() {
            if (!Directory.Exists(configFolder))
                Directory.CreateDirectory(configFolder);

            if (!File.Exists(configFolderAndFile)) {
                bot = new BotConfig() {
                    cachesize = 1000,
                    prefix = "$",
                    apionline = false,
                    apikey = "",
                    hvzwebsite = "",
                    token = "insert bot token here"
                };

                string json = JsonConvert.SerializeObject(bot, Formatting.Indented);
                File.WriteAllText(configFolderAndFile, json);
            } else {
                string json = File.ReadAllText(configFolderAndFile);
                bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }
    }

    public struct BotConfig {
        public string token; // bot token
        public string prefix; // command prefix default $
        public string hvzwebsite; // website for hvz must start with 'https://' to work and be the full website including .com .org etc.
        public int cachesize; // default 1000, seems fine
        public string apikey; // get api key at '{website}/api/v2/auth/apikey' more api info available at https://github.com/redxdev/hvzsite/
        public bool apionline; // set this to true to enable automagic role updating. if website is not up, can cause exceptions
    }
}
