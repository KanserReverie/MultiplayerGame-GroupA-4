using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beanbattle.ThrowObjects
{
    public class TablesOnlyMode : NetworkBehaviour
    {
        // Its now a singleton
    #region Singleton Code
        private static TablesOnlyMode _instance;
        public static TablesOnlyMode Instance
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

        [SyncVar] public bool tablesOnlyMode = false;
        [SyncVar] public bool canChangeModes = true;

        public void TurnTablesOnlyOn() => tablesOnlyMode = true;
        public void CantTurnTablesOnlyOnAnymore() => canChangeModes = false;
    }
}