using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace Beanbattle.Menu
{
	public class MenuHandler : MonoBehaviour
	{
		public void ChangeScene(int sceneIndex)
		{
			SceneManager.LoadScene(sceneIndex);

		}
	}
}