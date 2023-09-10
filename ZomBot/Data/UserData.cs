using Discord;
using System.Collections.Generic;

namespace ZomBot.Data {
	public class UserData {
		public ulong id;
		public string discordUsername;
		public PlayerData playerData;
		public ExtendedPlayerData extendedPlayerData;
		public bool blacklisted;
		public SpecialPlayerData specialPlayerData;
		public OZApplication ozApp;
	}

	public struct SpecialPlayerData {
		public bool cured;
		public bool isMVZ;
	}
	
	public class PlayerData { // cannot be changed - based on website api
		public int id;
		public string name;
		public string team;
		public string clan;
		public string access;
		public int humansTagged;
	}

	public class ExtendedPlayerData { // cannot be changed - based on website api
		public string zombieId;
		public bool oz;
		public List<HumanID> humanIds;
	}

	public class HumanID { // cannot be changed - based on website api
		public string idString;
		public bool active;
	}

	public class EPDReturn { // cannot be changed - based on website api
		public ExtendedPlayerData user;
	}

	public struct OZApplication {
		public bool applied;
		public string rating;
		public string time;
		public string experience;
		public string why;
	}
}
