using System.Collections.Generic;

namespace ZomBot.Data {
    public class GuildData {
        public ulong id;
        public List<UserData> userData;
        public RoleList roleIDs;
        public ChannelList channels;
        public MissionList missions;
        public bool setupComplete;
        public List<Clan> clanList;
    }

    public struct RoleList {
        public ulong human;
        public ulong zombie;
        public ulong mod;
        public ulong mvz;
        public ulong player;
    }
    
    public struct ChannelList {
        public List<ulong> zombieChannels;
        public ulong zombieAnnouncementChannel;
        public List<ulong> humanChannels;
        public ulong humanAnnouncementChannel;
        public List<ulong> modChannels;
        public ulong generalAnnouncementChannel;
        public ulong logChannel;
    }

    public struct MissionData { // cannot be changed - based on website api
        public int id;
        public string title;
        public string body;
        public string team;
    }

    public struct MissionList { // cannot be changed - based on website api
        public List<MissionData> missions;
    }

    public struct Clan {
        public string clanName;
        public ulong roleID;
    }
}
