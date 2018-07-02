using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gigascapes.Sensors;

namespace Gigascapes.SystemDebug
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(LidarSignal))]
    public class RplidarVisualizer : MonoBehaviour
    {
        LidarSignal Signal;
        MeshRenderer MeshRenderer;
        public Mesh m_mesh;
        private MeshFilter m_meshfilter;

        private List<Vector3> m_vert;
        private List<int> m_ind;
        private List<int> triangles = new List<int>();
		private MeshCollider mc =null;

        void Awake()
        {
            MeshRenderer = GetComponent<MeshRenderer>();
            Signal = GetComponent<LidarSignal>();
            m_meshfilter = GetComponent<MeshFilter>();
			mc = GetComponent<MeshCollider>();

            Signal.OnLidarUpdate += HandleLidarUpdate;

            m_ind = new List<int>();
            m_vert = new List<Vector3>();
            for (int i = 0; i < Signal.DataPoints; i++)
            {
                m_ind.Add(i);
            }
            m_mesh = new Mesh();
            m_mesh.MarkDynamic();
        }
        
        void HandleLidarUpdate(LidarData[] m_data)
        {
            m_vert.Clear();
            triangles.Clear();
            m_vert.Add(Vector3.zero);
            for (int i = 0; i < m_data.Length; i++)
            {
                Vector3 inputV = Quaternion.Euler(0, 0, -1f * m_data[i].theta) * Vector3.down * m_data[i].distant * 0.001f;
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
            for (int i = 0; i < m_data.Length - 1; i++)
            {
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(i + 1);

            }
            triangles.Add(0);
            triangles.Add(m_data.Length);
            triangles.Add(1);

            m_mesh.SetVertices(m_vert);
            //m_mesh.SetIndices(m_ind.ToArray(), MeshTopology.Points, 0);
            m_mesh.SetTriangles(triangles.ToArray(), 0);


            m_mesh.UploadMeshData(false);
            m_meshfilter.mesh = m_mesh;

			if(mc != null)
			{
				mc.enabled = false;
				mc.sharedMesh = m_meshfilter.mesh;
				mc.enabled = true;
			}
        }

        public void Hide()
        {
            MeshRenderer.enabled = false;
        }

        public void Show()
        {
            MeshRenderer.enabled = true;
        }

        void OnDestroy()
        {
            Signal.OnLidarUpdate -= HandleLidarUpdate;
        }
    }
}
