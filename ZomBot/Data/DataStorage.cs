using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace ZomBot.Data {
    public static class DataStorage {
        public static void SaveAccounts(IEnumerable<GuildData> accounts, string filePath)
        {
            string json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static IEnumerable<GuildData> LoadAccounts(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<GuildData>>(json);
        }
    }
}
