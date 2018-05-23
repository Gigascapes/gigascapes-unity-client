using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gigascapes.SystemDebug;

namespace Gigascapes.SignalProcessing
{
    public interface ISensorOutput
    {
    }

    public class SensorSignal : MonoBehaviour
    {
        public bool IsCalibrated = false;

        public delegate void SignalEvent(ISensorOutput signal);
        public static event SignalEvent OnSignal;

        void Start()
        {
            DebugManager.Instance.DebugCanvas.OnCalibrationStarted += HandleCalibrationStarted;
            DebugManager.Instance.DebugCanvas.OnCalibrationFinished += HandleCalibrationFinished;
        }

        private void OnDestroy()
        {
            DebugManager.Instance.DebugCanvas.OnCalibrationStarted -= HandleCalibrationStarted;
            DebugManager.Instance.DebugCanvas.OnCalibrationFinished -= HandleCalibrationFinished;
        }

        void HandleCalibrationStarted()
        {
            IsCalibrated = false;
        }

        protected void HandleCalibrationFinished()
        {
            IsCalibrated = true;
        }

        protected void Send(ISensorOutput signal)
        {
            if (OnSignal != null)
            {
                OnSignal(signal);
            }
        }
    }
}