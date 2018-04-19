using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gigascapes.Sensors
{
    public class MockSensorArray : MonoBehaviour
    {
        [SerializeField] GameObject LaserPrefab;
        [SerializeField] GameObject SonicSensorPrefab;

        [SerializeField] int LaserCount;
        [SerializeField] float LaserMinX;
        [SerializeField] float LaserSpacing;

        MockLaserArray LaserArray = new MockLaserArray();

        void Awake()
        {
            PopulateLasers();
        }

        void PopulateLasers()
        {
            var startingPosition = transform.position - transform.right * transform.localScale.x / 2f;
            for (var i = 0; i < LaserCount; i++)
            {
                var newLaser = Instantiate(LaserPrefab, transform);

                newLaser.layer = gameObject.layer;
                foreach (Transform child in newLaser.transform)
                {
                    child.gameObject.layer = gameObject.layer;
                }

                newLaser.transform.position = startingPosition + i * transform.right * LaserSpacing;
                LaserArray.Lasers.Add(newLaser.GetComponent<MockLaserSensor>());
            }
        }

        public SensorArrayOutput GetSensorData()
        {
            return new SensorArrayOutput
            {
                LaserArray1 = LaserArray.GetSensorData()
            };
        }
    }

    public class MockLaserArray : ISensorArray
    {
        public List<MockLaserSensor> Lasers = new List<MockLaserSensor>();
        public float[] GetSensorData()
        {
            return Lasers.Select(x => x.GetReading()).ToArray();
        }
    }
}