using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beanbattle.Spawn
{
    /// <summary> This will just hold all the spawn points in sceen. </summary>
    public class SpawnPoint : MonoBehaviour
    {
        /// <summary> The spawn point of this point. </summary>
        public Vector3 Position => transform.position;
    }
}