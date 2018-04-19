using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Gigascapes.Sensors;

namespace Gigascapes.SignalProcessing
{
    public enum Direction
    {
        Horizontal,
        Vertical
    }

    public class LaserArrayProcessing : ProcessingMethod
    {
        [SerializeField]
        protected float GroupingTolerance = 0.05f;

        protected Vector2 MaxDistanceObserved = Vector2.zero;

        public override Entity[] Process(ISensorOutput data)
        {
            var sensorArrayData = data as SensorArrayOutput;
            if (sensorArrayData == null)
                return new Entity[]{};

            var arrayOfInterest = sensorArrayData.LaserArray1 ?? sensorArrayData.LaserArray2;
            if (arrayOfInterest.Length == 0)
            {
                return Entities.ToArray();
            }

            var direction = arrayOfInterest == sensorArrayData.LaserArray1 ? Direction.Vertical : Direction.Horizontal;

            return ProcessArray(arrayOfInterest, direction);
        }

        protected virtual Entity[] ProcessArray(float[] arrayOfInterest, Direction direction)
        {
            var frameEntities = CalculateFrameEntities(arrayOfInterest, direction);
            ProcessFrameEntities(frameEntities);

            return Entities.ToArray();
        }

        public static string ArrayToString(float[] array)
        {
            return array == null ? "null" : string.Join(", ", array.Select(x => x.ToString()).ToArray());
        }

        protected virtual void UpdateMaxDistance(float[] array, Direction direction)
        {
            var maxDist = GetMaxDistance(direction);

            foreach (var distance in array)
            {
                if (distance > maxDist)
                {
                    SetMaxDistance(distance, direction);
                }
            }
        }

        protected float GetMaxDistance(Direction direction)
        {
            switch (direction)
            {
                case Direction.Horizontal:
                    return MaxDistanceObserved.x;
                case Direction.Vertical:
                    return MaxDistanceObserved.y;
                default:
                    return 0f;
            }
        }

        protected void SetMaxDistance(float newDist, Direction direction)
        {
            switch (direction)
            {
                case Direction.Horizontal:
                    MaxDistanceObserved.x = newDist;
                    break;
                case Direction.Vertical:
                    MaxDistanceObserved.y = newDist;
                    break;
            }
        }

        protected float[] NormalizeArray(float[] array, Direction direction)
        {
            var maxDist = GetMaxDistance(direction);
            return array.Select(x => x / maxDist).ToArray();
        }

        protected virtual Entity[] CalculateFrameEntities(float[] array, Direction direction)
        {
            var entities = new List<Entity>();
            UpdateMaxDistance(array, direction);

            var normalizedArray = NormalizeArray(array, direction);

            var startingIndex = 0;
            var length = 1;
            var lastDist = normalizedArray[0];

            for (var i = 1; i < normalizedArray.Length; i++)
            {
                var dist = normalizedArray[i];

                if (i == normalizedArray.Length - 1 // Last element must create cluster
                    || Mathf.Abs(dist - lastDist) > GroupingTolerance)
                {
                    var position = GetAveragePosition(normalizedArray, startingIndex, length, direction);
                    var distanceFromSensor = direction == Direction.Vertical ? position.y : position.x;

                    if (Mathf.Abs(distanceFromSensor - 1) > GroupingTolerance) // Ignore clusters that are at the max distance
                    {
                        var radius = length / (array.Length * 1f);

                        position += radius * (direction == Direction.Vertical ? Vector2.up : Vector2.right);
                        position = new Vector2(position.x, 1f - position.y);

                        var newEntity = new Entity
                        {
                            Position = position,
                            Radius = radius
                        };

                        entities.Add(newEntity);
                    }

                    startingIndex = i;
                    length = 1;
                }
                else
                {
                    length++;
                }

                lastDist = dist;
            }

            return entities.ToArray();
        }

        protected Vector2 GetAveragePosition(float[] array, int startingIndex, int clusterLength, Direction direction)
        {
            if (array.Length == 0)
                return Vector2.zero;

            var sum = 0f;


            for (var i = startingIndex; i < startingIndex + clusterLength; i++)
            {
                sum += array[i];
            }

            var locationAcrossArray = (startingIndex + clusterLength / 2f) / (array.Length * 1f);
            var distanceFromArray = sum / (clusterLength * 1f);

            switch (direction)
            {
                case Direction.Vertical:
                    return new Vector2(locationAcrossArray, distanceFromArray);
                case Direction.Horizontal:
                    return new Vector2(distanceFromArray, locationAcrossArray);
                default:
                    return Vector2.zero;
            }

        }

        protected Vector2 GetPosition(Vector2 normalizedPos)
        {
            return new Vector2(normalizedPos.x * MaxDistanceObserved.x, normalizedPos.y * MaxDistanceObserved.y);
        }

        protected Vector2 GetNormalizedPosition(Vector2 position)
        {
            return new Vector2(position.x / MaxDistanceObserved.x, position.y / MaxDistanceObserved.y);
        }

        public override CalibrationUpdate Calibrate(ISensorOutput data)
        {
            throw new System.NotImplementedException();
        }
    }    
}