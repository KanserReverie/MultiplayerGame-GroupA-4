using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beanbattle.Menu
{
    public class SPIIIIN : MonoBehaviour
    {
        private Rigidbody rb;
        private float xvalue, yvalue;
        [SerializeField] private Vector3 spin = new Vector3(0.1f, 0.3f, 0);

        [SerializeField] private Vector3 maxSpeed;
        [SerializeField] private Vector3 minSpeed;
        [SerializeField] private bool atMaxSpeed;
        private void Start()
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezePosition;
            atMaxSpeed = false;
        }

        void FixedUpdate()
        {
            if(!atMaxSpeed) rb.AddTorque(spin);

            if(rb.angularVelocity.x > maxSpeed.x)
                if(rb.angularVelocity.y > maxSpeed.y)
                    atMaxSpeed = true;

            if(rb.angularVelocity.x < minSpeed.x)
                if(rb.angularVelocity.y < minSpeed.y)
                    atMaxSpeed = false;
        }
    }
}