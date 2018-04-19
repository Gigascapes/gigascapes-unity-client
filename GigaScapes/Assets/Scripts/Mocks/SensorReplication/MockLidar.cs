using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gigascapes.Sensors
{
    public class OmniscientLidarOutput : LidarOutput
    {
        public Vector2 Location;
        public float Direction;
        public float MaxRange;
        public Vector2 TopLeft;
        public Vector2 BottomRight;
    }

    public class MockLidar : MonoBehaviour
    {
        [SerializeField] int Index;
        [SerializeField] GameObject LaserPrefab;
        [SerializeField] int LaserCount;
        [SerializeField] float RadialMountOffset;
        [SerializeField] float LaserRange;

        [SerializeField] Transform TopLeftMarker;
        [SerializeField] Transform BottomRightMarker;

        MockLidarArray LaserArray = new MockLidarArray();

        void Awake()
        {
            PopulateLasers();
        }

        void PopulateLasers()
        {
            var angleDiff = 360f / (float)LaserCount;
            for (var i = 0; i < LaserCount; i++)
            {
                var angle = i * angleDiff;
                var angleRad = angle * Mathf.Deg2Rad;
                var localPosition = new Vector3(Mathf.Cos(angleRad), 0f, Mathf.Sin(angleRad)) * RadialMountOffset;

                var newLaser = Instantiate(LaserPrefab, transform.position + localPosition, Quaternion.identity, transform);
                newLaser.transform.up = localPosition * -1f;

                SetLayerToSameAsSelf(newLaser);

                var laserComponent = newLaser.GetComponent<MockLaserSensor>();
                laserComponent.MaxRange = LaserRange;

                LaserArray.Lasers.Add(laserComponent);
                LaserArray.Angles.Add(angle);
            }
        }

        void SetLayerToSameAsSelf(GameObject target)
        {
            target.layer = gameObject.layer;
            foreach (Transform child in target.transform)
            {
                child.gameObject.layer = gameObject.layer;
            }
        }

        public LidarOutput GetSensorData()
        {
            return new LidarOutput
            {
                Index = Index,
                Data = LaserArray.GetSensorData()
            };
            /*
            return new OmniscientLidarOutput
            {
                Index = Index,
                Data = LaserArray.GetSensorData(),
                Location = new Vector2(transform.position.x, transform.position.z),
                Direction = transform.rotation.eulerAngles.y,
                MaxRange = LaserRange,
                TopLeft = new Vector2(TopLeftMarker.position.x, TopLeftMarker.position.z),
                BottomRight = new Vector2(BottomRightMarker.position.x, BottomRightMarker.position.z)
            };
            */
        }
    }

    public class MockLidarArray
    {
        public List<MockLaserSensor> Lasers = new List<MockLaserSensor>();
        public List<float> Angles = new List<float>();
        public LidarBeam[] GetSensorData()
        {
            var output = new List<LidarBeam>();
            for (var i = 0; i < Lasers.Count; i++)
            {
                output.Add(new LidarBeam
                {
                    Distance = Lasers[i].GetReading(),
                    Angle = Angles[i],
                    Quality = Random.Range(0f, 10f)
                });
            }
            return output.ToArray();
        }
    }
}
