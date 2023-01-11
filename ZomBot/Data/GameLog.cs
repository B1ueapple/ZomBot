using Discord.WebSocket;
using System;
using System.Collections.Generic;

namespace ZomBot.Data {
	public class GameLog {
		public List<GameLogMessage> messages;
		public int gameStage;

		public string GetFormattedMessages(GameLogMessageFormat format) {
			string msg = "";

			foreach (GameLogMessage glm in messages) {
				msg += $"{glm.FormattedMessage(format)}\n";
			}

			return msg;
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
				message = "%name1% has been tagged.",
				time = DateTimeOffset.Now.ToUnixTimeMilliseconds()
			};
			messages.Add(msg);
			Accounts.SaveAccounts();
		}

		public void PlayerSurvivedMessage(SocketGuildUser survivor) {
			var data = Accounts.GetUser(survivor);

			var msg = new GameLogMessage() {
				associatedUsers = new List<GameLogUser>() {
					new GameLogUser() {
						discordID = data.id,
						discordUsername = data.discordUsername,
						websiteName = data.playerData.name,
						index = 1
					}
				},
				message = "%name1% survived the horde.",
				time = DateTimeOffset.Now.ToUnixTimeMilliseconds()
			};
			messages.Add(msg);
			Accounts.SaveAccounts();
		}

		public void StartMessage() {
			var msg = new GameLogMessage() {
				associatedUsers = new List<GameLogUser>(),
				message = "The game has started.",
				time = DateTimeOffset.Now.ToUnixTimeMilliseconds()
			};
			messages.Add(msg);
			Accounts.SaveAccounts();
		}

		public void EndMessage(bool survivors) {
			var msg = new GameLogMessage() {
				associatedUsers = new List<GameLogUser>(),
				message = $"The game has ended{(survivors ? "" : " with no survivors")}.",
				time = DateTimeOffset.Now.ToUnixTimeMilliseconds()
			};
			messages.Add(msg);
			Accounts.SaveAccounts();
		}

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
					msg.message = "1/4 of the humans have been infected!";
					break;
				case GameLogEvents.HALFTAGGED:
					if (gameStage != 1)
						return;

					gameStage = 2;
					msg.message = "1/2 of the humans have been infected!";
					break;
				case GameLogEvents.THREEQUARTERSTAGGED:
					if (gameStage != 2)
						return;

					gameStage = 3;
					msg.message = "3/4 of the humans have been infected!";
					break;
				case GameLogEvents.PLAYERCURED:
					msg.message = "%name1% has been cured!";
					msg.associatedUsers.Add(new GameLogUser() {
						discordID = assocPlayer.id,
						discordUsername = assocPlayer.discordUsername,
						websiteName = assocPlayer.playerData.name,
						index = 1
					});
					break;
				case GameLogEvents.WASTEDCURE:
					msg.message = "%name1% has wasted their cure!";
					msg.associatedUsers.Add(new GameLogUser() {
						discordID = assocPlayer.id,
						discordUsername = assocPlayer.discordUsername,
						websiteName = assocPlayer.playerData.name,
						index = 1
					});
					break;
				case GameLogEvents.NEWMVZ:
					msg.message = $"%name1% has set a new record for tags: {num1}";
					msg.associatedUsers.Add(new GameLogUser() {
						discordID = assocPlayer.id,
						discordUsername = assocPlayer.discordUsername,
						websiteName = assocPlayer.playerData.name,
						index = 1
					});
					break;
				case GameLogEvents.ENDOFDAY:
					msg.message = $"---------------\nDay ended with {num1} tags and {num2} humans left.\n---------------";
					break;
				case GameLogEvents.CLANWIPED:
					msg.message = $"{clan} has been wiped out!";
					break;
			}

			messages.Add(msg);
			Accounts.SaveAccounts();
		}
	}

	public struct GameLogMessage {
		public List<GameLogUser> associatedUsers;
		public string message;
		public long time;

		public string FormattedMessage(GameLogMessageFormat format) {
			string msg = $"[{DateTimeOffset.FromUnixTimeMilliseconds(time).Month}/{DateTimeOffset.FromUnixTimeMilliseconds(time).Day} @ {DateTimeOffset.FromUnixTimeMilliseconds(time).Hour}:{DateTimeOffset.FromUnixTimeMilliseconds(time).Minute}:{DateTimeOffset.FromUnixTimeMilliseconds(time).Second}] {message}";

			if (associatedUsers.Count > 0) {
				foreach (GameLogUser user in associatedUsers) {
					string name = "";

					switch (format) {
						case GameLogMessageFormat.DISCORDNAME:
							name = user.discordUsername;
							break;
						case GameLogMessageFormat.DISCORDMENTION:
							name = $"<@{user.discordID}>";
							break;
						case GameLogMessageFormat.WEBSITE:
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
		ENDOFDAY, // unimplemented
		CLANWIPED
	}

	public struct GameLogUser {
		public ulong discordID;
		public string discordUsername;
		public string websiteName;
		public int index;
	}
}
