using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beanbattle.Spawn
{
    public class SpawnManager : MonoBehaviour
    {
        // Its now a singleton
    #region Singleton Code
        private static SpawnManager _instance;
        public static SpawnManager Instance
        {
            get { return _instance; }
        }

        private void Awake()
        {
            if(_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
    #endregion

        // A list of all the spawn points
        private SpawnPoint[] allSpawnPoints;
        
        // A list of all the spawn points
        private List<GameObject> allplayers = new List<GameObject>();

        private void Start()
        {
            // FindObjectsOfType gets every instance of this component in the scene
            allSpawnPoints = GameObject.FindObjectsOfType<SpawnPoint>();

            // Get a list of all players.
            // Get a list of all spawn points
        }

        // Start is called before the first frame update
        public void PlayerDeath()
        {
            // Delete all other Game Objects in Scene.
            // Spawn players at spawn points (or just move them to points)
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}