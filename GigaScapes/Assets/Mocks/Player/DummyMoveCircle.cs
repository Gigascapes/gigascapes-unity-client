using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gigascapes.Player
{
    public class DummyMoveCircle : MonoBehaviour
    {
        [SerializeField]
        float MovementForce = 1;

        [SerializeField]
        float Period = 3f;

        Rigidbody Rigidbody;
        float Timer = 0f;

        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            Timer += Time.deltaTime;
            var angle = Mathf.PI * 2f * Timer / Period;
            Rigidbody.AddForce((Vector3.forward * Mathf.Sin(angle) + Vector3.right * Mathf.Cos(angle)) * MovementForce);
        }
    }
}