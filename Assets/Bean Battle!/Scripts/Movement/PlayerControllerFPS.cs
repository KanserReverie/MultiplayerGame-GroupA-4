using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Beanbattle.Spawn;

namespace BattleCrusaders.Movement
{
    [RequireComponent(typeof(CharacterController))] public class PlayerControllerFPS : NetworkBehaviour
    {
        [SerializeField] private float walkingSpeed = 7.5f;
        [SerializeField] private float runningSpeed = 11.5f;
        [SerializeField] private float jumpSpeed = 8.0f;
        [SerializeField] private float gravity = 20.0f;
        [SerializeField] private float lookSpeed = 2.0f;
        [SerializeField] private float lookXLimit = 45.0f;
        [SerializeField] private int extraJumpsCount = 2;
        private int extraJumps = 2;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Vector3 moveDirection = Vector3.zero;

        [Header("Time to throw a thing")]
        [SerializeField] private GameObject[] gameObjectsToThrow;
        [SerializeField] private Vector3 throwPointVector3;
        [SerializeField] private Quaternion rightThrowRotation;
        [SerializeField] private Quaternion leftThrowRotation;
        [SerializeField] private float myForce;
        [SerializeField] private KeyCode rightThrow = KeyCode.RightArrow;
        [SerializeField] private KeyCode leftThrow = KeyCode.LeftArrow;

        [HideInInspector] public bool canMove = true;
        
        [Header("Time to Die")] 
        [SerializeField] private SpawnPoint[] spawnPoints;


        [Header("UI")] 
        [SerializeField] private bool lockCursor = false;
        [SerializeField] private bool resetTransform000;
        
        private float zPosition = 0f;
        private Vector3 movementOffSet;

        void Start()
        {
            characterController = GetComponent<CharacterController>();
            spawnPoints = FindObjectsOfType<SpawnPoint>();
        }

        private void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()
        {
            // Lock the cursor if you want to.
            if(lockCursor) 
                LockCursor();

            // If we are not the main client dont run this method.
            if(!isLocalPlayer)
                return;

        #region Z Offset Alignment
            // This should fix the player from moving on the z direction.
            if(transform.position.z != zPosition)
                movementOffSet.z = (zPosition - transform.position.z) * 0.05f;

            characterController.Move(movementOffSet);
        #endregion

        #region Throw Item
            if(Input.GetKeyDown(rightThrow))
                CmdThrow(throwPointVector3, rightThrowRotation, ThrowDirection.Right);

            if(Input.GetKeyDown(leftThrow))
                CmdThrow(new Vector3(-throwPointVector3.x, throwPointVector3.y, throwPointVector3.z), leftThrowRotation, ThrowDirection.Left);
        #endregion
            
        #region Move Player
            // We are grounded, so recalculate move direction based on axes.
            Vector3 right = transform.TransformDirection(Vector3.right);
            // Press Left Shift to run.
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            //float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
            float movementDirectionY = moveDirection.y;
            moveDirection = (right * curSpeedY);
            
            if(Input.GetButtonDown("Jump") && canMove && (characterController.isGrounded || extraJumps > 0)) {
                moveDirection.y = jumpSpeed;
                extraJumps--; }
            else
                moveDirection.y = movementDirectionY;

            if(extraJumps != extraJumpsCount && characterController.isGrounded)
                extraJumps = extraJumpsCount;
            
            // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
            // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
            // as an acceleration (ms^-2)
            if(!characterController.isGrounded)
                moveDirection.y -= gravity * Time.deltaTime;
            
            // Move the controller
            characterController.Move(moveDirection * Time.deltaTime);
        #endregion
        }
        
        private void OnCollisionStay(Collision _collision)
        {
            if(!isLocalPlayer)
                return;
            
            if(_collision.gameObject.layer == LayerMask.NameToLayer("Objects to Throw"))
            {
                if(resetTransform000)
                {
                    spawnPoints = FindObjectsOfType<SpawnPoint>();
                    CmdResetPosition(
                        spawnPoints[Random.Range(0, spawnPoints.Length)].gameObject.transform.position, 
                        spawnPoints[Random.Range(0, spawnPoints.Length)].gameObject.transform.rotation);
                }
                else
                {
                    CmdResetPosition(new Vector3(0,0,0), new Quaternion(1,0,0,0));
                }
                
                print("dead");
                Destroy(_collision.gameObject);
            }
        }

        /// <summary>
        /// Reset the location of this Player.
        /// </summary>
        /// <param name="_position"> Position to move to. </param>
        /// <param name="_rotation"> New rotation of the Player. </param>
        [Command]
        private void CmdResetPosition(Vector3 _position, Quaternion _rotation)
        {
            print("Position = " + transform.position + "Rotation = " + transform.rotation);
            gameObject.transform.position = _position;
            gameObject.transform.rotation = _rotation;
            print("Position = " + transform.position + "Rotation = " + transform.rotation);
        }
        // ^^^Might need to add back ^^^
        // [ClientRpc]
        // private void rpcResetPosition(Vector3 _position, Quaternion _rotation)
        // {
        //     
        // }
        
        
        /// <summary>
        /// Throws an object.
        /// </summary>
        /// <param name="_position"> Place to start throwing the Object. </param>
        /// <param name="_rotation"> Rotation of the throw. </param>
        [Command] private void CmdThrow(Vector3 _position, Quaternion _rotation, ThrowDirection _direction)
        {
            GameObject newThrowObject = Instantiate(gameObjectsToThrow[Random.Range(0,gameObjectsToThrow.Length)], transform.localPosition + _position, _rotation);
            NetworkServer.Spawn(newThrowObject);
            Rigidbody throwRigidbody = newThrowObject.GetComponent<Rigidbody>();
            if(_direction == ThrowDirection.Right)
                newThrowObject.GetComponent<Rigidbody>().AddForce(myForce , myForce, Random.Range(0,10f), ForceMode.Impulse);
            if(_direction == ThrowDirection.Left)
                newThrowObject.GetComponent<Rigidbody>().AddForce(-myForce , myForce, -Random.Range(0f,10f), ForceMode.Impulse);
            
            throwRigidbody.AddTorque(Random.Range(myForce, -myForce),Random.Range(myForce, -myForce),Random.Range(myForce, -myForce),ForceMode.Impulse);
        }
        private enum ThrowDirection { Right, Left }
    }
}