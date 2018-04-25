using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Gigascapes.SignalProcessing;

namespace Gigascapes.Sensors
{
    public class LidarSignal : SensorSignal
    {
        public string ComCode;
        public float StartupDelay;
        public int DataPoints = 720;
        bool IsFound;
        bool IsDataChanged;
        Thread Thread;
        public LidarData[] Data;

        public delegate void LidarUpdateEvent(LidarData[] data);
        public event LidarUpdateEvent OnLidarUpdate;

        IEnumerator Start()
        {
            yield return new WaitForSeconds(StartupDelay);

            Data = new LidarData[DataPoints];
            RplidarBinding.OnConnect(ComCode);
            RplidarBinding.StartMotor();
            IsFound = RplidarBinding.StartScan();

            if (IsFound)
            {
                Thread = new Thread(CheckForData);
                Thread.Start();
            }
        }

        void CheckForData()
        {
            while (true)
            {
                int datacount = RplidarBinding.GetData(ref Data);
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
                SendData();
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

        public void SendCalibrationData(Vector2 topLeft, Vector2 bottomRight)
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
                TopLeft = topLeft,
                BottomRight = bottomRight,
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
            Thread.Abort();

            RplidarBinding.EndScan();
            RplidarBinding.EndMotor();
            RplidarBinding.OnDisconnect();
            RplidarBinding.ReleaseDrive();

            IsFound = false;
        }
    }
}