using System.Linq;
using System.Collections;
using System.Threading;
using UnityEngine;
using Gigascapes.SignalProcessing;
using Gigascapes.SystemDebug;

namespace Gigascapes.Sensors
{
    public enum LidarLibraryTarget
    {
        Base,
        A,
        B
    }

    public class LidarSignal : SensorSignal
    {
        public LidarLibraryTarget LibraryTarget;
        public string ComCode;
        public float StartupDelay;
        public int DataPoints = 720;
        bool IsFound;
        bool IsDataChanged;
        Thread Thread;
        public LidarData[] Data;

        public delegate void LidarUpdateEvent(LidarData[] data);
        public event LidarUpdateEvent OnLidarUpdate;

        void Start()
        {
            Data = new LidarData[DataPoints];
            StartCoroutine(InitializeLidar(LibraryTarget));
        }

        void CheckForData()
        {
            while (true)
            {
                int datacount = GetData(LibraryTarget);
                if (datacount == 0)
                {
                    Thread.Sleep(20);
                }
                else
                {
                    IsDataChanged = true;
                }
            }
        }

        void Update()
        {
            if (IsDataChanged)
            {
                if (IsCalibrated) SendData(); else SendCalibrationData();
                IsDataChanged = false;
            }
        }

        void SendData()
        {
            if (OnLidarUpdate != null)
            {
                OnLidarUpdate(Data);
            }

            var signal = new LidarOutput
            {
                Index = ComCode.GetHashCode(),
                Data = Data.Select(x => new LidarBeam
                {
                    Distance = x.distant / 1000f,
                    Angle = x.theta,
                    Quality = (int)x.quality
                }).ToArray()
            };
            Send(signal);
        }

        public void SendCalibrationData()
        {
            if (OnLidarUpdate != null)
            {
                OnLidarUpdate(Data);
            }

            var signal = new OmniscientLidarOutput
            {
                Index = ComCode.GetHashCode(),
                Direction = transform.rotation.eulerAngles.z,
                Location = transform.position,
                TopLeft = DebugManager.Instance.GameSpaceVisualizer.TopLeft.position,
                BottomRight = DebugManager.Instance.GameSpaceVisualizer.BottomRight.position,
                Data = Data.Select(x => new LidarBeam
                {
                    Distance = x.distant / 1000f,
                    Angle = x.theta,
                    Quality = (int)x.quality
                }).ToArray()
            };
            Send(signal);
        }

        void OnDestroy()
        {
            if (IsFound)
            {
                Thread.Abort();
                IsFound = false;
            }

            ShutDownLidar(LibraryTarget);
        }

        IEnumerator InitializeLidar(LidarLibraryTarget target)
        {
            switch (target)
            {
                case LidarLibraryTarget.Base:
                    RplidarBinding.OnConnect(ComCode);
                    RplidarBinding.StartMotor();
                    break;
                case LidarLibraryTarget.A:
                    RplidarBindingA.OnConnectS(ComCode);
                    RplidarBindingA.StartMotorS();
                    break;
                case LidarLibraryTarget.B:
                    RplidarBindingB.OnConnectS(ComCode);
                    RplidarBindingB.StartMotorS();
                    break;
            }

            yield return new WaitForSeconds(StartupDelay);

            switch (target)
            {
                case LidarLibraryTarget.Base:
                    IsFound = RplidarBinding.StartScan();
                    break;
                case LidarLibraryTarget.A:
                    IsFound = RplidarBindingA.StartScanS();
                    break;
                case LidarLibraryTarget.B:
                    IsFound = RplidarBindingB.StartScanS();
                    break;
            }

            Debug.Log(IsFound);
            if (IsFound)
            {
                Thread = new Thread(CheckForData);
                Thread.Start();
            }
        }

        int GetData(LidarLibraryTarget target)
        {
            switch (target)
            {
                case LidarLibraryTarget.Base:
                    return RplidarBinding.GetData(ref Data);
                case LidarLibraryTarget.A:
                    return RplidarBindingA.GetDataS(ref Data);
                case LidarLibraryTarget.B:
                    return RplidarBindingB.GetDataS(ref Data);
                default:
                    return 0;
            }
        }

        void ShutDownLidar(LidarLibraryTarget target)
        {
            Debug.Log("Shutting Down");
            switch (target)
            {
                case LidarLibraryTarget.Base:
                    RplidarBinding.EndScan();
                    RplidarBinding.EndMotor();
                    RplidarBinding.OnDisconnect();
                    RplidarBinding.ReleaseDrive();
                    break;
                case LidarLibraryTarget.A:
                    RplidarBindingA.EndScanS();
                    RplidarBindingA.EndMotorS();
                    RplidarBindingA.OnDisconnectS();
                    RplidarBindingA.ReleaseDriveS();
                    break;
                case LidarLibraryTarget.B:
                    RplidarBindingB.EndScanS();
                    RplidarBindingB.EndMotorS();
                    RplidarBindingB.OnDisconnectS();
                    RplidarBindingB.ReleaseDriveS();
                    break;
            }
        }
    }
}