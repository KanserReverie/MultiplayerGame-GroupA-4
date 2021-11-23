using UnityEngine;
using Mirror;

namespace BattleCrusaders.Movement
{
    public class GoundCollider : NetworkBehaviour
    {
        // This is the controller that will move the player
        private PlayerControlsManager myplayerControlsManager;
        
        // This will get and set the movement script
        public void SetPlayerContoller(PlayerControlsManager _newPlayerControlsManager) 
            => myplayerControlsManager = _newPlayerControlsManager;
        public void Update()
        {
            // If we are not the main client dont run this method.
            if(!isLocalPlayer)
                return;
            
            GroundCheck();
        }
        
        void GroundCheck()
        {
            RaycastHit hit;
            float distance = 0.3f;
            Vector3 dir = new Vector3(0, -1);

            int layerMask = LayerMask.GetMask($"NormalObjects");
            
            if(!Physics.Raycast(transform.position, dir, out hit, distance,~layerMask))
            {
                myplayerControlsManager.isGrounded = false;
            }
            else
            {
                myplayerControlsManager.isGrounded = true;
            }
        }
    }
}