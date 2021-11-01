using UnityEngine;

namespace Beanbattle.UtilityScripts
{
	/// <summary> This just will turn on or off the object when you load the scene in Awake, TY James. </summary>
	public class TurnOnOrOffOnPlay : MonoBehaviour
	{
		private enum OnOrOffOnPlay { On, Off }

		[Header("Active status of this GameObject on play")] [SerializeField]
		private OnOrOffOnPlay turnOnOrOff = OnOrOffOnPlay.Off;
        
		public void TryActivate() => this.gameObject.SetActive(turnOnOrOff == OnOrOffOnPlay.On);
        
	}
}