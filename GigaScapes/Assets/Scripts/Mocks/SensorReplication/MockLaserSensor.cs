using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gigascapes.Sensors
{
    public class MockLaserSensor : MonoBehaviour, ISensor
    {
        public Vector3 RayDirection;

        public float MaxRange;

        LineRenderer LineRenderer;

        void Start()
        {
            LineRenderer = GetComponentInChildren<LineRenderer>();
            LineRenderer.SetPositions(new Vector3[]
            {
                Vector3.zero,
                RayDirection.normalized * MaxRange
            });
        }

        public float GetReading()
        {
            var output = MaxRange;
            var hit = new RaycastHit();
            if (Physics.Raycast(new Ray(transform.position, transform.TransformDirection(RayDirection)), out hit, MaxRange))
            {
                output = hit.distance;
            }

            SetLaserLength(output);
            return output;
        }

        void SetLaserLength(float value)
        {
            LineRenderer.SetPosition(1, RayDirection.normalized * value);
        }
    }   
}