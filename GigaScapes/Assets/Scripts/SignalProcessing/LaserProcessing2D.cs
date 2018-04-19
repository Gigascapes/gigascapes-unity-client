using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Gigascapes.Sensors;

namespace Gigascapes.SignalProcessing
{
    public class LaserProcessing2D : LaserArrayProcessing
    {
        public override Entity[] Process(ISensorOutput data)
        {
            var sensorArrayData = data as SensorArrayOutput;
            if (sensorArrayData == null)
                return new Entity[] { };

            var arrayOfInterest = sensorArrayData.LaserArray1 ?? sensorArrayData.LaserArray2;
            if (arrayOfInterest.Length == 0)
            {
                return Entities.ToArray();
            }

            var direction = arrayOfInterest == sensorArrayData.LaserArray1 ? Direction.Vertical : Direction.Horizontal;

            return ProcessArray(arrayOfInterest, direction);
        }

        protected override Entity[] ProcessArray(float[] arrayOfInterest, Direction direction)
        {
            var frameEntities = CalculateFrameEntities(arrayOfInterest, direction);

            var matches = MatchAndUpdateFrameEntities(frameEntities);
            var matchedEntities = matches.Select(x => x.oldEntity);
            var matchedFrameEntities = matches.Select(x => x.newEntity);

            var entitiesToRemove = Entities.Where(x => !matchedEntities.Contains(x)).ToArray();
            foreach (var entity in entitiesToRemove)
            {
                DeclareMissing(entity, Time.deltaTime);
            }
            CullMissingEntities();

            var newEntities = frameEntities.Where(x => !matchedFrameEntities.Contains(x));
            foreach (var newEntity in newEntities)
            {
                AddEntity(newEntity);
            }

            return Entities.ToArray();
        }
    }
}
