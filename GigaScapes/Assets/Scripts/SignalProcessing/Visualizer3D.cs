using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gigascapes.Sensors;
using Gigascapes.SignalProcessing;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
public class Visualizer3D : MonoBehaviour {

	public LidarSignal a;
	public LidarSignal b;
	public LidarSignal c;

	public List<List<LidarDataAnd>> History = new List<List<LidarDataAnd>>();

	public List<List<Vector2>> PositionHistory = new List<List<Vector2>>();

	public List<GameObject> ManipulatedPool;

	public GameObject vPrefab = null;


	MeshRenderer MeshRenderer;
	public Mesh m_mesh;
	private MeshFilter m_meshfilter;

	private List<Vector3> m_vert;
	
	private List<int> triangles = new List<int>();

	void Awake()
	{
		MeshRenderer = GetComponent<MeshRenderer>();
		
		m_meshfilter = GetComponent<MeshFilter>();

		
		m_vert = new List<Vector3>();
		
		m_mesh = new Mesh();
		m_mesh.MarkDynamic();
	}

	private void FixedUpdate()
	{
		// conversion
		List<LidarData> ad = a.Data.ToList<LidarData>();
		List<LidarDataAnd> at = new List<LidarDataAnd>();
		foreach(LidarData ld in ad)
		{
			at.Add(new LidarDataAnd(ld, a.transform));
		}
		List<LidarData> bd = b.Data.ToList<LidarData>();
		List<LidarDataAnd> bt = new List<LidarDataAnd>();
		foreach (LidarData ld in bd)
		{
			bt.Add(new LidarDataAnd(ld, b.transform));
		}
		List<LidarData> cd = c.Data.ToList<LidarData>();
		List<LidarDataAnd> ct = new List<LidarDataAnd>();
		foreach (LidarData ld in cd)
		{
			ct.Add(new LidarDataAnd(ld, c.transform));
		}

		List<LidarDataAnd> combo = new List<LidarDataAnd>();
		combo.AddRange(at);
		combo.AddRange(bt);
		combo.AddRange(ct);


		/// convert to 2d space
		//combo.Sort((x, y) => x.theta.CompareTo(y.theta));

		//combo.ConvertAll(new System.Converter<LidarData, Vector2>(LidarData2World));

		List<Vector2> ComboPos = new List<Vector2>();
		List<LidarDataAnd> combo2 = combo;
		List<Vector2> TransformedPostitions = combo2.ConvertAll<Vector2>(new System.Converter<LidarDataAnd, Vector2>(LidarData2Transform));

		/// this is for tracking over time
		History.Add(combo);
		PositionHistory.Add(TransformedPostitions);

		if (History.Count > 100)
		{
			History.RemoveAt(0);
		}
		if (PositionHistory.Count > 100)
		{
			PositionHistory.RemoveAt(0);
		}

		//LidarProcessing.GetWorldPosition()

		/// visualize
		//int i = 0;
		//for(int x = 0; x < PositionHistory.Count; x++)
		//{
		//	for(int y = 0; y < PositionHistory[x].Count; y++)
		//	{
		//		if(i < ManipulatedPool.Count)
		//		{
		//			ManipulatedPool[i].transform.position = new Vector3(PositionHistory[x][y].x, PositionHistory[x][y].y, x);
		//		}
		//		else
		//		{
		//				ManipulatedPool.Add(Instantiate(vPrefab));

		//			ManipulatedPool[i].transform.position = new Vector3(PositionHistory[x][y].x, PositionHistory[x][y].y, x);
		//		}
		//		i++;
		//	}
		//}

		m_vert.Clear();
		triangles.Clear();
		m_vert.Add(Vector3.zero);
		int i = 0;
		for (int x = 0; x < PositionHistory.Count; x++)
		{
			for (int y = 0; y < PositionHistory[x].Count; y++)
			{
				Vector3 inputV = new Vector3(PositionHistory[x][y].x, PositionHistory[x][y].y,x);
				//m_vert.Add(new Vector3(-inputV.x,inputV.y,0));
				m_vert.Add(inputV);
				i++;
			}
		}
		for (int d = 1; d < m_vert.Count; d++)
		{
			triangles.Add(d-1);
			triangles.Add(d);
			triangles.Add(d + 1);

		}
	

		m_mesh.SetVertices(m_vert);
		//m_mesh.SetIndices(m_ind.ToArray(), MeshTopology.Points, 0);
		m_mesh.SetTriangles(triangles.ToArray(), 0);


		m_mesh.UploadMeshData(false);
		m_meshfilter.mesh = m_mesh;
	}

	public static Vector2 LidarData2Transform(LidarDataAnd data)
	{
		Vector2 pos = data.t.position;
		return LidarProcessing.GetWorldPosition(pos, data.t.rotation.ToEulerAngles().z, data.L.theta, data.L.distant);
	}

	//public static Vector2 LidarData2World (LidarData data, Transform tran)
	//{
	//	return LidarProcessing.GetWorldPosition();
	//}
}

public class LidarDataAnd
{
	public LidarData L;
	public Transform t;

	public LidarDataAnd(LidarData l, Transform T)
	{
		L = l;
		t = T;
	}
}
