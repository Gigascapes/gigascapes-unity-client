using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gigascapes.SystemDebug
{
    public class DebugManager : MonoBehaviour
    {
        public DebugCanvas DebugCanvas;

		void Awake()
		{
            HideDebugCanvas();
		}

		void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                ToggleDebugCanvas();
            }
        }

        void ToggleDebugCanvas()
        {
            if (DebugCanvas.gameObject.activeInHierarchy)
            {
                HideDebugCanvas();
            }
            else
            {
                ShowDebugCanvas();
            }
        }

        void ShowDebugCanvas()
        {
            DebugCanvas.gameObject.SetActive(true);
        }

        void HideDebugCanvas()
        {
            DebugCanvas.gameObject.SetActive(false);
        }
    }   
}