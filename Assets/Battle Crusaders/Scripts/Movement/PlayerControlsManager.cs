using System;
using UnityEngine;

namespace BattleCrusaders.Movement
{
    public class PlayerControlsManager : MonoBehaviour
    {
        private Rigidbody myRigidbody;
        [Tooltip("The players movement speed")] public float speed = 2;
        [Tooltip("The height of the Jump")] public float jumpForce = 4;
        [Tooltip("How fast the fall from the peak")] public float fallMultiplier = 2.5f;
        public float lowJumpMultiplier = 2f;
        public bool isGrounded = false;
        public bool leftground = false;
        public float rememberGroundedFor;
        private float lastTimeGrounded;
        public int defaultAdditionalJumps = 1;
        private int additionalJumps;
        public bool isReady = false;

        [SerializeField] private bool leftButtonDown = false;
        [SerializeField] private bool rightButtonDown = false;

        private bool jumpingNow = false;

        private void Awake()
        {
            // Ok this will set the grounded controller.
            GoundCollider _groundCollider = GetComponentInChildren<GoundCollider>();
            _groundCollider.SetPlayerContoller(this);
            // Sets the rigidbody for this to work.
            myRigidbody = GetComponentInChildren<Rigidbody>();
        }


        void Start()
        {
            additionalJumps = defaultAdditionalJumps;
        }

        void Update()
        {
            if(isReady)
            {
                Move();

                if(Input.GetButtonDown("Jump"))
                {
                    Jump();
                }

                BetterJump();
                CheckIfGrounded();
                Quit();
            }
        }

        private void Quit()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
            }
        }
        
        // This will be for the move left button.
        public void MoveLeftButton()
        {
            leftButtonDown = true;
        }

        // This will be for the move left button release.
        public void MoveLeftButtonUp()
        {
            leftButtonDown = false;
        }

        // This will be for the move right button.
        public void MoveRightButton()
        {
            rightButtonDown = true;
        }

        // This will be for the move right button release.
        public void MoveRightButtonUp()
        {
            rightButtonDown = false;
        }

        public void Move()
        {
            float x = Input.GetAxis("Horizontal");

            if(leftButtonDown || rightButtonDown)
            {
                x = 0;

                if(leftButtonDown)
                {
                    x = +-1;
                }

                if(rightButtonDown)
                {
                    x = +1;
                }
            }

            float moveBy = x * speed;
            myRigidbody.velocity = new Vector3(moveBy, myRigidbody.velocity.y, 0);
        }

        public void Jump()
        {
            if((isGrounded || Time.time - lastTimeGrounded <= rememberGroundedFor) && additionalJumps >= 0)
            {
                myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpForce);
                additionalJumps--;
                jumpingNow = true;
            }
        }

        public void BetterJump()
        {
            if(myRigidbody.velocity.y < 0)
            {
                Vector2 HoriVerti = Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
                myRigidbody.velocity += new Vector3(HoriVerti.x, HoriVerti.y, 0);
                jumpingNow = false;
            }
            else if(myRigidbody.velocity.y > 0 && !Input.GetButtonDown("Jump"))
            {
                Vector2 HoriVerti2 = Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
                myRigidbody.velocity += new Vector3(HoriVerti2.x, HoriVerti2.y, 0);
            }
        }

        void CheckIfGrounded()
        {
            if(isGrounded && !jumpingNow)
            {
                additionalJumps = defaultAdditionalJumps;
                leftground = false;
            }
            else
            {
                if(!leftground)
                {
                    lastTimeGrounded = Time.time;
                }

                leftground = true;
                isGrounded = false;
            }
        }
    }
}