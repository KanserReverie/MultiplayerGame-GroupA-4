using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace BattleCrusaders.Movement
{
    [RequireComponent(typeof(CharacterController))] 
    public class PlayerControllerFPS : NetworkBehaviour
    {
        public float walkingSpeed = 7.5f;
        public float runningSpeed = 11.5f;
        public float jumpSpeed = 8.0f;
        public float gravity = 20.0f;
        public float lookSpeed = 2.0f;
        public float lookXLimit = 45.0f;
        public int extraJumpsCount = 2;
        private int extraJumps = 2;
        private CharacterController characterController;
        private Vector3 moveDirection = Vector3.zero;

        [HideInInspector] public bool canMove = true;

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
    }
}