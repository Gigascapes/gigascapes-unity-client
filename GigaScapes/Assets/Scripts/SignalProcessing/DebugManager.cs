﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Gigascapes.Sensors;

namespace Gigascapes.SystemDebug
{
    public class DebugManager : MonoBehaviour
    {
        public DebugCanvas DebugCanvas;
        public GameSpaceVisualizer GameSpaceVisualizer;
        public CalibrationVisualizer CalibrationVisualizer;

        public float CalibrationTranslationSpeed = 0.5f;
        public float CalibrationRotationSpeed = 1f;
        public float SpeedMultiplier = 2f;

        List<RplidarVisualizer> LidarVisualizers;
        Transform CalibrationTarget;

        public static DebugManager Instance;

		void Awake()
		{
            Instance = this;
            DebugCanvas.OnCalibrationStarted += HandleCalibrationStarted;
            DebugCanvas.OnCalibrationFinished += HandleCalibrationFinished;
            CalibrationTarget = GameSpaceVisualizer.TopLeft;
            LidarVisualizers = FindObjectsOfType<RplidarVisualizer>().ToList();
            HideDebugCanvas();
		}

        private void OnDestroy()
        {
            DebugCanvas.OnCalibrationStarted -= HandleCalibrationStarted;
            DebugCanvas.OnCalibrationFinished -= HandleCalibrationFinished;
        }

        void HandleCalibrationStarted()
        {
            CalibrationVisualizer.ClearBlackEntities();
        }

        void HandleCalibrationFinished()
        {
            foreach (var visualizer in LidarVisualizers)
            {
                //visualizer.GetComponent<LidarSignal>().SendCalibrationData(GameSpaceVisualizer.TopLeft.position, GameSpaceVisualizer.BottomRight.position);
            }
        }

        void Update()
        {
            if (DebugCanvas.isActiveAndEnabled)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    SwitchCalibrationTarget();
                }
                var speed = CalibrationTranslationSpeed;
                var rotationSpeed = CalibrationRotationSpeed;
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    speed *= SpeedMultiplier;
                    rotationSpeed *= SpeedMultiplier;
                }
                CalibrationTarget.position += Vector3.right * Input.GetAxis("Horizontal") * speed;
                CalibrationTarget.position += Vector3.up * Input.GetAxis("Vertical") * speed;


                if (Input.GetKey(KeyCode.Q))
                {
                    CalibrationTarget.Rotate(Vector3.forward * rotationSpeed);
                }
                else if (Input.GetKey(KeyCode.E))
                {
                    CalibrationTarget.Rotate(Vector3.back * rotationSpeed);
                }
            }

            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                ToggleDebugCanvas();
            }
        }

        void SwitchCalibrationTarget()
        {
            if (CalibrationTarget == GameSpaceVisualizer.TopLeft)
            {
                CalibrationTarget = GameSpaceVisualizer.BottomRight;
            }
            else if (CalibrationTarget == GameSpaceVisualizer.BottomRight)
            {
                if (LidarVisualizers.Count > 0)
                    CalibrationTarget = LidarVisualizers[0].transform;
                else
                    CalibrationTarget = GameSpaceVisualizer.TopLeft;
            }
            else
            {
                var lidarIndexSelected = LidarVisualizers.IndexOf(CalibrationTarget.GetComponent<RplidarVisualizer>());
                if (lidarIndexSelected == LidarVisualizers.Count - 1)
                {
                    CalibrationTarget = GameSpaceVisualizer.TopLeft;
                }
                else
                {
                    lidarIndexSelected++;
                    CalibrationTarget = LidarVisualizers[lidarIndexSelected].transform;
                }
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
            foreach (var visualizer in LidarVisualizers)
            {
                visualizer.Show();
            }
            GameSpaceVisualizer.Show();
            CalibrationVisualizer.Show();
        }

        void HideDebugCanvas()
        {
            DebugCanvas.gameObject.SetActive(false);
            foreach (var visualizer in LidarVisualizers)
            {
                visualizer.Hide();
            }
            GameSpaceVisualizer.Hide();
            CalibrationVisualizer.Hide();
        }
    }   
}