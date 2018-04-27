using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gigascapes.Player
{
    public class ScaleWobbler : MonoBehaviour
    {
        [SerializeField]
        Vector3 Noise;

        Vector3 InitialScale;

        void Awake()
        {
            InitialScale = transform.localScale;
        }

        void Update()
        {
            Undulate();
        }

        void Undulate()
        {
            var modifier = new Vector3(Random.Range(-1 * Noise.x, Noise.x),
                                       Random.Range(-1 * Noise.y, Noise.y),
                                       Random.Range(-1 * Noise.z, Noise.z));
            
            var newRelativeScale = Vector3.one + modifier;

            transform.localScale = new Vector3(InitialScale.x * newRelativeScale.x,
                                               InitialScale.y * newRelativeScale.y,
                                               InitialScale.z * newRelativeScale.z);
        }
    }   
}