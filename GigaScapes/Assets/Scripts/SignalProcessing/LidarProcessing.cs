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

    public class LidarProcessingInfo
    {
        public Sensor Sensor;
        public LidarBeam Beam;
        public Vector2 StartingPosition;
        public int StartingIndex;
        public int ReadingLength;
        public LidarOutput Data;
        public float LastDist;
        public bool WasInBlackList;
        public bool IsInBlackList;
        
        LidarProcessing ProcessingMethod;

        public LidarProcessingInfo (LidarProcessing method)
        {
            ProcessingMethod = method;
        }

        public Entity[] Process()
        {
            var output = new List<Entity>();
            for (var i = StartingIndex + 1; i < Data.Data.Length; i++)
            {
                Beam = Data.Data[i];
                if (Beam.Quality < ProcessingMethod.MinimumBeamQuality)
                    continue;

                var worldPos = LidarProcessing.GetWorldPosition(Sensor, Beam);
                var isInBlacklist = ProcessingMethod.IsPointInBlackList(worldPos);
                if (!isInBlacklist)
                {
                    //Debug.LogFormat("Sensor Location: ({0},{1}), dirRad: {2}, angleRad: {3}, distance: {4}", Sensor.Location.x, Sensor.Location.y, Sensor.Direction * Mathf.Deg2Rad, Beam.Angle * Mathf.Deg2Rad, Beam.Distance);
                    //Debug.LogFormat("World pos: ({0},{1}) - [{2},{3}] [{4},{5}]", worldPos.x, worldPos.y, ProcessingMethod.Left, ProcessingMethod.Right, ProcessingMethod.Back, ProcessingMethod.Front);
                }
                var relativeDistanceDelta = Mathf.Abs(Beam.Distance - LastDist) / (ProcessingMethod.IsCalibrated ? ProcessingMethod.Front - ProcessingMethod.Back : 6f);
                if (i == Data.Data.Length - 1 // Last element must create cluster
                    || relativeDistanceDelta > ProcessingMethod.GroupingTolerance
                    || isInBlacklist && !WasInBlackList)
                {
                    if (!WasInBlackList)
                    {
                        var newEntity = TieOffReading();
                        if (newEntity != null)
                            output.Add(newEntity);
                    }

                    StartingIndex = i;
                    ReadingLength = 1;
                    StartingPosition = worldPos;
                }
                else
                {
                    ReadingLength++;
                }

                WasInBlackList = isInBlacklist;
                LastDist = Beam.Distance;
            }

            return output.ToArray();
        }

        Entity TieOffReading()
        {
            var position = LidarProcessing.GetAveragePosition(Sensor, Data.Data, StartingIndex, ReadingLength);
            var displacement = position - Sensor.Location;

            var radius = (position - StartingPosition).magnitude;
            position += radius * displacement.normalized;
            var normalizedPos = ProcessingMethod.IsCalibrated ? ProcessingMethod.GetNormalizedPosition(position) : position;
            var normalizedRad = radius / (ProcessingMethod.IsCalibrated ? ProcessingMethod.Right - ProcessingMethod.Left : 6f);

            var newEntity = new Entity
            {
                Position = normalizedPos,
                Radius = normalizedRad
            };

            var isAtDetectionEdge = ProcessingMethod.IsCalibrated && (normalizedPos.x < 0 || normalizedPos.x > 1 || normalizedPos.y < 0 || normalizedPos.y > 1);
            var isRelevantSize = normalizedRad > ProcessingMethod.MinimumEntityRadius && normalizedRad < ProcessingMethod.MaximumEntityRadius;
            if (isRelevantSize && !isAtDetectionEdge)
            {
                return newEntity;
            }
            else
            {
                //Debug.LogWarningFormat("Entity ignored because {0}", isAtDetectionEdge ? "Outside field of play" : !isRelevantSize ? "Irrelevant size" : "On black list");
                //Debug.LogWarningFormat("Position: {0}, {1}; Rad: {2}", normalizedPos.x, normalizedPos.y, normalizedRad);
                return null;
            }
        }
    }

    public class LidarProcessing : ProcessingMethod
    {
        [SerializeField]
        public float GroupingTolerance = 0.05f;

        [SerializeField]
        public float MinimumBeamQuality;

        [SerializeField]
        public float MinimumEntityRadius = 0.3f;

        [SerializeField]
        public float MaximumEntityRadius= 1f;

        public float Left;
        public float Right;
        public float Front;
        public float Back;

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
                    //Debug.LogErrorFormat("Setting sensor position: ({0},{1}), direction: {2}", mockLidarData.Location.x, mockLidarData.Location.y, mockLidarData.Direction);
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
            var frameEntities = CalculateCalibrationEntities(lidarData);

            MatchAndUpdateCalibrationBlackList(sensor, frameEntities);
            return new CalibrationUpdate { Sensor = sensor, Entities = CalibrationInfo[sensor].BlackList.Cast<Entity>().ToArray(), Phase = CalibrationPhase };
        }

		public override Entity[] Process(ISensorOutput data)
        {
            //Debug.LogWarning("-----------------");
            var lidarData = data as LidarOutput;
            if (lidarData == null)
            {
                return Entities.ToArray();
            }

            var frameEntities = CalculateFrameEntities(lidarData);
            ProcessFrameEntities(frameEntities);

            return Entities.ToArray();
        }

        protected Entity[] CalculateCalibrationEntities(LidarOutput lidarData)
        {
            if (!Sensors.ContainsKey(lidarData.Index))
            {
                return Entities.ToArray();
            }

            var entities = new List<Entity>();

            var sensor = Sensors[lidarData.Index];
            var dataLength = lidarData.Data.Length;

            for (var i = 0; i < dataLength; i++)
            {
                var beam = lidarData.Data[i];
                if (beam.Quality < MinimumBeamQuality)
                {
                    continue;
                }

                var position = GetWorldPosition(sensor, beam);

                var radius = GroupingTolerance;
                var normalizedPos = IsCalibrated ? GetNormalizedPosition(position) : position;

                var newEntity = new Entity
                {
                    Position = normalizedPos,
                    Radius = radius,
                    RawPosition = position,
                    RawRadius = radius
                };

                entities.Add(newEntity);
            }
            return entities.ToArray();
        }

        protected Entity[] CalculateFrameEntities(LidarOutput lidarData)
        {
            if (!Sensors.ContainsKey(lidarData.Index))
            {
                return Entities.ToArray();
            }

            var info = new LidarProcessingInfo (this)
            {
                Sensor = Sensors[lidarData.Index],
                Data = lidarData,
                StartingIndex = 0,
                WasInBlackList = false
            };

            var dataLength = lidarData.Data.Length;

            while (info.StartingIndex < dataLength 
                   && lidarData.Data[info.StartingIndex].Quality < MinimumBeamQuality)
            {
                info.StartingIndex++;
            }
            if (info.StartingIndex == dataLength)
                return Entities.ToArray();

            info.Beam = lidarData.Data[info.StartingIndex];
            info.StartingPosition = GetWorldPosition(info.Sensor, info.Beam);
            info.ReadingLength = 1;
            info.LastDist = info.Beam.Distance;
            var entities = info.Process();
            
            //Debug.LogFormat("Frame Entities: {0}", string.Join("\n", entities.Select(x => x.ToString()).ToArray()));
            return entities;
        }

        public bool IsPointInBlackList(Vector2 position)
        {
            return EntityMatchInBlackList(new Entity { Position = position, SmoothedVelocity = Vector2.zero });
        }

        public static Vector2 GetAveragePosition(Sensor sensor, LidarBeam[] beams, int startingIndex, int length)
        {
            var point1 = GetWorldPosition(sensor, beams[startingIndex]);
            var point2 = GetWorldPosition(sensor, beams[startingIndex + length - 1]);
            var output = (point1 + point2) / 2f;

            return output;
        }

        public static Vector2 GetWorldPosition(Sensor sensor, LidarBeam beam)
        {
            var angleRad = beam.Angle * Mathf.Deg2Rad;
            var output = GetWorldPosition(sensor.Location, sensor.Direction * Mathf.Deg2Rad, angleRad, beam.Distance);

            return output;
        }

        public static Vector2 GetWorldPosition(Vector2 location, float dirRad, float angleRad, float distance)
        {
            var combinedAngle = (Mathf.PI * 2f - angleRad) + Mathf.PI * 1.5f + dirRad;
            return location + distance * new Vector2(Mathf.Cos(combinedAngle), Mathf.Sin(combinedAngle));
        }

        public Vector2 GetNormalizedPosition(Vector2 position)
        {
            //Debug.LogFormat("Left:{0}, Right:{1}, Front:{2}, Back:{3}", Left, Right, Front, Back);
            return new Vector2(
                (position.x - Left) / (Right - Left),
                (position.y - Back) / (Front - Back)
            );
        }
    }
}
