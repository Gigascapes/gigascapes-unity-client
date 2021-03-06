﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

namespace Gigascapes.SystemDebug
{
    [RequireComponent(typeof(MeshFilter))]
    public class RplidarMesherMap1 : MonoBehaviour
    {

        public bool m_onscan = false;

        private LidarData[] m_data;
        public string COM = "COM7";

        public Mesh m_mesh;
        private List<Vector3> m_vert;
        private List<int> m_ind;
        private List<int> triangles = new List<int>();

        private MeshFilter m_meshfilter;

        private Thread m_thread;
        private bool m_datachanged = false;
        void Start()
        {

            m_meshfilter = GetComponent<MeshFilter>();

            m_data = new LidarData[720];

            m_ind = new List<int>();
            m_vert = new List<Vector3>();
            for (int i = 0; i < 720; i++)
            {
                m_ind.Add(i);
            }
            m_mesh = new Mesh();
            m_mesh.MarkDynamic();

            RplidarBinding1.OnConnectD(COM);
            RplidarBinding1.StartMotorD();
            m_onscan = RplidarBinding1.StartScanD();


            if (m_onscan)
            {
                m_thread = new Thread(GenMesh);
                m_thread.Start();
            }

        }

        void OnDestroy()
        {
            m_thread.Abort();

            RplidarBinding1.EndScanD();
            RplidarBinding1.EndMotorD();
            RplidarBinding1.OnDisconnectD();
            RplidarBinding1.ReleaseDriveD();

            m_onscan = false;
        }

        void Update()
        {
            if (m_datachanged)
            {
                m_vert.Clear();
                triangles.Clear();
                m_vert.Add(Vector3.zero);
                for (int i = 0; i < 720; i++)
                {
                    Vector3 inputV = Quaternion.Euler(0, 0, m_data[i].theta) * Vector3.down * m_data[i].distant * 0.001f;
                    //m_vert.Add(new Vector3(-inputV.x,inputV.y,0));
                    m_vert.Add(inputV);
                    Debug.DrawLine(transform.position + transform.forward * 0.2f, transform.position + transform.rotation * Quaternion.Euler(0, 0, m_data[i].theta) * -transform.up * 0.001f * m_data[i].distant, Color.black, 0.2f);

                    if (m_data[i].quality > 0)
                    {
                        //Debug.DrawLine(transform.position - transform.forward * 0.5f, transform.position + transform.rotation * Quaternion.Euler(0, 0, m_data[i].theta) * -transform.up * 0.1f * m_data[i].quality, Color.green, 0.2f);
                    }
                    if (m_data[i].quality <= 0)
                    {
                       //Debug.DrawLine(transform.position - transform.forward * 0.5f, transform.position + transform.rotation * Quaternion.Euler(0, 0, m_data[i].theta) * -transform.up, Color.blue, 0.2f);
                    }

                }
                for (int i = 0; i < 719; i++)
                {
                    triangles.Add(0);
                    triangles.Add(i);
                    triangles.Add(i + 1);

                }
                triangles.Add(0);
                triangles.Add(720);
                triangles.Add(1);

                m_mesh.SetVertices(m_vert);
                //m_mesh.SetIndices(m_ind.ToArray(), MeshTopology.Points, 0);
                m_mesh.SetTriangles(triangles.ToArray(), 0);


                m_mesh.UploadMeshData(false);
                m_meshfilter.mesh = m_mesh;

                m_datachanged = false;
            }
        }

        void GenMesh()
        {
            while (true)
            {
                int datacount = RplidarBinding1.GetDataD(ref m_data);
                if (datacount == 0)
                {
                    Thread.Sleep(20);
                }
                else
                {
                    m_datachanged = true;
                }
            }
        }

    }

}
