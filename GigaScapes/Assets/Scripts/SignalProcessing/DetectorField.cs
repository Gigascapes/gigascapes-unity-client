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
    public Dictionary<int[], Detector> On0 = new Dictionary<int[], Detector>();
    public Dictionary<int[], Detector> PosDelta = new Dictionary<int[], Detector>();
    public Dictionary<int[], Detector> NegDelta = new Dictionary<int[], Detector>();

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

        PosDelta.Clear();
        NegDelta.Clear();

        foreach (KeyValuePair<int[], Detector> kvp in SpawnsD)
        {
            if (kvp.Value.History[0] > kvp.Value.History[1])
            {
                PosDelta.Add(kvp.Key, kvp.Value);
            }
            else if (kvp.Value.History[0] == 0 && kvp.Value.History[1] == 1 && kvp.Value.History[2] == 1)
            {
                NegDelta.Add(kvp.Key, kvp.Value);
            }
        }

            //foreach (KeyValuePair<int[], Detector> kvp in On)
            //{
            //    if (!On0.Contains(kvp))
            //    {
            //        if (!PosDelta.Contains(kvp))
            //        {
            //            PosDelta.Add(kvp.Key, kvp.Value);
            //        }
            //        else
            //        {
            //            PosDelta[kvp.Key] = kvp.Value;
            //        }
            //    }
            //}

            //foreach (KeyValuePair<int[], Detector> kvp in On0)
            //{
            //    if (!On.Contains(kvp))
            //    {
            //        NegDelta.Add(kvp.Key, kvp.Value);
            //    }
            //    else
            //    {
            //        NegDelta[kvp.Key] = kvp.Value;
            //    }
            //}

            foreach (KeyValuePair<int[], Detector> k in PosDelta)
        {
            Vector3 point = transform.localToWorldMatrix.MultiplyPoint(new Vector3(unitLength * (float)k.Key[0], 0, unitLength * (float)k.Key[1]));
            Debug.DrawLine(point, point+ Vector3.up,Color.green);
        }
        foreach (KeyValuePair<int[], Detector> k in NegDelta)
        {
            Vector3 point = transform.localToWorldMatrix.MultiplyPoint(new Vector3(unitLength * (float)k.Key[0], 0, unitLength * (float)k.Key[1]));
            Debug.DrawLine(point, point + Vector3.up, Color.red);
        }


        if (true)
        {
            Vector3 Corner1 = transform.localToWorldMatrix.MultiplyPoint(new Vector3(0.5f, 0, 0.5f));
            Vector3 Corner2 = transform.localToWorldMatrix.MultiplyPoint(new Vector3(-0.5f, 0, 0.5f));
            Vector3 Corner3 = transform.localToWorldMatrix.MultiplyPoint(new Vector3(-0.5f, 0, -0.5f));
            Vector3 Corner4 = transform.localToWorldMatrix.MultiplyPoint(new Vector3(0.5f, 0, -0.5f));

            Debug.DrawLine(Corner1, Corner1 + Vector3.up, Color.blue);
            Debug.DrawLine(Corner2, Corner2 + Vector3.up, Color.blue);
            Debug.DrawLine(Corner3, Corner3 + Vector3.up, Color.blue);
            Debug.DrawLine(Corner4, Corner4 + Vector3.up, Color.blue);
        }

        //foreach (KeyValuePair<int[], Detector> k in On)
        //{
        //	Vector3 point = new Vector3(unitLength * (float)k.Key[0], 0, unitLength * (float)k.Key[1]);
        //	Debug.DrawLine(point * transform.localScale.x, point * transform.localScale.x +Vector3.up);
        //}


    }

    private void LateUpdate()
    {
        On0 = On;
    }

    public Vector3 FromLocalToWorld(Vector3 loc)
    {
        //  Vector3 shape = transform.localPosition;

        //Vector3 ans = new Vector3(loc.x*shape.x,loc.y*shape.y,loc.shape)

        return transform.localToWorldMatrix.MultiplyPoint(loc);
    }
}
