using System.Collections;
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

        List<RplidarVisualizer> LidarVisualizers = new List<RplidarVisualizer>();
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

                // save and set calibration state
                CalibrationSave(KeyCode.Alpha1, 1);
                CalibrationSave(KeyCode.Alpha2, 2);
                CalibrationSave(KeyCode.Alpha3, 3);
                CalibrationSave(KeyCode.Alpha4, 4);
                CalibrationSave(KeyCode.Alpha5, 5);
                CalibrationSave(KeyCode.Alpha6, 6);
                CalibrationSave(KeyCode.Alpha7, 7);
                CalibrationSave(KeyCode.Alpha8, 8);

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

        void CalibrationSave(KeyCode kCode, int slot)
        {
            // save and set calibration state
            if (Input.GetKeyDown(kCode) && Input.GetKey(KeyCode.LeftShift) && LidarVisualizers.Count == 3)
            {
                CalibrationState a = new CalibrationState(
                    LidarVisualizers[0].transform.position, LidarVisualizers[0].transform.rotation,
                    LidarVisualizers[1].transform.position, LidarVisualizers[1].transform.rotation,
                    LidarVisualizers[2].transform.position, LidarVisualizers[2].transform.rotation
                    );

                a.SaveCalibrationState(a, slot);

            }
            else if (Input.GetKeyDown(kCode) && LidarVisualizers.Count == 3)
            {
                CalibrationState a = new CalibrationState();
                a = a.GetSavedCalibration(slot);

                LidarVisualizers[0].transform.position = a.sensor1T;
                LidarVisualizers[0].transform.rotation = a.sensor1R;
                LidarVisualizers[1].transform.position = a.sensor2T;
                LidarVisualizers[1].transform.rotation = a.sensor2R;
                LidarVisualizers[2].transform.position = a.sensor3T;
                LidarVisualizers[2].transform.rotation = a.sensor3R;
            }
        }
    }   

	public class CalibrationState
	{
		public Vector3 sensor1T = Vector3.zero;
		public Quaternion sensor1R = Quaternion.identity;

		public Vector3 sensor2T = Vector3.zero;
		public Quaternion sensor2R = Quaternion.identity;

		public Vector3 sensor3T = Vector3.zero;
		public Quaternion sensor3R = Quaternion.identity;

		public CalibrationState()
		{
			
		}

		public CalibrationState(Vector3 s1Pos, Quaternion s1Rot,
			Vector3 s2Pos, Quaternion s2Rot,
			Vector3 s3Pos, Quaternion s3Rot)
		{
			sensor1T = s1Pos;
			sensor1R = s1Rot;
			sensor2T = s2Pos;
			sensor2R = s2Rot;
			sensor3T = s3Pos;
			sensor3R = s3Rot;
		}

		public void SaveCalibrationState (CalibrationState cs, int key)
		{
			//sensor1
			PlayerPrefs.SetFloat(key.ToString() + "s1tx", cs.sensor1T.x);
			PlayerPrefs.SetFloat(key.ToString() + "s1ty", cs.sensor1T.y);
			PlayerPrefs.SetFloat(key.ToString() + "s1tz", cs.sensor1T.z);
			PlayerPrefs.SetFloat(key.ToString() + "s1rx", cs.sensor1R.x);
			PlayerPrefs.SetFloat(key.ToString() + "s1ry", cs.sensor1R.y);
			PlayerPrefs.SetFloat(key.ToString() + "s1rz", cs.sensor1R.z);
			PlayerPrefs.SetFloat(key.ToString() + "s1rw", cs.sensor1R.w);
			//sensor2
			PlayerPrefs.SetFloat(key.ToString() + "s2tx", cs.sensor2T.x);
			PlayerPrefs.SetFloat(key.ToString() + "s2ty", cs.sensor2T.y);
			PlayerPrefs.SetFloat(key.ToString() + "s2tz", cs.sensor2T.z);
			PlayerPrefs.SetFloat(key.ToString() + "s2rx", cs.sensor2R.x);
			PlayerPrefs.SetFloat(key.ToString() + "s2ry", cs.sensor2R.y);
			PlayerPrefs.SetFloat(key.ToString() + "s2rz", cs.sensor2R.z);
			PlayerPrefs.SetFloat(key.ToString() + "s2rw", cs.sensor2R.w);
			//sensor3
			PlayerPrefs.SetFloat(key.ToString() + "s3tx", cs.sensor3T.x);
			PlayerPrefs.SetFloat(key.ToString() + "s3ty", cs.sensor3T.y);
			PlayerPrefs.SetFloat(key.ToString() + "s3tz", cs.sensor3T.z);
			PlayerPrefs.SetFloat(key.ToString() + "s3rx", cs.sensor3R.x);
			PlayerPrefs.SetFloat(key.ToString() + "s3ry", cs.sensor3R.y);
			PlayerPrefs.SetFloat(key.ToString() + "s3rz", cs.sensor3R.z);
			PlayerPrefs.SetFloat(key.ToString() + "s3rw", cs.sensor3R.w);
		}

		public CalibrationState GetSavedCalibration(int key)
		{
			if (PlayerPrefs.HasKey(key.ToString() + "s3rw"))
			{
				return new CalibrationState(
					new Vector3(
						PlayerPrefs.GetFloat(key.ToString() + "s1tx"),
						PlayerPrefs.GetFloat(key.ToString() + "s1ty"),
						PlayerPrefs.GetFloat(key.ToString() + "s1tz")),
					new Quaternion(
						PlayerPrefs.GetFloat(key.ToString() + "s1rx"),
						PlayerPrefs.GetFloat(key.ToString() + "s1ry"),
						PlayerPrefs.GetFloat(key.ToString() + "s1rz"),
						PlayerPrefs.GetFloat(key.ToString() + "s1rw")),
					new Vector3(
						PlayerPrefs.GetFloat(key.ToString() + "s2tx"),
						PlayerPrefs.GetFloat(key.ToString() + "s2ty"),
						PlayerPrefs.GetFloat(key.ToString() + "s2tz")),
					new Quaternion(
						PlayerPrefs.GetFloat(key.ToString() + "s2rx"),
						PlayerPrefs.GetFloat(key.ToString() + "s2ry"),
						PlayerPrefs.GetFloat(key.ToString() + "s2rz"),
						PlayerPrefs.GetFloat(key.ToString() + "s2rw")),
					new Vector3(
						PlayerPrefs.GetFloat(key.ToString() + "s3tx"),
						PlayerPrefs.GetFloat(key.ToString() + "s3ty"),
						PlayerPrefs.GetFloat(key.ToString() + "s3tz")),
					new Quaternion(
						PlayerPrefs.GetFloat(key.ToString() + "s3rx"),
						PlayerPrefs.GetFloat(key.ToString() + "s3ry"),
						PlayerPrefs.GetFloat(key.ToString() + "s3rz"),
						PlayerPrefs.GetFloat(key.ToString() + "s3rw"))
					);
			}
			else
			{
				Debug.Log("Cant Find keys");
				return new CalibrationState();
			}
		}

	}
}