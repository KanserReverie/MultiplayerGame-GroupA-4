using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkGame
{
	public class ThrowBox : MonoBehaviour
	{
		// Just makes this enemy move forward. 
		// Will be changed to a box.
		private void Update()
		{
			transform.position += transform.forward * Time.deltaTime * 5f;
		}
	}
}