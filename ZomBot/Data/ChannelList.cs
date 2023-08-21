using System.Collections.Generic;

namespace ZomBot.Data {
	public class ChannelList {
		public List<Channel> channels;

		public ChannelList() {
			channels = new List<Channel>();
		}

		/// <summary>
		/// Get a copy of the channel list.
		/// </summary>
		/// <returns>A copy of the channel list.</returns>
		public List<Channel> Get() {
			return new List<Channel>(channels);
		}

		/// <summary>
		/// Adds a channel to the channel list.
		/// </summary>
		/// <param name="channelID">The channel's ID.</param>
		/// <param name="type">The channel's <see cref="ChannelDesignation"/>.</param>
		/// <returns>true if channel was added or false if channel already existed (or some other unforeseen error occurs).</returns>
		public bool Add(ulong channelID, ChannelDesignation type) {
			Channel channelToAdd = new Channel(channelID, type);

			if (channels == null) {
				channels = new List<Channel> { channelToAdd };
				return true; // channels doesn't exist so we can add new channel without worry
			}

			if (channels.Contains(channelToAdd))
				return false; // channels already has this exact channel in it

			Remove(channelID);
			channels.Add(channelToAdd);
			return true; // added channel (and removed duplicate copy if necessary)
		}

		/// <summary>
		/// Adds a channel to the channel list and ensure there are no others of the same <see cref="ChannelDesignation"/>.
		/// </summary>
		/// <param name="channelID">The channel's ID.</param>
		/// <param name="type">The channel's <see cref="ChannelDesignation"/>.</param>
		/// <returns>true if channel was added or false if channel already existed (or some other unforeseen error occurs).</returns>
		public bool AddUnique(ulong channelID, ChannelDesignation type) {
			if (channels.Contains(new Channel(channelID, type)))
				return false;

			Remove(type);
			return Add(channelID, type);
		}

		/// <summary>
		/// Use to get the first channel of a certain <see cref="ChannelDesignation"/>.
		/// </summary>
		/// <param name="type">Type to find.</param>
		/// <returns>ID of first channel found or null if no channel found.</returns>
		public ulong? GetFirstChannelByType(ChannelDesignation type) {
			if (channels == null)
				return null;

			foreach (Channel channel in channels)
				if (channel.type == type)
					return channel.id;

			return null;
		}

		/// <summary>
		/// Use to get all channels of a certain <see cref="ChannelDesignation"/>.
		/// </summary>
		/// <param name="type">Type to find.</param>
		/// <returns>A list of all channels found with given <see cref="ChannelDesignation"/>.</returns>
		public List<ulong> GetAllChannelsByType(ChannelDesignation type) {
			List<ulong> found = new List<ulong>();

			if (channels != null)
				foreach (Channel channel in channels)
					if (channel.type == type)
						found.Add(channel.id);

			return found;
		}

		/// <summary>
		/// Use to find out a channel's <see cref="ChannelDesignation"/> from it's ID.
		/// </summary>
		/// <param name="id">Channel's ID.</param>
		/// <returns><see cref="ChannelDesignation"/> of channel or null if channel is not found.</returns>
		public ChannelDesignation? GetChannelType(ulong id) {
			if (channels == null)
				return null;

			foreach (Channel channel in channels)
				if (channel.id == id)
					return channel.type;

			return null;
		}

		public bool Remove(ulong channelID) {
			if (channels == null)
				return false;

			Channel? channelToRemove = null;

			foreach (Channel channel in channels) {
				if (channel.id == channelID) {
					channelToRemove = channel;
					break;
				}
			}

			if (channelToRemove == null)
				return false;
			else
				return channels.Remove((Channel)channelToRemove);
		}

		public bool Remove(ChannelDesignation type) {
			if (channels == null)
				return false;

			List<Channel> channelsToRemove = new List<Channel>();

			foreach (Channel channel in channels) {
				if (channel.type == type)
					channelsToRemove.Add(channel);
			}

			if (channelsToRemove.Count == 0)
				return false;
			else
				foreach (Channel channel in channelsToRemove)
					channels.Remove(channel);

			return true;
		}

		/// <summary>
		/// Clear channels list.
		/// </summary>
		public void Clear() {
			channels = new List<Channel>();
		}
	}

	public struct Channel {
		public ulong id;
		public ChannelDesignation type;

		public Channel(ulong id, ChannelDesignation type) {
			this.id = id;
			this.type = type;
		}
	}
}
