using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ZomBot.Data {
    class Accounts {
        private static readonly List<GuildData> guildAccounts;

        private static readonly string dataFolder = "Data";
        private static readonly string dataFile = "guilds.json";
        private static readonly string dataFolderAndDataFile = dataFolder + "/" + dataFile;

        static Accounts() {
            if (File.Exists($"{dataFolder}/{dataFile}")) {
                guildAccounts = DataStorage.LoadAccounts(dataFolderAndDataFile).ToList();
            } else {
                guildAccounts = new List<GuildData>();
                SaveAccounts();
            }
        }

        public static void SaveAccounts() {
            if (!Directory.Exists(dataFolder))
                Directory.CreateDirectory(dataFolder);

            DataStorage.SaveAccounts(guildAccounts, dataFolderAndDataFile);
        }

        public static UserData GetUser(SocketGuildUser user) {
            return GetOrCreateUserAccount(user.Id, user.Guild.Id, user.Username);
        }

        public static UserData GetUser(IUser user, IGuild guild) {
            return GetOrCreateUserAccount(user.Id, guild.Id, user.Username);
        }

        public static UserData GetUser(ulong userid, ulong guildid) {
            return GetOrCreateUserAccount(userid, guildid, "");
        }

        public static GuildData GetGuild(IGuild guild) {
            return GetOrCreateGuildAccount(guild.Id);
        }

        public static GuildData GetGuild(ulong id) {
            return GetOrCreateGuildAccount(id);
        }

        private static UserData GetOrCreateUserAccount(ulong userid, ulong guildid, string username) {
            var result = from a in GetGuild(guildid).userData
                         where a.id == userid
                         select a;

            var account = result.FirstOrDefault();

            if (account == null)
                account = CreateUserAccount(userid, guildid, username);

            return account;
        }

        private static GuildData GetOrCreateGuildAccount(ulong id) {
            var result = from a in guildAccounts
                         where a.id == id
                         select a;

            var account = result.FirstOrDefault();

            if (account == null)
                account = CreateGuildAccount(id);

            return account;
        }

        private static UserData CreateUserAccount(ulong userid, ulong guildid, string username) {
            var newAccount = new UserData() {
                id = userid,
                discordUsername = username,
                playerData = new PlayerData(),
                blacklisted = false,
                warnings = new List<Warning>(),
                specialPlayerData = new SpecialPlayerData() {
                    cured = false,
                    isMVZ = false,
                    tagsToday = 0
                }
            };

            GetGuild(guildid).userData.Add(newAccount);
            SaveAccounts();
            return newAccount;
        }

        private static GuildData CreateGuildAccount(ulong id) {
            var newAccount = new GuildData() {
                id = id,
                userData = new List<UserData>(),
                channels = new ChannelList() {
                    humanChannels = new List<ulong>(),
                    zombieChannels = new List<ulong>(),
                    modChannels = new List<ulong>()
                },
                roleIDs = new RoleList(),
                missions = new MissionList() {
                    missions = new List<MissionData>()
                },
                setupComplete = false,
                clanList = new List<Clan>(),
                gameData = new GameData() {
                    active = false,
                    startTime = 0,
                    daysElapsed = 0,
                    tagsToday = 0
                },
                gameLog = new GameLog() {
                    messages = new List<GameLogMessage>(),
                    gameStage = 0
				}
            };

            guildAccounts.Add(newAccount);
            SaveAccounts();
            return newAccount;
        }
    }
}
