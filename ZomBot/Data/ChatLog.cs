using Discord;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ZomBot.Data {
	class ChatManager {
		private static readonly List<ChatLog> chatLogs;

		private static readonly string dataFolder = "Data";
		private static readonly string dataFile = "chat_logs.json";
		private static readonly string dataFolderAndDataFile = dataFolder + "/" + dataFile;

		static ChatManager() {
			if (File.Exists($"{dataFolder}/{dataFile}")) {
				chatLogs = DataStorage.LoadChatLogs(dataFolderAndDataFile)?.ToList() ?? new List<ChatLog>();
			} else {
				chatLogs = new List<ChatLog>();
				SaveChatLogs();
			}
		}

		public static void SaveChatLogs() {
			if (!Directory.Exists(dataFolder))
				Directory.CreateDirectory(dataFolder);

			DataStorage.SaveChatLogs(chatLogs, dataFolderAndDataFile);
		}

		public static ChatLog GetChatLog(IUser user) {
			return GetOrCreateChatLog(user.Id);
		}

		public static ChatLog GetChatLog(ulong userID) {
			return GetOrCreateChatLog(userID);
		}

		private static ChatLog GetOrCreateChatLog(ulong userid) {
			var result = from a in chatLogs
						 where a.UserID == userid
						 select a;

			var log = result.FirstOrDefault();

			if (log == null) {
				log = new ChatLog(userid);
				chatLogs.Add(log);
			}

			return log;
		}
	}

	public class ChatLog {
		public readonly ulong UserID;
		public List<ChatMessage> Messages { get; private set; }

		public ChatLog(ulong userID, List<ChatMessage> messages = null) {
			UserID = userID;
			Messages = messages ?? new List<ChatMessage>();
		}

		public void AddMessage(IMessage message) {
			AddMessage(new ChatMessage(message));
		}

		public void AddMessage(ChatMessage message) {
			Messages.Add(message);
		}

		public void EditMessage(ulong messageID, string newContent) {
			var result = from a in Messages
						 where a.MessageID == messageID
						 select a;

			var log = result.FirstOrDefault();

			if (log == null)
				Program.Error("Tried to edit a non-existent message.");
			else
				log.AddEdit(newContent);
		}

		public void DeleteMessage(ulong messageID) {
			var result = from a in Messages
						 where a.MessageID == messageID
						 select a;

			var log = result.FirstOrDefault();

			if (log == null)
				Program.Error("Tried to set deleted on non-existent message.");
			else
				log.SetDeleted();
		}
	}

	public struct LocalMessageChannel {
		public readonly ulong ID;
		public readonly string Name;

		public LocalMessageChannel(ulong id, string name) {
			ID = id;
			Name = name;
		}

		public LocalMessageChannel(IMessageChannel channel) {
			ID = channel.Id;
			Name = channel.Name;
		}
	}

	public class ChatMessage {
		public readonly ulong MessageID;
		public readonly long Timestamp;
		public readonly string OriginalContent;
		public readonly LocalMessageChannel ChannelInfo;

		public string CurrentContent { get; private set; }
		public List<string> Edits { get; private set; }

		public bool Edited { get; private set; }
		public bool Deleted { get; private set; }

		public ChatMessage(IMessage message) {
			MessageID = message.Id;
			Timestamp = message.Timestamp.ToUnixTimeMilliseconds();
			OriginalContent = message.CleanContent;
			CurrentContent = message.CleanContent;
			ChannelInfo = new LocalMessageChannel(message.Channel);

			Edits = new List<string>();
			Edited = false;
			Deleted = false;
		}

		public ChatMessage(ulong messageID, long timestamp, string content, LocalMessageChannel channelInfo, string current = null, List<string> edits = null, bool deleted = false) {
			MessageID = messageID;
			Timestamp = timestamp;
			OriginalContent = content;
			CurrentContent = current ?? content;
			ChannelInfo = channelInfo;
			Edits = edits ?? new List<string>();
			Edited = Edits.Count > 0;
			Deleted = deleted;
		}

		public void SetDeleted() {
			CurrentContent = "";
			Deleted = true;
		}
		
		public void AddEdit(string newContent) {
			CurrentContent = newContent;
			Edits.Add(newContent);
			Edited = true;
		}
	}
}
