using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gigascapes.SignalProcessing
{
    public struct EntityMatch
    {
        public Entity oldEntity;
        public Entity newEntity;
    }

    public struct CalibrationUpdate
    {
        public Sensor Sensor;
        public Entity[] Entities;
        public CalibrationPhase Phase;
    }

    public class SensorCalibrationInfo
    {
        public List<CalibrationEntity> BlackList = new List<CalibrationEntity>();
        public int BaselineCalibrationCount;

        public float BlackListConfidence(CalibrationEntity entity)
        {
            if (!BlackList.Contains(entity))
                return 0f;
            return entity.Hits / (1f * BaselineCalibrationCount);
        }
    }

    public abstract class ProcessingMethod : MonoBehaviour
    {
        [SerializeField, Range(0f, 0.95f)]
        protected float Smoothing = 0f;

        [SerializeField, Range(0f, 10f)]
        protected float EntityTimeout = 0f;

        [SerializeField, Range(1f, 10f)]
        protected float EntitySearchRadiusMultiplier = 1f;

        [SerializeField, Range(0f, 1f)]
        protected float RequiredBlackListConfidence = 0.7f;

        public abstract Entity[] Process(ISensorOutput data);
        public abstract CalibrationUpdate Calibrate(ISensorOutput data);

        protected List<Entity> Entities = new List<Entity>();
        protected Dictionary<Entity, float> MissingEntities = new Dictionary<Entity, float>();

        protected readonly Dictionary<int, Sensor> Sensors = new Dictionary<int, Sensor>();
        protected readonly Dictionary<Sensor, SensorCalibrationInfo> CalibrationInfo = new Dictionary<Sensor, SensorCalibrationInfo>();
        protected List<Entity> BlackList = new List<Entity>();

        public CalibrationPhase CalibrationPhase { get; protected set; }
        public bool IsCalibrated { get { return CalibrationPhase == CalibrationPhase.Calibrated; } }

        public delegate void BlackListPublishEvent(Entity[] blacklist);
        public static event BlackListPublishEvent OnPublishBlackList;


        void Awake()
		{
            CalibrationPhase = CalibrationPhase.Uncalibrated;
		}

		public virtual void StartCalibration()
        {
            CalibrationPhase = CalibrationPhase.Calibrating;

            Entities.Clear();
            CalibrationInfo.Clear();
            Sensors.Clear();
        }

        public virtual void FinishCalibration()
        {
            CalibrationPhase = CalibrationPhase.Calibrated;
            RegisterBlackList();
        }

        protected void RegisterBlackList()
        {
            BlackList.Clear();
            foreach (var info in CalibrationInfo.Values)
            {
                foreach (var blackEntity in info.BlackList)
                {
                    if (info.BlackListConfidence(blackEntity) >= RequiredBlackListConfidence)
                    {
                        TryRegisterNewBlackEntity(blackEntity);
                    }
                }
            }

            if (OnPublishBlackList != null)
            {
                OnPublishBlackList(BlackList.ToArray());
            }
        }

        void TryRegisterNewBlackEntity(Entity blackEntity)
        {
            if (!EntityMatchInBlackList(blackEntity))
            {
                BlackList.Add(blackEntity);
            }
        }

        protected bool EntityMatchInBlackList(Entity entity)
        {
            return BlackList.Any(x => EntitiesMatch(entity, x));
        }

        protected void ProcessFrameEntities(Entity[] frameEntities)
        {
            var matches = MatchAndUpdateFrameEntities(frameEntities);
            var matchedEntities = matches.Select(x => x.oldEntity);
            var matchedFrameEntities = matches.Select(x => x.newEntity);

            var entitiesToRemove = Entities.Where(x => !matchedEntities.Contains(x)).ToArray();
            foreach (var entity in entitiesToRemove)
            {
                DeclareMissing(entity, Time.deltaTime);
            }
            CullMissingEntities();

            var unmatchedEntities = frameEntities.Where(x => !matchedFrameEntities.Contains(x));
            var blackMatches = new List<Entity>();
            foreach (var entity in unmatchedEntities)
            {
                foreach (var blackEntity in BlackList)
                {
                    if (EntitiesMatch(entity, blackEntity))
                    {
                        blackMatches.Add(entity);
                        continue;
                    }
                }
            }

            foreach (var newEntity in unmatchedEntities.Where(x => !blackMatches.Contains(x)))
            {
                AddEntity(newEntity);
            }
        }

        protected void AddEntity(Entity entity)
        {
            var i = 0;
            var indecesUsed = Entities.Select(x => x.ID);
            while (indecesUsed.Contains(i))
            {
                i++;
            }

            entity.ID = i;
            entity.Velocity = entity.SmoothedVelocity = Vector2.zero;
            entity.SmoothedRadius = entity.Radius;
            entity.SmoothedPosition = entity.Position;
            Entities.Add(entity);
        }

        protected void UpdateEntity(EntityMatch match, float weight = 1f)
        {
            var oldPosition = match.oldEntity.Position;
            match.oldEntity.Position = match.newEntity.Position;
            match.oldEntity.Radius = match.newEntity.Radius;
            match.oldEntity.Velocity = match.newEntity.Position - oldPosition;

            match.oldEntity.SmoothedPosition = match.newEntity.Position * weight + match.oldEntity.SmoothedPosition * (1f - weight);
            match.oldEntity.SmoothedRadius = match.newEntity.Radius * weight + match.oldEntity.SmoothedRadius * (1f - weight);
            match.oldEntity.SmoothedVelocity = match.oldEntity.Velocity * weight + match.oldEntity.SmoothedVelocity * (1f - weight);
        }

        protected bool EntitiesMatch(Entity entity1, Entity entity2)
        {
            var distance = (entity1.Position - entity2.Position).magnitude;
            var simulatedDistance = (entity1.Position - (entity2.Position + entity2.SmoothedVelocity)).magnitude;
            var searchRadius = entity2.SmoothedRadius * EntitySearchRadiusMultiplier;
            var isMatch = distance <= searchRadius || simulatedDistance <= searchRadius;
            return isMatch;
        }

        protected void MatchAndUpdateCalibrationBlackList(Sensor sensor, Entity[] frameEntities)
        {
            if (!CalibrationInfo.ContainsKey(sensor))
            {
                CalibrationInfo.Add(sensor, new SensorCalibrationInfo
                {
                    BaselineCalibrationCount = 1,
                    BlackList = frameEntities.Select(x => new CalibrationEntity(x)).ToList()
                });
                return;
            }

            CalibrationInfo[sensor].BaselineCalibrationCount++;

            var calibrationInfo = CalibrationInfo[sensor];
            foreach (var frameEntity in frameEntities)
            {
                var matchFound = false;
                foreach (var entity in calibrationInfo.BlackList)
                {
                    if (EntitiesMatch(frameEntity, entity))
                    {
                        matchFound = true;
                        UpdateEntity(new EntityMatch{oldEntity = entity, newEntity = frameEntity}, 1f - Smoothing);
                        entity.Hits++;
                        break;
                    }
                }

                if (!matchFound)
                {
                    calibrationInfo.BlackList.Add(new CalibrationEntity(frameEntity));
                }
            }
        }

        protected List<EntityMatch> MatchAndUpdateFrameEntities(Entity[] frameEntities)
        {
            var entityMatches = new List<EntityMatch>();
            var unclaimedEntities = new List<Entity>();
            unclaimedEntities.AddRange(Entities);

            foreach (var frameEntity in frameEntities)
            {
                var matches = new List<Entity>();
                foreach (var entity in unclaimedEntities)
                {
                    if (EntitiesMatch(frameEntity, entity))
                    {
                        matches.Add(entity);
                    }
                }

                if (matches.Count == 0 && Entities.Count > 0)
                {
                    /*Debug.LogWarningFormat("Breakage detected\nFrame Entity: {0}\n Entities: \n{1}", 
                                           FrameEntityString(frameEntity),
                                           string.Join(",\n", unclaimedEntities.Select(x => EntityMatchTestString(x, frameEntity)).ToArray()));*/
                }

                Entity match = null;
                if (matches.Count == 1)
                {
                    match = matches[0];
                }
                else if (matches.Count > 1)
                {
                    var velocityDiffs = matches.Select(x => Mathf.Abs(AngleDiff(x.SmoothedVelocity, frameEntity.Position - x.Position))).ToList();

                    var leastDifference = Mathf.Infinity;
                    var leastDifferenceIndex = 0;
                    for (var i = 0; i < velocityDiffs.Count; i++)
                    {
                        if (velocityDiffs[i] < leastDifference)
                        {
                            leastDifference = velocityDiffs[i];
                            leastDifferenceIndex = i;
                        }
                    }
                        
                    match = matches[leastDifferenceIndex];
                }

                if (match == null)
                    continue;
                
                unclaimedEntities.Remove(match);
                var newMatch = new EntityMatch { oldEntity = match, newEntity = frameEntity };
                entityMatches.Add(newMatch);
                UpdateEntity(newMatch, 1f - Smoothing);
            }

            return entityMatches;
        }

        string FrameEntityString(Entity frameEntity)
        {
            return string.Format("[Pos: ({0}, {1}), Rad: {2}", frameEntity.Position.x, frameEntity.Position.y, frameEntity.Radius);
        }

        string EntityString(Entity frameEntity)
        {
            return string.Format("[Pos: ({0}, {1}), SearchRad: {2}", frameEntity.Position.x, frameEntity.Position.y, frameEntity.SmoothedRadius * EntitySearchRadiusMultiplier);
        }

        string EntityMatchTestString(Entity entity, Entity frameEntity)
        {
            var distance = (frameEntity.Position - entity.Position).magnitude;
            var simulatedDistance = (frameEntity.Position - (entity.Position + entity.SmoothedVelocity)).magnitude;
            return string.Format("{0} - Distance: {1}, SimulatedDist: {2}", EntityString(entity), distance, simulatedDistance);
        }

        static float AngleDiff(Vector2 a, Vector2 b)
        {
            if (a.magnitude == 0 || b.magnitude == 0)
                return Mathf.Infinity;

            return Mathf.Acos(Vector2.Dot(a.normalized, b.normalized));
        }

        protected void DeclareMissing(Entity entity, float time)
        {
            if (!MissingEntities.ContainsKey(entity))
            {
                MissingEntities[entity] = time;
            }
            else
            {
                MissingEntities[entity] += time;
            }

            UpdateEntity(new EntityMatch { oldEntity = entity, newEntity = entity }, 1f - Smoothing);
        }

        protected void CullMissingEntities()
        {
            foreach (var entity in MissingEntities.Keys.ToArray())
            {
                if (MissingEntities[entity] >= EntityTimeout)
                {
                    Entities.Remove(entity);
                    MissingEntities.Remove(entity);
                }
            }
        }
    }
}