using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

namespace Beanbattle.Networking
{
	public delegate void SceneLoadedDelegate(Scene _scene);
	public class NetworkSceneManager : NetworkBehaviour
	{
		public void LoadNetworkScene(string _scene)
		{
			if(isLocalPlayer)
			{
				CmdLoadNetworkScene(_scene);
			}
		}

		[Command] public void CmdLoadNetworkScene(string _scene) => RpcLoadNetworkScene(_scene);
		[ClientRpc] public void RpcLoadNetworkScene(string _scene) => LoadScene(_scene, _loadedScene => SceneManager.SetActiveScene(_loadedScene));

		public void LoadScene(string _sceneName, SceneLoadedDelegate _onSceneLoaded = null) => StartCoroutine(LoadScene_CR(_sceneName, _onSceneLoaded));

		private IEnumerator LoadScene_CR(string _sceneName, SceneLoadedDelegate _onSceneLoaded = null)
		{
			yield return SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);
            
			_onSceneLoaded?.Invoke(SceneManager.GetSceneByName(_sceneName));
		}
	}
}