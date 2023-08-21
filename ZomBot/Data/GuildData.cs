using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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
		// transient roles
		public ulong human;         // given to all humans
		public ulong zombie;        // given to all zombies
		public ulong mvz;			// given to zombie(s) with the most tags
		public ulong cured;			// given to anyone that is turned from zombie -> human

		// static roles
		public ulong player;        // given to all players (convenience mention for mods; turns into veteran role after game ends)
		public ulong mod;			// given to all mods (turns into veteran mod role after game ends)
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

	public enum ChannelDesignation {
		MOD,
		MODIMPORTANT,
		LOG,
		TAG,
		HUMAN,
		HUMANANNOUNCEMENT,
		ZOMBIE,
		ZOMBIEANNOUNCEMENT,
		SHAREDANNOUNCEMENT
	}
}
