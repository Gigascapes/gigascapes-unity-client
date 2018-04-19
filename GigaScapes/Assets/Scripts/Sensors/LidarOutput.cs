using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Gigascapes.SignalProcessing;

namespace Gigascapes.Sensors
{
    public struct LidarBeam
    {
        public float Angle;
        public float Distance;
        public float Quality;
    }

    public class LidarOutput : ISensorOutput 
    {
        public int Index;
        public LidarBeam[] Data;

        public override string ToString()
        {
            var beams = Data.Select(x => string.Format("({0}, {1})", x.Angle, x.Distance));
            return string.Format("Index: {0},\nData:\n{1}", Index, string.Join("\n", beams.ToArray()));
        }
    }
}


