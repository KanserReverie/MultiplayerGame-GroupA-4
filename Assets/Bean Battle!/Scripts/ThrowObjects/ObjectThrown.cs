using BattleCrusaders.Movement;
using Beanbattle.Spawn;
using Mirror;
using UnityEngine;

namespace Beanbattle.ThrowObjects
{
    /// <summary>
    /// This script is attached to any object thrown.
    /// </summary>
    public class ObjectThrown : NetworkBehaviour
    {
        [SerializeField] private float lifeTimer = 2.1f;
        [SerializeField] private Rigidbody thisRigidbody;

        [SyncVar] public Vector3 currentPosition = Vector3.zero;
        [SyncVar] public Quaternion currentRotation = Quaternion.identity;

        [Header("Time to Die")] 
        [SerializeField] private SpawnPoint[] spawnPoints;

        [ClientRpc] private void RpcSyncPositionWithClients(Vector3 positionToSync)
        {
            currentPosition = positionToSync;
        }

        [ClientRpc] private void RpcSyncRotationWithClients(Quaternion rotationToSync)
        {
            currentRotation = rotationToSync;
        }

        private void Update()
        {
            if(isServer)
            {
                RpcSyncPositionWithClients(this.transform.position);
                RpcSyncRotationWithClients(this.transform.rotation);
                lifeTimer -= Time.deltaTime;

                if(lifeTimer < 0)
                {
                    print("Destroy book on time");
                    NetworkServer.Destroy(gameObject);
                    Destroy(this);
                }
            }
        }

        private void OnCollisionEnter(Collision _collision)
        {
            if(_collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                print("Hit from Object - " + gameObject.name);
                // // Change to this code for the 2 player thing.
                // int allPlayers = FindObjectsOfType<PlayerControllerFPS>().Length;
                // if(allPlayers < 2)
                // {
                //     spawnPoints = FindObjectsOfType<SpawnPoint>();
                //     _collision.gameObject.GetComponent<PlayerControllerFPS>().ResetPosition(spawnPoints[Random.Range(0, spawnPoints.Length)].gameObject.transform.position, spawnPoints[Random.Range(0, spawnPoints.Length)].gameObject.transform.rotation);
                // }

                spawnPoints = FindObjectsOfType<SpawnPoint>();
                _collision.gameObject.GetComponent<PlayerControllerFPS>().ResetPosition(spawnPoints[Random.Range(0, spawnPoints.Length)].gameObject.transform.position, spawnPoints[Random.Range(0, spawnPoints.Length)].gameObject.transform.rotation);
                NetworkServer.Destroy(gameObject);
                Destroy(gameObject);
            }
        }

        private void LateUpdate()
        {
            if(!isServer)
            {
                this.transform.position = currentPosition;
                this.transform.rotation = currentRotation;
            }
        }
    }
}