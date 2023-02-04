using Discord.WebSocket;
using System;
using System.Collections.Generic;

namespace ZomBot.Data {
	public class GameLog {
		public List<GameLogMessage> messages;
		public int gameStage;
		public long startTime;
		public long endTime;

		public string GetFormattedMessages(GameLogMessageFormat format) {
			string msg = $"HvZ : {DateTimeOffset.FromUnixTimeMilliseconds(startTime).ToLocalTime().Month}/{DateTimeOffset.FromUnixTimeMilliseconds(startTime).ToLocalTime().Day}/{DateTimeOffset.FromUnixTimeMilliseconds(startTime).ToLocalTime().Year} to {DateTimeOffset.FromUnixTimeMilliseconds(endTime).ToLocalTime().Month}/{DateTimeOffset.FromUnixTimeMilliseconds(endTime).ToLocalTime().Day}/{DateTimeOffset.FromUnixTimeMilliseconds(endTime).ToLocalTime().Year} \n--------------------------------------------------\n";

			foreach (GameLogMessage glm in messages)
				msg += $"{glm.FormattedMessage(format)}\n";

			msg += "--------------------------------------------------";
			return msg;
		}

		private void AddMessage(GameLogMessage msg) {
			if ((messages?.Count ?? 0) == 0)
				messages = new List<GameLogMessage>();

			messages.Add(msg);
			Accounts.SaveAccounts();
		}

		public void TagMessage(UserData tagged) {
			var msg = new GameLogMessage() {
				associatedUsers = new List<GameLogUser>() {
					new GameLogUser() {
						discordID = tagged.id,
						discordUsername = tagged.discordUsername,
						websiteName = tagged.playerData.name,
						index = 1
					}
				},
				message = "%name1% has been infected.",
				time = DateTimeOffset.Now.ToUnixTimeMilliseconds()
			};
			AddMessage(msg);
		}

		public void ZombieRecapMessage(UserData zombie) {
			var msg = new GameLogMessage() {
				associatedUsers = new List<GameLogUser>() {
					new GameLogUser() {
						discordID = zombie.id,
						discordUsername = zombie.discordUsername,
						websiteName = zombie.playerData.name,
						index = 1
					}
				},
				message = $"%name1% infected {zombie.specialPlayerData.tagsToday} human{(zombie.specialPlayerData.tagsToday == 1 ? "" : "s")} today.",
				time = DateTimeOffset.Now.ToUnixTimeMilliseconds()
			};
			AddMessage(msg);
		}

		public void PlayerSurvivedMessage(UserData survivor) {

			var msg = new GameLogMessage() {
				associatedUsers = new List<GameLogUser>() {
					new GameLogUser() {
						discordID = survivor.id,
						discordUsername = survivor.discordUsername,
						websiteName = survivor.playerData.name,
						index = 1
					}
				},
				message = "%name1% survived the horde.",
				time = DateTimeOffset.Now.ToUnixTimeMilliseconds()
			};
			AddMessage(msg);
		}

		public void StartMessage() {
			var msg = new GameLogMessage() {
				associatedUsers = new List<GameLogUser>(),
				message = "The game has started.",
				time = DateTimeOffset.Now.ToUnixTimeMilliseconds()
			};
			AddMessage(msg);
		}

		public void EndMessage(bool survivors) {
			var msg = new GameLogMessage() {
				associatedUsers = new List<GameLogUser>(),
				message = $"The game has ended{(survivors ? $"" : " with no survivors")}.",
				time = DateTimeOffset.Now.ToUnixTimeMilliseconds()
			};
			AddMessage(msg);
		}

