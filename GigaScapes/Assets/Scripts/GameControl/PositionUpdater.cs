using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Gigascapes.SignalProcessing;

namespace Gigascapes.GameModules.Common
{
    public class PositionUpdater : MonoBehaviour
    {
        [SerializeField]
        List<Transform> ControlledTransforms;

        [SerializeField]
        bool ShowEntityMarkers;
        bool WasShowingEntityMarkers;

        [SerializeField]
        GameObject EntityMarkerPrefab;

        [SerializeField]
        List<Color> EntityMarkerColors;
        int LastUsedColorIndex = -1;

        Dictionary<int, Transform> EntityMarkerLookup = new Dictionary<int, Transform>();

        void Awake()
        {
            SignalProcessor.Publish += HandleSignal;
        }

		void Update()
		{
            if (ShowEntityMarkers != WasShowingEntityMarkers)
            {
                WasShowingEntityMarkers = ShowEntityMarkers;
                UpdateMarkerVisibility();
            }
		}

        void UpdateMarkerVisibility()
        {
            foreach (var marker in EntityMarkerLookup.Values)
            {
                marker.gameObject.SetActive(ShowEntityMarkers);
            }
        }

		void OnDestroy()
		{
            SignalProcessor.Publish -= HandleSignal;	
		}

		void HandleSignal(Entity[] entities)
        {
            var unhandledEntities = entities.ToList();
            foreach (var t in ControlledTransforms)
            {
                if (unhandledEntities.Count > 0)
                {
                    var closestEntity = GetClosest(t, unhandledEntities);
                    t.position = GameController.Instance.GetScenePosition(closestEntity.SmoothedPosition);
                    unhandledEntities.Remove(closestEntity);
                }
            }

            if (ShowEntityMarkers)
            {
                UpdateMarkers(entities);
            }
        }

        Entity GetClosest(Transform t, List<Entity> entities)
        {
            var minDistance = Mathf.Infinity;
            var minDistanceIndex = 0;
            for (var i = 0; i < entities.Count; i++)
            {
                var distance = (entities[i].SmoothedPosition - new Vector2(t.position.x, t.position.y)).magnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minDistanceIndex = i;
                }
            }

            return entities[minDistanceIndex];
        }

        void UpdateMarkers(IEnumerable<Entity> entities)
        {
            RemoveUnusedMarkers(entities);
            UpdateMarkerPositions(entities);
        }

        void RemoveUnusedMarkers(IEnumerable<Entity> entities)
        {
            var ids = entities.Select(x => x.ID);
            foreach (var markerId in EntityMarkerLookup.Keys.ToArray())
            {
                if (!ids.Contains(markerId))
                {
                    Destroy(EntityMarkerLookup[markerId].gameObject);
                    EntityMarkerLookup.Remove(markerId);
                }
            }
        }

        void UpdateMarkerPositions(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                if (EntityMarkerLookup.ContainsKey(entity.ID))
                {
                    EntityMarkerLookup[entity.ID].position = GameController.Instance.GetScenePosition(entity.Position);
                    EntityMarkerLookup[entity.ID].localScale = GameController.Instance.SceneSize * entity.Radius;
                }
                else
                {
                    var newMarker = Instantiate(EntityMarkerPrefab, GameController.Instance.GetScenePosition(entity.Position), Quaternion.identity);
                    newMarker.transform.localScale = GameController.Instance.SceneSize * entity.Radius;
                    EntityMarkerLookup[entity.ID] = newMarker.transform;

                    if (EntityMarkerColors.Count > 0)
                    {
                        LastUsedColorIndex = (LastUsedColorIndex + 1) % EntityMarkerColors.Count;
                        newMarker.GetComponent<SpriteRenderer>().color = EntityMarkerColors[LastUsedColorIndex];
                    }
                }
            }
        }
    }
}
