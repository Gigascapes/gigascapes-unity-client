using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gigascapes.SignalProcessing;

namespace Gigascapes.Sensors
{
    public class MockLidarSignal : SensorSignal
    {
        [SerializeField] List<MockLidar> LidarSensors;
        [SerializeField] float UpdateHz = 10;

        float UpdateInterval;
        float Timer = 0;

        int LidarIndex = -1;

		void Awake()
		{
            UpdateInterval = 1f / UpdateHz / LidarSensors.Count;
		}

		void Update()
        {
            Timer += Time.deltaTime;
            if (Timer >= UpdateInterval)
            {
                Timer = 0f;
                LidarIndex = (LidarIndex + 1) % LidarSensors.Count;

                var signal = LidarSensors[LidarIndex].GetSensorData();
                Send(signal);
            }
        }
    }

}
