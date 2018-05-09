using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activator : MonoBehaviour {

	private int ind = 0;

	public List<GameObject> GOs = new List<GameObject>();


	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
		{
			if(ind < GOs.Count)
			{
				GOs[ind].SetActive(true);
				ind++;
			}
		}
	}
}
