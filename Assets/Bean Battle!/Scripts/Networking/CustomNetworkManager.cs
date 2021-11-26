using JetBrains.Annotations;

using Mirror;

using System.Collections.Generic;

namespace Beanbattle.Networking
{
	public class CustomNetworkManager : NetworkManager
	{
		/// <summary> A reference to the CustomNetworkManager version of the singleton. </summary>
		public static CustomNetworkManager Instance => singleton as CustomNetworkManager;

		/// <summary> Attempts to find a player using the passed NetID, this can return null. </summary>
		/// <param name="_id"> The NetID of the player that we are trying to find. </param>
		[CanBeNull]
		public static NetworkPlayer FindPlayer(uint _id)
		{
			Instance.players.TryGetValue(_id, out NetworkPlayer player);
			return player;
		}

		/// <summary> Adds a player to the dictionary. </summary>
		public static void AddPlayer([NotNull] NetworkPlayer _player) => Instance.players.Add(_player.netId, _player);

		/// <summary> Removes a player from the dictionary. </summary>
		public static void RemovePlayer([NotNull] NetworkPlayer _player) => Instance.players.Remove(_player.netId);

		/// <summary> A reference to the localplayer of the game. </summary>
		public static NetworkPlayer LocalPlayer
		{
			get
			{
				// If the internal localPlayer instance is null
				if(localPlayer == null)
				{
					// Loop through each player in the game and check if it is a local player
					foreach(NetworkPlayer networkPlayer in Instance.players.Values)
					{
						if(networkPlayer.isLocalPlayer)
						{
							// Set localPlayer to this player as it is the localPlayer
							localPlayer = networkPlayer;
							break;
						}
					}
				}

				// Return the cached local player
				return localPlayer;
			}
		}

		/// <summary> The internal reference to the localPlayer. </summary>
		private static NetworkPlayer localPlayer;

		/// <summary> Whether or not this NetworkManager is the host. </summary>
		public bool IsHost { get; private set; }

		public CustomNetworkDiscovery discovery;

		/// <summary> The dictionary of all connected players using their NetID as the key. </summary>
		private readonly Dictionary<uint, NetworkPlayer> players = new Dictionary<uint, NetworkPlayer>();

		/// <summary>
		/// This is invoked when a host is started.
		/// <para>StartHost has multiple signatures, but they all cause this hook to be called.</para>
		/// </summary>
		public override void OnStartHost()
		{
			IsHost = true;
			// This makes it visible on the network
			discovery.AdvertiseServer();
		}

		/// <summary> This is called when a host is stopped. </summary>
		public override void OnStopHost()
		{
			IsHost = false;
		}

		public bool tablesOnlyMode = false;
		public bool canChangeModes = true;
		public void TurnTablesOnlyOn() => tablesOnlyMode = true;
		public void CantTurnTablesOnlyOnAnymore() => canChangeModes = false;
	}
}