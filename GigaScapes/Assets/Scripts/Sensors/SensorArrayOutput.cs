using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gigascapes.SignalProcessing;

namespace Gigascapes.Sensors
{
    public class SensorArrayOutput : ISensorOutput
    {
        public float[] LaserArray1;
        public float[] LaserArray2;
        public float[] SonicArray;
    }
}
