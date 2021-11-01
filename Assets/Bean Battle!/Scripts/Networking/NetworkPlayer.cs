using UnityEngine;
using Mirror;
using Beanbattle.Player;

namespace Beanbattle.Networking
{
	[RequireComponent(typeof(PlayerController))]
	public class NetworkPlayer : NetworkBehaviour
	{
		[SerializeField] private GameObject enemyToSpawn;
		// This will mean it will now be syned when any player starts because the color is a "Sync Var". 
		[SyncVar(hook = nameof(OnSetColor)), SerializeField] private Color cubeColor;

		[SerializeField] private SyncList<float> syncedFloats = new SyncList<float>();
		
		// SyncVarHooks get called in the order the VARIABLES are defined not the functions
		// [SyncVar(hook = "SetX")] public float x;
		// [SyncVar(hook = "SetY")] public float y;
		// [SyncVar(hook = "SetZ")] public float z;
		//
		// [Command]
		// public void CmdSetPosition(float _x, float _y, float _z)
		// {
		//     z = _z;
		//     x = _x;
		//     y = _y;
		// }
		
		private Material cachedMaterial;
		private void OnSetColor(Color _old, Color _new)
		{
			if(cachedMaterial == null)
			{
				cachedMaterial = gameObject.GetComponent<MeshRenderer>().material;
			}

			cachedMaterial.color = _new;
		}
		
		private void Awake()
		{
			
			// This will run REGARDLESS if we are the local or remote player.
		}

		private void Update()
		{
			// First determine if this function is being run on the local player.
			if(isLocalPlayer)
			{
				if(Input.GetKeyDown(KeyCode.Space))
				{
					// Run a function that tells every client to change the colour of this gameObject.
					// The "Random" part is done on the client.
					CmdRandomColorOnClient(Random.Range(0f,1f));
				}
				if(Input.GetKeyDown(KeyCode.Z))
				{
					// Run a function that tells every client to change the colour of this gameObject.
					CmdRandomColorOnServer();
				}
				if(Input.GetKeyDown(KeyCode.E))
				{
					CmdSpawnEnemy();
				}
			}
		}

		[Command] public void CmdSpawnEnemy()
		{
			// You first need to Instantiate...
			GameObject newEnemy = Instantiate(enemyToSpawn);
			// ...and then you can spawn the enemy.
			NetworkServer.Spawn(newEnemy);
		}
		
		// RULES FOR COMMANDS:
		// 1. Cannot return anything.
		// 2. Must follow the correct naming convention: The function name MUST start with "Cmd" exactly like that.
		// 3. The function must have the attribute [Command] found in Mirror namespace.
		// 4. Can only be certain serializable types (see Command in the documentation).
		[Command] public void CmdRandomColorOnClient(float _hue)
		{
			// SyncVar Must be set on the server, otherwise it won't be synced between clients.
			cubeColor = Random.ColorHSV(0, 1, 1, 1, 1, 1);
			
			// This is run on the server.
			// RpcRandomColorOnClient(_hue);
		}

		// RULES FOR RPC:
		// 1. Cannot return anything.
		// 2. Must follow the correct naming convention: The function name MUST start with "Rpc" exactly like that.
		// 3. The function must have the attribute [ClientRpc] found in Mirror namespace.
		// 4. Can only be certain serializable types (see Command in the documentation).
		// [ClientRpc] public void RpcRandomColorOnClient(float _hue)
		// {
		// 	// This is running on every instance of the same object that the client was calling from.
		// 	// i.e. Red GO on Red Client runs Cmd, Red GO on Red, Green and Blue client's run Rpc.
		// 	MeshRenderer rend = gameObject.GetComponent<MeshRenderer>();
		// 	rend.material.color = Color.HSVToRGB(_hue, 1, 1);
		// }

		// This is the same version as "CmdRandomColorOnClient" BUT,
		// the "Random" decision was made on the server NOT client
		[Command] public void CmdRandomColorOnServer()
		{
			// Ever so slightly performant but not really.
			// RpcRandomColorOnClient(Random.Range(0f, 1f));
		}




		// This is run via the network starting and the player connecting...
		// NOT Unity
		// It is run when the object is spawned via the networking system NOT when Unity
		// instantiates the object.
		public override void OnStartLocalPlayer()
		{
			// This is run if we are the local player and NOT a remote player.
		}
		
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

			CustomNetworkManager.AddPlayer(this);
		}

		public override void OnStopClient()
		{
			CustomNetworkManager.RemovePlayer(this);
		}
		
		// This runs when the server starts... ON the server.
		// In the case of a Host-Client situation,
		// this only runs when the HOST launches because the host is the server.
		public override void OnStartServer()
		{
			for(int i = 0; i < 10; i++)
			{
				syncedFloats.Add(Random.Range(0,10));
			}
		}
	}
}