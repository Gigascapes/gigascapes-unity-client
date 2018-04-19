using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gigascapes.SignalProcessing;

namespace Gigascapes.SystemDebug
{
    public class CalibrationVisualizer : MonoBehaviour
    {
        void Start()
        {
            SignalProcessor.UpdateCalibration += HandleCalibrationUpdate;
        }

        void HandleCalibrationUpdate(CalibrationUpdate update)
        {
            
        }
    }   
}