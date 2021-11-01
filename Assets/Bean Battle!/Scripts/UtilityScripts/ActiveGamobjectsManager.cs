using UnityEngine;

namespace Beanbattle.UtilityScripts
{
	/// <summary> This is the Manager to Turn tagged gameObjects, On/Off in Awake, TY James. </summary>
	public class ActiveGamobjectsManager : MonoBehaviour
	{
		// Start is called before the first frame update
		void Awake()
		{
			TurnOnOrOffOnPlay[] onOffs = FindObjectsOfType<TurnOnOrOffOnPlay>();

			foreach(TurnOnOrOffOnPlay onOff in onOffs)
				onOff.TryActivate();
		}
	}
}