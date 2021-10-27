using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Beanbattle.Player;

namespace Beanbattle.Networking
{
    [RequireComponent(typeof(PlayerController))]
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
            PlayerController controller = gameObject.GetComponent<PlayerController>();
            controller.enabled = isLocalPlayer;
        }
    }
}