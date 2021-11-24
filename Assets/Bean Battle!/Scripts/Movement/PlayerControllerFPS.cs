using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace BattleCrusaders.Movement
{
    [RequireComponent(typeof(CharacterController))] 
    public class PlayerControllerFPS : NetworkBehaviour
    {
        [SerializeField] private float walkingSpeed = 7.5f;
        [SerializeField] private  float runningSpeed = 11.5f;
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
        [SerializeField] private Vector3 rightThrowPoint;
        [SerializeField] private Quaternion rightThrowRotation;
        [SerializeField] private GameObject gun;

        [HideInInspector] public bool canMove = true;
        private float zPosition = 0f;
        private Vector3 movementOffSet;

        void Start()
        {
            characterController = GetComponent<CharacterController>();

            // Lock cursor
            // Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = false;
        }

        void Update()
        {
            // If we are not the main client dont run this method.
            if (!isLocalPlayer)
                return;

            // This should fix the player from moving on the z direction.
            if(transform.position.z != zPosition)
                movementOffSet.z = (zPosition - transform.position.z) * 0.05f;
            characterController.Move (movementOffSet);
            
            if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                CmdThrow(rightThrowPoint, rightThrowRotation*Quaternion.LookRotation(rightThrowPoint), rightThrowPoint);
            }
            
            // We are grounded, so recalculate move direction based on axes
            //Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            // Press Left Shift to run
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            //float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
            float movementDirectionY = moveDirection.y;
            moveDirection = (right * curSpeedY);

            if(Input.GetButtonDown("Jump") && canMove && (characterController.isGrounded || extraJumps > 0))
            {
                moveDirection.y = jumpSpeed;
                extraJumps--;
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }

            if(extraJumps != extraJumpsCount && characterController.isGrounded)
                extraJumps = extraJumpsCount;

            // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
            // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
            // as an acceleration (ms^-2)
            if(!characterController.isGrounded)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }

            // Move the controller
            characterController.Move(moveDirection * Time.deltaTime);
        }

        [Command]
        private void CmdThrow(Vector3 _position, Quaternion _rotation, Vector3 _forward)
        {
            throw new System.NotImplementedException();
        }
    }
}