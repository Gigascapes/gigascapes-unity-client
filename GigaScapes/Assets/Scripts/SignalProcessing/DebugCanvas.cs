using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gigascapes.SignalProcessing;

namespace Gigascapes.SystemDebug
{
    public class DebugCanvas : MonoBehaviour
    {
        public SignalProcessor SignalProcessor;
        public CalibrationVisualizer CalibrationVisualizer;

        public Text CalibrationButtonText;
        public Text InstructionText;

        public delegate void CalibrationStartedEvent();
        public event CalibrationStartedEvent OnCalibrationStarted;

        public delegate void CalibrationFinishedEvent();
        public event CalibrationFinishedEvent OnCalibrationFinished;

		void Start()
		{
            SetInitial();
		}

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) ProceedWithCalibration();
        }

        public void ProceedWithCalibration()
        {
            switch (SignalProcessor.CalibrationPhase)
            {
                case CalibrationPhase.Calibrated:
                case CalibrationPhase.Uncalibrated:
                    StartCalibration();
                    break;
                case CalibrationPhase.Calibrating:
                    FinishCalibration();
                    break;
                default:
                    FinishCalibration();
                    break;
            }
        }

        void SetInitial()
        {
            CalibrationButtonText.text = "Calibrate";
            InstructionText.text = "System needs calibration";
        }

        void StartCalibration()
        {
            CalibrationButtonText.text = "Area clear";
            InstructionText.text = "Clear play area";
            if (OnCalibrationStarted != null)
            {
                OnCalibrationStarted();
            }
            if (SignalProcessor != null)
            {
                SignalProcessor.StartCalibration();
            }
        }

        void FinishCalibration()
        {
            CalibrationButtonText.text = "Calibrate";
            InstructionText.text = "Calibrated";
            if (OnCalibrationFinished != null)
            {
                OnCalibrationFinished();
            }
            if (SignalProcessor != null)
            {
                SignalProcessor.FinishCalibration();
            }
        }
    }   
}