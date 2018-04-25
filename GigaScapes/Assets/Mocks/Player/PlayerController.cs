using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gigascapes.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        float MovementForce = 1;

        Rigidbody Rigidbody;

        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            Rigidbody.AddForce((Vector3.forward * Input.GetAxis("Vertical") + Vector3.right * Input.GetAxis("Horizontal")) * MovementForce);
        }
    }
}
