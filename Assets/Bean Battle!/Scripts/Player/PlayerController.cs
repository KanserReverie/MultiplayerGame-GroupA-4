using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beanbattle.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float speed = 2f;

        // Update is called once per frame
        void Update()
        {
            transform.position += transform.right * Time.deltaTime * speed * Input.GetAxis("Horizontal");
        }
    }
}