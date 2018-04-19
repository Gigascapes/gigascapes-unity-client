using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gigascapes.SignalProcessing
{
    public class Entity
    {
        public int ID;

        public Vector2 Position;
        public Vector2 Velocity;
        public float Radius;

        public Vector2 SmoothedPosition;
        public Vector2 SmoothedVelocity;
        public float SmoothedRadius;

        public override string ToString()
        {
            return string.Format("(Pos: {0},{1}, Rad: {2}, Vel: {3},{4})", Position.x, Position.y, Radius, Velocity.x, Velocity.y);
        }

        public void Update(Entity newEntity)
        {
            
        }
    }

    public class CalibrationEntity : Entity
    {
        public int Hits;

        public CalibrationEntity(Entity entity)
        {
            ID = entity.ID;
            Position = entity.Position;
            Velocity = entity.Velocity;
            Radius = entity.Radius;
            SmoothedRadius = entity.SmoothedRadius;
            SmoothedPosition = entity.SmoothedPosition;
            SmoothedVelocity = entity.SmoothedVelocity;
            Hits = 1;
        }
    }

    public abstract class Sensor
    {
        public int Index;
        public Vector2 Location;
        public float Direction;
        public float MaxDistanceObserved;
    }

    public class SignalProcessor : MonoBehaviour
    {
        [SerializeField]
        ProcessingMethod ProcessingMethod;

        public delegate void PublishEvent(Entity[] entities);
        public static PublishEvent Publish;

        public delegate void CalibrateEvent(CalibrationUpdate calibrationUpdate);
        public static CalibrateEvent UpdateCalibration;

        public CalibrationPhase CalibrationPhase { get { return ProcessingMethod.CalibrationPhase; }}

		void Awake()
		{
            SensorSignal.OnSignal += HandleSignal;
		}

        void HandleSignal(ISensorOutput signal)
        {
            if (ProcessingMethod == null || signal == null || ProcessingMethod.CalibrationPhase == CalibrationPhase.Uncalibrated)
                return;

            if (ProcessingMethod.CalibrationPhase == CalibrationPhase.Calibrated)
            {
                if (Publish != null)
                {
                    Publish(ProcessingMethod.Process(signal));
                }
            }
            else
            {
                if (UpdateCalibration != null)
                {
                    UpdateCalibration(ProcessingMethod.Calibrate(signal));
                }
            }
        }

        public void StartCalibration()
        {
            if (ProcessingMethod != null)
            {
                ProcessingMethod.StartCalibration();
            }
        }

        public void SetBaseline()
        {
            if (ProcessingMethod != null)
            {
                ProcessingMethod.SetBaseline();
            }
        }

        public void EndCalibration()
        {
            if (ProcessingMethod != null)
            {
                ProcessingMethod.FinishCalibration();
            }
        }
	}
}
