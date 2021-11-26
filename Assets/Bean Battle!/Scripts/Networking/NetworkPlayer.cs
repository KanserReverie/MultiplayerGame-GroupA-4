using BattleCrusaders.Movement;
using UnityEngine;
using Mirror;

namespace Beanbattle.Networking
{
	[RequireComponent(typeof(PlayerControllerFPS))]
	public class NetworkPlayer : NetworkBehaviour
	{
		// This is run via the network starting and the player connecting...
		// NOT Unity
		// It is run when the object is spawned via the networking system NOT when Unity
		// instantiates the object.
		public override void OnStartClient()
		{
			// This will run REGARDLESS if we are the local or remote player.
			// (isLocalPlayer) is true if this object is the client's local player otherwise it's false.
			PlayerControllerFPS Controller = gameObject.GetComponent<PlayerControllerFPS>();
			Controller.enabled = isLocalPlayer;

			CustomNetworkManager.AddPlayer(this);
		}

		public override void OnStopClient()
		{
			CustomNetworkManager.RemovePlayer(this);
		}
	}
}