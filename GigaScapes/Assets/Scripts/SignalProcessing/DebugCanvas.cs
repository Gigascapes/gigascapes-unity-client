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

        bool ShowingPreCalibrationInstructions = false;

		void Start()
		{
            EndCalibration();
            SignalProcessor.UpdateCalibration += HandleCalibrationUpdate;
		}

        public void ProceedWithCalibration()
        {
            switch (SignalProcessor.CalibrationPhase)
            {
                case CalibrationPhase.Calibrated:
                case CalibrationPhase.Uncalibrated:
                    StartCalibration();
                    break;
                case CalibrationPhase.BaselineEmpty:
                    SetBaseline();
                    break;
                default:
                    EndCalibration();
                    break;
            }
        }

        void StartCalibration()
        {
            if (ShowingPreCalibrationInstructions)
            {
                CalibrationButtonText.text = "Set Baseline";
                InstructionText.text = "Calibrating...";
                if (SignalProcessor != null)
                {
                    SignalProcessor.StartCalibration();
                }
            }
            else
            {
                CalibrationButtonText.text = "Area clear";
                InstructionText.text = "Clear play area";
            }
            ShowingPreCalibrationInstructions = !ShowingPreCalibrationInstructions;
        }

        void SetBaseline()
        {
            CalibrationButtonText.text = "Abort";
            InstructionText.text = "Stand in center of play area";
            if (SignalProcessor != null)
            {
                SignalProcessor.SetBaseline();
            }
        }

        void EndCalibration()
        {
            if (SignalProcessor != null)
            {
                CalibrationButtonText.text = "Calibrate";
                InstructionText.text = "";
                SignalProcessor.EndCalibration();
            }
        }

        void HandleCalibrationUpdate(CalibrationUpdate update)
        {
            switch (update.Phase)
            {
                case CalibrationPhase.SearchingForPoint1:
                    InstructionText.text = "Stand in center of play area";
                    break;
                case CalibrationPhase.SearchingForPoint2:
                    InstructionText.text = "Stand in front left of play area";
                    break;
                case CalibrationPhase.SearchingForPoint3:
                    InstructionText.text = "Stand in back right of play area";
                    break;
                case CalibrationPhase.Calibrated:
                    EndCalibration();
                    break;
            }
        }
    }   
}