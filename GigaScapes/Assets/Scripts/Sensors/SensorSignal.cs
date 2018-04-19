using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gigascapes.SignalProcessing
{
    public interface ISensorOutput
    {
    }

    public class SensorSignal : MonoBehaviour
    {
        public delegate void SignalEvent(ISensorOutput signal);
        public static event SignalEvent OnSignal;

        protected void Send(ISensorOutput signal)
        {
            if (OnSignal != null)
            {
                OnSignal(signal);
            }
        }
    }
}