using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Gigascapes.Sensors;

namespace Gigascapes.SignalProcessing
{
    public class Wall
    {
        public Vector2 Direction;
        public Vector2 Anchor;
    }

    public class LidarSensor : Sensor
    {
    }

    public enum CalibrationPhase
    {
        Uncalibrated,
        Calibrating,
        Calibrated
    }

    public class LidarProcessing : ProcessingMethod
    {
        [SerializeField]
        protected float GroupingTolerance = 0.05f;

        [SerializeField]
        protected float MinimumBeamQuality;

        [SerializeField]
        protected float MinimumEntityRadius = 0.3f;

        [SerializeField]
        protected float MaximumEntityRadius= 1f;

        float Left;
        float Right;
        float Front;
        float Back;

        public override CalibrationUpdate Calibrate(ISensorOutput data)
		{
            var lidarData = data as LidarOutput;
            if (lidarData == null)
            {
                Debug.LogError("Lidar data received is not valid");
                return new CalibrationUpdate();
            }

            var mockLidarData = data as OmniscientLidarOutput;
            if (mockLidarData != null)
            {
                if (!Sensors.ContainsKey(lidarData.Index))
                {
                    Sensors.Add(lidarData.Index, new LidarSensor
                    {
                        Index = lidarData.Index,
                        Location = mockLidarData.Location,
                        Direction = mockLidarData.Direction,
                        MaxDistanceObserved = mockLidarData.MaxRange,
                    });
                }
                else
                {
                    Sensors[lidarData.Index].Location = mockLidarData.Location;
                    Sensors[lidarData.Index].Direction = mockLidarData.Direction;
                }
                Left = mockLidarData.TopLeft.x;
                Right = mockLidarData.BottomRight.x;
                Front = mockLidarData.TopLeft.y;
                Back = mockLidarData.BottomRight.y;
            }

            if (!Sensors.ContainsKey(lidarData.Index))
            {
                var newSensor = new LidarSensor
                {
                    Index = lidarData.Index,
                    Location = Vector2.zero,
                    MaxDistanceObserved = 1f
                };
                Sensors.Add(lidarData.Index, newSensor);
            }
            var sensor = Sensors[lidarData.Index];
            var frameEntities = CalculateFrameEntities(lidarData);

            MatchAndUpdateCalibrationBlackList(sensor, frameEntities);
            return new CalibrationUpdate { Sensor = sensor, Entities = CalibrationInfo[sensor].BlackList.Cast<Entity>().ToArray(), Phase = CalibrationPhase };
        }

		public override Entity[] Process(ISensorOutput data)
        {
            var lidarData = data as LidarOutput;
            if (lidarData == null)
                return Entities.ToArray();
            
            var frameEntities = CalculateFrameEntities(lidarData);
            ProcessFrameEntities(frameEntities);

            //Debug.LogWarningFormat("Entities: {0}", string.Join("\n", Entities.Select(x => x.ToString()).ToArray()));
            return Entities.ToArray();
        }

        protected Entity[] CalculateFrameEntities(LidarOutput lidarData)
        {
            if (!Sensors.ContainsKey(lidarData.Index))
            {
                return Entities.ToArray();
            }

            var entities = new List<Entity>();

            var sensor = Sensors[lidarData.Index];
            var dataLength = lidarData.Data.Length;

            var startingIndex = 0;
            while (startingIndex < dataLength 
                   && lidarData.Data[startingIndex].Quality < MinimumBeamQuality)
            {
                startingIndex++;
            }
            if (startingIndex == dataLength)
                return Entities.ToArray();

            var startingBeam = lidarData.Data[startingIndex];
            var startingPosition = GetWorldPosition(sensor, startingBeam);
            var length = 1;
            var lastDist = startingBeam.Distance;

            for (var i = startingIndex + 1; i < dataLength; i++)
            {
                var beam = lidarData.Data[i];
                if (beam.Quality < MinimumBeamQuality)
                    continue;

                if (beam.Distance > sensor.MaxDistanceObserved)
                {
                    sensor.MaxDistanceObserved = beam.Distance;
                }

                var relativeDistanceDelta = Mathf.Abs(beam.Distance - lastDist) / (IsCalibrated ? Front - Back : 6f);
                if (i == dataLength - 1 // Last element must create cluster
                    || relativeDistanceDelta > GroupingTolerance)
                {
                    var position = GetAveragePosition(sensor, lidarData.Data, startingIndex, length);
                    var displacement = position - sensor.Location;

                    var radius = (position - startingPosition).magnitude;
                    position += radius * displacement.normalized;
                    var normalizedPos = IsCalibrated ? GetNormalizedPosition(position, Left, Right, Front, Back) : position;
                    var normalizedRad = radius / (IsCalibrated ? Right - Left : 6f);

                    var newEntity = new Entity
                    {
                        Position = normalizedPos,
                        Radius = normalizedRad
                    };

                    var isAtDetectionEdge = IsCalibrated && (normalizedPos.x < 0 || normalizedPos.x > 1 || normalizedPos.y < 0 || normalizedPos.y > 1);
                    var isInBlackList = EntityMatchInBlackList(newEntity);
                    var isRelevantSize = normalizedRad > MinimumEntityRadius && normalizedRad < MaximumEntityRadius;
                    if (isRelevantSize && !isAtDetectionEdge && !isInBlackList)
                    {
                        entities.Add(newEntity);
                    }
                    else
                    {
                        Debug.LogWarningFormat("Entity ignored because {0}", isAtDetectionEdge ? "Outside field of play" : !isRelevantSize ? "Irrelevant size" : "On black list");
                        Debug.LogWarningFormat("Position: {0}, {1}", normalizedPos.x, normalizedPos.y);
                    }

                    startingIndex = i;
                    length = 1;
                    startingPosition = GetWorldPosition(sensor, lidarData.Data[i]);
                }
                else
                {
                    length++;
                }

                lastDist = beam.Distance;
            }
            Debug.LogFormat("Frame Entities: {0}", string.Join("\n", entities.Select(x => x.ToString()).ToArray()));
            return entities.ToArray();
        }

        protected static Vector2 GetAveragePosition(Sensor sensor, LidarBeam[] beams, int startingIndex, int length)
        {
            var point1 = GetWorldPosition(sensor, beams[startingIndex]);
            var point2 = GetWorldPosition(sensor, beams[startingIndex + length - 1]);
            var output = (point1 + point2) / 2f;

            return output;
        }

        protected static Vector2 GetWorldPosition(Sensor sensor, LidarBeam beam)
        {
            var angleRad = beam.Angle * Mathf.Deg2Rad;
            var output = GetWorldPosition(sensor.Location, sensor.Direction * Mathf.Deg2Rad, angleRad, beam.Distance);

            return output;
        }

        protected static Vector2 GetWorldPosition(Vector2 location, float dirRad, float angleRad, float distance)
        {
            return location + distance * new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        protected static Vector2 GetNormalizedPosition(Vector2 position, float left, float right, float front, float back)
        {
            return new Vector2(
                (position.x - left) / (right - left),
                (position.y - back) / (front - back)
            );
        }
    }
}
