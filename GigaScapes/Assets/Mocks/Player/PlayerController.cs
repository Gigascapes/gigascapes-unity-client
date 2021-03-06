﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gigascapes.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        float MovementForce = 1;

        Rigidbody2D Rigidbody;

        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            Rigidbody.AddForce((Vector3.up * Input.GetAxis("Vertical") + Vector3.right * Input.GetAxis("Horizontal")) * MovementForce);
        }
    }
}
