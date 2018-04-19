using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gigascapes.GameModules.Common
{
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        Transform TopLeftMarker;

        [SerializeField]
        Transform BottomRightMarker;

        public static GameController Instance;

        void Awake()
        {
            Instance = this;
        }

        public float SceneWidth { get { return BottomRightMarker.position.x - TopLeftMarker.position.x; } }
        public float SceneHeight { get { return TopLeftMarker.position.y - BottomRightMarker.position.y; } }
        public Vector2 SceneSize { get { return new Vector2(SceneWidth, SceneHeight); } }

        public Vector2 GetScenePosition(Vector2 normalizedPosition)
        {
            return new Vector2(
                TopLeftMarker.position.x + normalizedPosition.x * (BottomRightMarker.position.x - TopLeftMarker.position.x),
                BottomRightMarker.position.y + normalizedPosition.y * (TopLeftMarker.position.y - BottomRightMarker.position.y)
            );
        }
    }
}
