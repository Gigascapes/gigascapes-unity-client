using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gigascapes.SignalProcessing;

namespace Gigascapes.SystemDebug
{
    public class CalibrationVisualizer : MonoBehaviour
    {
        [SerializeField]
        GameObject BlackEntityPrefab;

        void Awake()
        {
            ProcessingMethod.OnPublishBlackList += SpawnBlackEntities;
        }

        public void ClearBlackEntities()
        {
            foreach (var blackEntity in GetComponentsInChildren<BlackEntity>())
            {
                Destroy(blackEntity.gameObject);
            }
        }

        void SpawnBlackEntities(Entity[] blackEntities)
        {
            ClearBlackEntities();

            foreach (var blackEntity in blackEntities)
            {
                //Debug.LogErrorFormat("Spawning black entity at {0}, {1}", blackEntity.RawPosition.x, blackEntity.RawPosition.y);
                var newPip = Instantiate(BlackEntityPrefab, blackEntity.RawPosition, Quaternion.identity);
                newPip.transform.localScale = Vector3.one * blackEntity.RawRadius * 3f;
                newPip.transform.SetParent(transform);
            }
        }

        public void Show()
        {
            foreach (var blackEntity in GetComponentsInChildren<BlackEntity>())
            {
                blackEntity.GetComponent<Renderer>().enabled = true;
            }
        }

        public void Hide()
        {
            foreach (var blackEntity in GetComponentsInChildren<BlackEntity>())
            {
                blackEntity.GetComponent<Renderer>().enabled = false;
            }
        }
    }   
}