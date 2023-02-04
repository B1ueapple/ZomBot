using System;
using System.Collections.Generic;
using System.Globalization;

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
		public GameLog gameLog;
	}

	public struct GameData {
		public bool active;
		public long startTime;
		public int daysElapsed;
		public int tagsToday;
	}

	public struct RoleList {
		// transient roles
		public ulong human;         // given to all humans
		public ulong zombie;        // given to all zombies
		public ulong mod;			// given to all mods (turns into veteran mod role after game ends)
		public ulong mvz;			// given to zombie(s) with the most tags
		public ulong player;        // given to all players (convenience mention for mods; turns into veteran role after game ends)
		public ulong cured;			// given to anyone that is turned from zombie -> human
	}
	
	public struct ChannelList {
		public ulong generalAnnouncementChannel;	// set manually by admin (bot manages perms automagically)

		public List<ulong> humanChannels;			// set manually by admin (bot manages perms automagically)
		public ulong humanAnnouncementChannel;		// set manually by admin (bot manages perms automagically)

		public List<ulong> zombieChannels;			// set manually by admin (bot manages perms automagically)
		public ulong zombieAnnouncementChannel;		// set manually by admin (bot manages perms automagically)

		public List<ulong> modChannels;				// set manually by admin (bot manages perms automagically)
		public ulong logChannel;					// created and managed by bot automagically

		public ulong tagChannel;					// set manually by admin (bot manages perms automagically)
	}

	public struct MissionData { // cannot be changed - based on website api
		public int id;
		public string title;
		public string body;
		public string postDate;
		public string team;

		public DateTimeOffset GetPostDate() {
			string pattern = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";

			DateTimeOffset date = DateTimeOffset.ParseExact(postDate, pattern, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None);
			return date.AddHours(Config.bot.timezone);
		}
	}

	public struct MissionList { // cannot be changed - based on website api
		public List<MissionData> missions;
	}

	public struct Clan {
		public string clanName;
		public ulong roleID;
	}
}
