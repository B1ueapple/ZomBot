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
		public GameData gameData;
	}

	public struct GameData {
		public bool active;
		public long startTime;
    }

	public struct RoleList {
		// transient roles
		public ulong human;         // given to all humans
		public ulong zombie;        // given to all zombies
		public ulong mod;			// given to all mods
		public ulong mvz;			// given to zombie(s) with the most tags
		public ulong player;        // given to all players (convenience mention for mods)
		public ulong revived;       // given to anyone that is turned from zombie -> human

		// roles that persist
		public ulong veteran;		// awarded to all players after game concludes
		public ulong veteranmod;	// awarded to all mods after game concludes
		public ulong survivor;		// awarded if human after end of final night mission

	}
	
	public struct ChannelList {
		public ulong generalAnnouncementChannel;	// set manually by admin (bot manages perms automagically)

		public List<ulong> humanChannels;			// set manually by admin (bot manages perms automagically)
		public ulong humanAnnouncementChannel;		// set manually by admin (bot manages perms automagically)

		public List<ulong> zombieChannels;			// set manually by admin (bot manages perms automagically)
		public ulong zombieAnnouncementChannel;		// set manually by admin (bot manages perms automagically)

		public List<ulong> modChannels;				// set manually by admin (bot manages perms automagically)
		public ulong logChannel;					// set manually by admin (bot manages perms automagically)

		public ulong postgameAnnouncementsChannel;	// created after game by bot
		public ulong afterthoughtsChannel;			// created after game by bot
		public ulong criticismsChannel;				// created after game by bot
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