		// must call quartertagged -> halftagged -> threequarterstagged in order to log
		public void EventMessage(GameLogEvents e, UserData assocPlayer = null, string clan = null, int num1 = 0, int num2 = 0) {
			var msg = new GameLogMessage() {
				associatedUsers = new List<GameLogUser>(),
				time = DateTimeOffset.Now.ToUnixTimeMilliseconds()
			};

			switch (e) {
				case GameLogEvents.QUARTERTAGGED:
					if (gameStage != 0)
						return;

					gameStage = 1;
					msg.message = "A quarter of all humans have been infected!";
					break;
				case GameLogEvents.HALFTAGGED:
					if (gameStage != 1)
						return;

					gameStage = 2;
					msg.message = "Half of all humans have been infected!";
					break;
				case GameLogEvents.THREEQUARTERSTAGGED:
					if (gameStage != 2)
						return;

					gameStage = 3;
					msg.message = "The vast majority of the humans have been infected!";
					break;
				case GameLogEvents.PLAYERCURED:
					msg.message = "%name1% has been cured.";
					msg.associatedUsers.Add(new GameLogUser() {
						discordID = assocPlayer.id,
						discordUsername = assocPlayer.discordUsername,
						websiteName = assocPlayer.playerData.name,
						index = 1
					});
					break;
				case GameLogEvents.WASTEDCURE:
					msg.message = "%name1% has wasted their cure.";
					msg.associatedUsers.Add(new GameLogUser() {
						discordID = assocPlayer.id,
						discordUsername = assocPlayer.discordUsername,
						websiteName = assocPlayer.playerData.name,
						index = 1
					});
					break;
				case GameLogEvents.NEWMVZ:
					msg.message = $"%name1% has achieved a new high of {num1} infection{(num1 == 1 ? "" : "s")}!";
					msg.associatedUsers.Add(new GameLogUser() {
						discordID = assocPlayer.id,
						discordUsername = assocPlayer.discordUsername,
						websiteName = assocPlayer.playerData.name,
						index = 1
					});
					break;
				case GameLogEvents.ENDOFDAY:
					msg.message = $"~~ Day ended with {num1} infection{(num1 == 1 ? "" : "s")} and {num2} human{(num2 == 1 ? "" : "s")} remaining. ~~";
					break;
				case GameLogEvents.CLANWIPED:
					msg.message = $"All of '{clan}' has been wiped out!";
					break;
			}

			AddMessage(msg);
		}
	}

	public struct GameLogMessage {
		public List<GameLogUser> associatedUsers;
		public string message;
		public long time;

		public string FormattedMessage(GameLogMessageFormat format) {
			string msg = $"[{DateTimeOffset.FromUnixTimeMilliseconds(time).ToLocalTime().DayOfWeek} @ {DateTimeOffset.FromUnixTimeMilliseconds(time).ToLocalTime().Hour}:{DateTimeOffset.FromUnixTimeMilliseconds(time).ToLocalTime().Minute}] {message}";

			if (associatedUsers.Count > 0) {
				foreach (GameLogUser user in associatedUsers) {
					string name = "%error%";

					switch (format) {
						case GameLogMessageFormat.DISCORDNAME:
							if ((user.discordUsername ?? "") != "")
								name = user.discordUsername;
							break;
						case GameLogMessageFormat.DISCORDMENTION:
							name = $"<@{user.discordID}>";
							break;
						case GameLogMessageFormat.WEBSITE:
							if ((user.websiteName ?? "") != "")
								name = user.websiteName;
							break;
					}

					msg = msg.Replace($"%name{user.index}%", name);
				}
			}

			return msg;
		}
	}

	public enum GameLogMessageFormat {
		DISCORDNAME,
		DISCORDMENTION,
		WEBSITE
	}

	public enum GameLogEvents {
		QUARTERTAGGED,
		HALFTAGGED,
		THREEQUARTERSTAGGED,
		PLAYERCURED,
		WASTEDCURE,
		NEWMVZ,
		ENDOFDAY,
		CLANWIPED
	}

	public struct GameLogUser {
		public ulong discordID;
		public string discordUsername;
		public string websiteName;
		public int index;
	}
}
