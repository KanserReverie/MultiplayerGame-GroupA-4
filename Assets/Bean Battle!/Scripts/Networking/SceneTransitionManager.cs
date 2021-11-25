using Mirror;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Beanbattle;

namespace Beanbattle.Networking
{
    /// <summary> This will change the game scenes over for mirror. </summary>
    public class SceneTransitionManager : MonoBehaviour
    {
        // Its now a singleton
    #region Singleton Code
        private static SceneTransitionManager _instance;
        public static SceneTransitionManager Instance
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
        
        //[SerializeField, SceneField] private String allScenes;

        [SerializeField, SceneField] private string[] allScenes;
        public void ChangeToRandomScene()
        {
            if(allScenes != null)
            {
                String nextScene = allScenes[Random.Range(0, allScenes.Length)];
                NetworkManager.singleton.ServerChangeScene(nextScene);
            }
        }
    }
}