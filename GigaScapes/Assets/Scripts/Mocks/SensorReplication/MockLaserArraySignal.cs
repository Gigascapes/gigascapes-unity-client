using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gigascapes.SignalProcessing;

namespace Gigascapes.Sensors
{
    public class MockLaserArraySignal : SensorSignal
    {
        [SerializeField] MockSensorArray Array1;
        [SerializeField] MockSensorArray Array2;

        void Update()
        {
            var signal = new SensorArrayOutput();
            if (Array1 != null && Array1.isActiveAndEnabled)
            {
                var array1Data = Array1.GetSensorData();
                signal.LaserArray1 = array1Data.LaserArray1;
                signal.SonicArray = array1Data.SonicArray;
            }
            if (Array2 != null && Array2.isActiveAndEnabled)
            {
                signal.LaserArray2 = Array2.GetSensorData().LaserArray1;
            }

            Send(signal);
        }
    }   
}