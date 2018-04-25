using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gigascapes.SystemDebug
{
    [RequireComponent(typeof(MeshFilter))]
    public class GameSpaceVisualizer : MonoBehaviour
    {
        public Transform TopLeft;
        public Transform BottomRight;

        MeshRenderer MeshRenderer;
        MeshFilter MeshFilter;
        Mesh Mesh;

        private List<Vector3> m_vert = new List<Vector3>();
        private List<int> m_ind = new List<int>();
        private List<int> triangles = new List<int> { 0,2,1, 0,1,3 };

        void Awake()
        {
            MeshRenderer = GetComponent<MeshRenderer>();
            MeshFilter = GetComponent<MeshFilter>();
            Mesh = new Mesh();
            Mesh.MarkDynamic();

            for (var i=0; i<4; i++)
            {
                m_ind.Add(i);
            }
        }

        void Update()
        {
            m_vert.Clear();

            m_vert.Add(TopLeft.position);
            m_vert.Add(BottomRight.position);
            m_vert.Add(new Vector3(BottomRight.position.x, TopLeft.position.y, TopLeft.position.z));
            m_vert.Add(new Vector3(TopLeft.position.x, BottomRight.position.y, BottomRight.position.z));

            Mesh.SetVertices(m_vert);
            Mesh.SetTriangles(triangles.ToArray(), 0);

            Mesh.UploadMeshData(false);
            MeshFilter.mesh = Mesh;
        }

        public void Hide()
        {
            MeshRenderer.enabled = false;
        }

        public void Show()
        {
            MeshRenderer.enabled = true;
        }
    }
}
