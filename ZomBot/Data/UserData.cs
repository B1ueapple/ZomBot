using Discord;
using System.Collections.Generic;

namespace ZomBot.Data {
	public class UserData {
		public ulong id;
		public string discordUsername;
		public PlayerData playerData;
		public bool blacklisted;
		public List<Warning> warnings;
		public SpecialPlayerData specialPlayerData;
		public OZApplication ozApp;

		public void AddWarning(IUser issuer, string reason) {
			if (blacklisted)
				return;

			var time = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();

			if (warnings == null)
				warnings = new List<Warning>();

			Warning warning = new Warning() {
				issuer = issuer.Id,
				reason = reason,
				time = time
			};
			warnings.Add(warning);
			Accounts.SaveAccounts();
		}
	}

	public struct SpecialPlayerData {
		public bool cured;
		public bool isMVZ;
		public int tagsToday;
	}
	
	public class PlayerData { // cannot be changed - based on website api
		public int id;
		public string name;
		public string team;
		public string clan;
		public string access;
		public int humansTagged;
	}

	public struct Warning {
		public ulong issuer;
		public string reason;
		public long time;
	}

	public struct OZApplication {
		public bool applied;
		public string rating;
		public string time;
		public string experience;
		public string why;
	}
}
