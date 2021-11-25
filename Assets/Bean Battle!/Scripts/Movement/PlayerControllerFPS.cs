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
        //[SerializeField] private float lookSpeed = 2.0f;
        //[SerializeField] private float lookXLimit = 45.0f;
        [SerializeField] private int extraJumpsMax = 2;
        [SerializeField] private int extraJumps = 2;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Vector3 moveDirection = Vector3.zero;
        [HideInInspector] public bool canMove = true;

        [Header("Time to throw a thing")]
        [SerializeField] private GameObject[] gameObjectsToThrow;
        [SerializeField] private Vector3 throwPointVector3;
        [SerializeField] private Quaternion rightThrowRotation;
        [SerializeField] private Quaternion leftThrowRotation;
        [SerializeField] private float myForce;
        [SerializeField] private KeyCode rightThrow = KeyCode.RightArrow;
        [SerializeField] private KeyCode leftThrow = KeyCode.LeftArrow;
        [SerializeField] private float throwTimerMax = 2.0f;
        [SerializeField] private float throwTimer;

        [Header("Time to Die")] 
        [SerializeField] private SpawnPoint[] spawnPoints;


        [Header("UI")] 
        [SerializeField] private bool lockCursor = false;
        
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
            
        #region Z Offset Alignment
            // This should fix the player from moving on the z direction.
            if(transform.position.z != zPosition)
                movementOffSet.z = (zPosition - transform.position.z) * 0.05f;

            characterController.Move(movementOffSet);
        #endregion

        #region Throw Item
            if(throwTimer < throwTimerMax)
                throwTimer += Time.deltaTime;
            else
            {
                if(Input.GetKeyDown(rightThrow))
                {
                    throwTimer = 0;
                    CmdThrow(throwPointVector3, rightThrowRotation, ThrowDirection.Right);
                }
                if(Input.GetKeyDown(leftThrow))
                {
                    throwTimer = 0;
                    CmdThrow(new Vector3(-throwPointVector3.x, throwPointVector3.y, throwPointVector3.z), leftThrowRotation, ThrowDirection.Left);
                }
            }
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

            if(Input.GetButtonDown("Jump") && canMove)
            {
                if(characterController.isGrounded)
                {
                    moveDirection.y = jumpSpeed;
                    extraJumps--;
                }
                else if(extraJumps > 0)
                {
                    moveDirection.y = jumpSpeed;
                    extraJumps--;
                }
            }
            else
                moveDirection.y = movementDirectionY;
           
            // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
            // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
            // as an acceleration (ms^-2)
            if(!characterController.isGrounded)
                moveDirection.y -= gravity * Time.deltaTime;

            // Move the controller
            characterController.Move(moveDirection * Time.deltaTime);
        #endregion

            if(characterController.isGrounded)
                extraJumps = extraJumpsMax;
        }

        private void OnControllerColliderHit(ControllerColliderHit _collision)
        {
            if(_collision.gameObject.layer == LayerMask.NameToLayer("Objects to Throw"))
            {
                print("Hit from player");
        
                // Think not needed.
                // if(!isLocalPlayer)
                //     return;
        
                // // Change to this code for the 2 player thing.
                // int allPlayers = FindObjectsOfType<PlayerControllerFPS>().Length;
                // if(allPlayers < 2)
                // {
                //     spawnPoints = FindObjectsOfType<SpawnPoint>();
                //     ResetPosition(spawnPoints[Random.Range(0, spawnPoints.Length)].gameObject.transform.position, spawnPoints[Random.Range(0, spawnPoints.Length)].gameObject.transform.rotation);
                // }
                
                spawnPoints = FindObjectsOfType<SpawnPoint>();
                ResetPosition(spawnPoints[Random.Range(0, spawnPoints.Length)].gameObject.transform.position, spawnPoints[Random.Range(0, spawnPoints.Length)].gameObject.transform.rotation);

                NetworkServer.Destroy(_collision.gameObject);
                Destroy(_collision.gameObject);
            }
        }

        /// <summary>
        /// Reset the location of this Player.
        /// </summary>
        /// <param name="_position"> Position to move to. </param>
        /// <param name="_rotation"> New rotation of the Player. </param>
        //[Command]
        public void ResetPosition(Vector3 _position, Quaternion _rotation)
        {
            characterController.enabled = false;
            print("Position = " + transform.position + "Rotation = " + transform.rotation);
            gameObject.transform.position = _position;
            gameObject.transform.rotation = _rotation;
            print("Position = " + transform.position + "Rotation = " + transform.rotation);
            characterController.enabled = true;
            moveDirection = new Vector3(0, 0, 0);
        }

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
            
            throwRigidbody.velocity = characterController.velocity;
            
            if(_direction == ThrowDirection.Right)
            {
                throwRigidbody.AddForce(myForce, myForce, Random.Range(0, 10f), ForceMode.Impulse);
            }
            if(_direction == ThrowDirection.Left)
            {
                throwRigidbody.AddForce(-myForce, myForce, -Random.Range(0f, 10f), ForceMode.Impulse);
            }
            throwRigidbody.AddTorque(Random.Range(myForce, -myForce),Random.Range(myForce, -myForce),Random.Range(myForce, -myForce),ForceMode.Impulse);
            throwTimer = 0f;
            NetworkServer.Spawn(newThrowObject);
        }
        
        //[ClientRpc]
        private void RpcThrow(Vector3 _position, Quaternion _rotation, ThrowDirection _direction)
        {
            GameObject newThrowObject = Instantiate(gameObjectsToThrow[Random.Range(0,gameObjectsToThrow.Length)], transform.localPosition + _position, _rotation);
            NetworkServer.Spawn(newThrowObject);
            Rigidbody throwRigidbody = newThrowObject.GetComponent<Rigidbody>();
            
            throwRigidbody.velocity = characterController.velocity.y > 0 ? characterController.velocity : new Vector3(characterController.velocity.x, 0, 0);

            if(_direction == ThrowDirection.Right)
            {
                throwRigidbody.AddForce(myForce, myForce, Random.Range(0, 10f), ForceMode.Impulse);
            }
            if(_direction == ThrowDirection.Left)
            {
                throwRigidbody.AddForce(-myForce, myForce, -Random.Range(0f, 10f), ForceMode.Impulse);
            }
            throwRigidbody.AddTorque(Random.Range(myForce, -myForce),Random.Range(myForce, -myForce),Random.Range(myForce, -myForce),ForceMode.Impulse);
            throwTimer = 0f;
            NetworkServer.Spawn(newThrowObject);
        }
        
        private enum ThrowDirection { Right, Left }
    }
}