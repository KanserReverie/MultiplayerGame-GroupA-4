using System;
using UnityEngine;

namespace Beanbattle.Player
{
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody myRigidBody;
        [SerializeField] private float jumpForce = 4;
        [SerializeField] private float fallMultiplier = 2.5f;
        [SerializeField] private float lowJumpMultiplier = 2f;
        // This if for if the player is touching the ground.
        [SerializeField] private bool isGrounded = false;
        [SerializeField] private bool leftground = false;
        [SerializeField] private float rememberGroundedFor = 1f;
        [SerializeField] private int defaultAdditionalJumps = 3;
        [SerializeField] private float speed = 2f;
        private float lastTimeGrounded;
        
        private int additionalJumps;
        private bool jumpingNow = false;
        
        public void Awake()
        {
            myRigidBody = GetComponent<Rigidbody>();
            isGrounded = true;
        }

        // Update is called once per frame
        void Update()
        {
            Move();

            if(Input.GetButtonDown("Jump"))
            {
                Jump();
            }

            BetterJump();
            CheckIfGrounded();
            //IsGrounded();
            //GroundCheck();
        }

        /// <summary> Jumping isn't good for dogs, but you can have the rest of my milk. </summary>
        private void Jump()
        {
            Debug.Log($"isGrounded = {isGrounded} || Time.time - lastTimeGrounded = {Time.time - lastTimeGrounded} (needs to be <=) {rememberGroundedFor} && aditionalJumps = {additionalJumps}");
            if((isGrounded || Time.time - lastTimeGrounded <= rememberGroundedFor) && additionalJumps >= 0)
            {
                myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, jumpForce);
                additionalJumps--;
                jumpingNow = true;
            }
        }

        /// <summary> Yests this should move the thing. </summary>
        private void Move()
        {
            float x = Input.GetAxis("Horizontal");

            float moveBy = x * speed;
            myRigidBody.velocity = new Vector3(moveBy, myRigidBody.velocity.y, 0);
        }
        
        

        /// <summary> The COOLER jump, jk it just feels better. </summary>
        public void BetterJump()
        {
            if(myRigidBody.velocity.y < 0)
            {
                Vector2 HoriVerti = Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
                myRigidBody.velocity += new Vector3(HoriVerti.x, HoriVerti.y, 0);
                jumpingNow = false;
            }
            else if(myRigidBody.velocity.y > 0 && !Input.GetButtonDown("Jump"))
            {
                Vector2 HoriVerti2 = Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
                myRigidBody.velocity += new Vector3(HoriVerti2.x, HoriVerti2.y, 0);
            }
        }

        private void FixedUpdate()
        {
            isGrounded = false;
        }

        /// <summary>
        /// detects if player is touching the ground. if so, resets players maxJumps count.
        /// </summary>
        /// <param name="other">object of player</param>
        private void OnCollisionStay(Collision other)
        {
            if(other.gameObject.CompareTag($"Ground"))
            {
                isGrounded = true;
            }
        }
        
        /// <summary> This lets the player jump even if they fall off the platform </summary>
        private void CheckIfGrounded()
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
        
    #region Not Working Ground Checks

        /// <summary> Should be checking if the player is touching the ground </summary>
        private void GroundCheck()
        {
            RaycastHit hit;
            float distance = 1.3f;
            
            Vector3 down = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z); 
            
            //isGrounded = Physics.Raycast(transform.position, down, out hit, distance,~layerMask);
            isGrounded = Physics.Raycast(transform.position, down, out hit, distance);
            // Debug.DrawLine(transform.position, down, Color.cyan,1.3f);

            if(isGrounded)
            {
                print("grounded");
            }
            else
            {
                print("ground not hit and away by - " + hit.distance);
            }
        }
        
        /// <summary>
        /// Raycast ground check from the middle not working.
        /// </summary>
        /// <returns> Should return true IF you are on the ground. </returns>
        private bool IsGrounded()
        {
            bool raycastHit = Physics.Raycast(transform.position, Vector3.down, 1.1f);
            Color rayColor;

            rayColor = raycastHit ? Color.green : Color.red;
            
            Debug.DrawRay(transform.position, Vector3.down * 1.1f);
            return raycastHit;
        }
    #endregion
        
    }
}