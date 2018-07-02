using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DetectorField : MonoBehaviour
{
	public GameObject DetectorPrefab;
	// Use this for initialization

	public int SquareSize = 6;

	private Vector3 LocalSize = Vector3.one;

	private float unitLength = 1;

	private Dictionary<int[], GameObject> Spawns = new Dictionary<int[], GameObject>();

	private Dictionary<int[], Detector> SpawnsD = new Dictionary<int[], Detector>();

	public Dictionary<int[], Detector> On = new Dictionary<int[], Detector>();

	void Start()
	{
		SquareSize = SquareSize - SquareSize % 2;

		LocalSize *= 1f / (float)SquareSize;
		unitLength = 1f / (float)SquareSize;

		if (DetectorPrefab != null)
		{
			for(int x = -SquareSize/2; x < 1 + SquareSize / 2; x++)
			{
				for (int y = -SquareSize / 2; y < 1 + SquareSize / 2; y++)
				{
					GameObject spawn = Instantiate(DetectorPrefab) as GameObject;
					spawn.transform.parent = transform;
					spawn.transform.localRotation = Quaternion.identity;
					spawn.transform.localScale = LocalSize;
					spawn.transform.localPosition = new Vector3(x * unitLength, 0, y * unitLength);
					Spawns.Add(new int[] { x, y }, spawn);
				}
			}
		}

		SpawnsD = Spawns.ToDictionary(p => p.Key, p => p.Value.GetComponent<Detector>());
	}

	bool Func(int[] x, Detector y)
	{
		return y.on;
	}

	// Update is called once per frame
	void Update()
	{
		On = SpawnsD.Where(i => i.Value.on).ToDictionary(p => p.Key, p => p.Value);

		foreach (KeyValuePair<int[], Detector> k in On)
		{
			Vector3 point = new Vector3(unitLength * (float)k.Key[0], 0, unitLength * (float)k.Key[1]);
			Debug.DrawLine(point * transform.localScale.x, point * transform.localScale.x +Vector3.up);
		}
	}
}
