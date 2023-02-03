using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace ZomBot.Data {
	public static class DataStorage {
		public static void SaveAccounts(IEnumerable<GuildData> accounts, string filePath) {
			string json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
			File.WriteAllText(filePath, json);
		}

		public static IEnumerable<GuildData> LoadAccounts(string filePath) {
			if (!File.Exists(filePath))
				return null;

			string json = File.ReadAllText(filePath);

			return JsonConvert.DeserializeObject<List<GuildData>>(json);
		}

		private class ChatLogInternal {
			public ulong UserID;
			public List<ChatMessageInternal> Messages;
		}

		private class ChatMessageInternal {
			public ulong MessageID;
			public long Timestamp;
			public string OriginalContent;
			public LocalMessageChannelInternal ChannelInfo;
			public string CurrentContent;
			public List<string> Edits;
			public bool Edited;
			public bool Deleted;
		}

		private struct LocalMessageChannelInternal {
			public ulong ID;
			public string Name;
		}

		public static void SaveChatLogs(IEnumerable<ChatLog> logs, string filePath) {
			var chatLogsInternal = new List<ChatLogInternal>();

			foreach (ChatLog log in logs) {
				var messageList = new List<ChatMessageInternal>();

				if (log.Messages.Count > 0) {
					foreach (ChatMessage msg in log.Messages) {
						messageList.Add(new ChatMessageInternal() {
							MessageID = msg.MessageID,
							Timestamp = msg.Timestamp,
							OriginalContent = msg.OriginalContent,
							ChannelInfo = new LocalMessageChannelInternal() {
								ID = msg.ChannelInfo.ID,
								Name = msg.ChannelInfo.Name
							},
							CurrentContent = msg.CurrentContent,
							Edits = msg.Edits,
							Edited = msg.Edited,
							Deleted = msg.Deleted
						});
					}
				}

				chatLogsInternal.Add(new ChatLogInternal() {
					UserID = log.UserID,
					Messages = messageList
				});
			}

			string json = JsonConvert.SerializeObject(chatLogsInternal, Formatting.Indented);
			File.WriteAllText(filePath, json);
		}

		public static IEnumerable<ChatLog> LoadChatLogs(string filePath) {
			if (!File.Exists(filePath))
				return null;

			string json = File.ReadAllText(filePath);

			var chatLogs = new List<ChatLog>();
			var chatLogsInternal = JsonConvert.DeserializeObject<List<ChatLogInternal>>(json);

			if (chatLogsInternal.Count > 0)
				foreach (ChatLogInternal log in chatLogsInternal) {
					var messageList = new List<ChatMessage>();

					if (log.Messages.Count > 0)
						foreach (ChatMessageInternal msg in log.Messages)
							messageList.Add(new ChatMessage(msg.MessageID,
								msg.Timestamp,
								msg.OriginalContent,
								new LocalMessageChannel(msg.ChannelInfo.ID, msg.ChannelInfo.Name),
								msg.CurrentContent,
								msg.Edits,
								msg.Deleted));

					chatLogs.Add(new ChatLog(log.UserID, messageList));
				}

			return chatLogs;
		}
	}
}
