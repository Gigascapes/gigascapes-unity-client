using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Detector : MonoBehaviour
{
	Rigidbody rb;
	Renderer rend;
	MeshFilter mf;
	Collider Col;

	public bool collided = false;

	public bool on = false;

	public List<int> History = new List<int>() { 0, 0, 0, 0, 0 };

	// Use this for initialization
	void Start()
	{

		
		Physics.IgnoreLayerCollision(0, 8);

		rb = GetComponent<Rigidbody>();
		rend = GetComponent<Renderer>();
		mf = GetComponent<MeshFilter>();
		Col = GetComponent<Collider>();
		//Invoke("Shutoff", 0.5f);

		rend.enabled = false;
	}

	//private void OnCollisionEnter(Collision collision)
	//{
	//	if (true/*!this.enabled*/)
	//	{
	//		//this.enabled = true;
	//		collided = true;
	//	}
	//	else
	//	{
	//		//this.CancelInvoke();
	//		//Invoke("Shutoff", 0.5f);
	//	}
	//}
	//private void OnCollisionStay(Collision collision)
	//{
	//	if (true/*!this.enabled*/)
	//	{
	//		//this.enabled = true;
	//		collided = true;
	//	}
	//	else
	//	{
	//		//this.CancelInvoke();
	//		//Invoke("Shutoff", 0.5f);
	//	}
	//}

	void Shutoff()
	{
		this.enabled = false;
	}

	private void Update()
	{
        if (Physics.CheckBox(transform.position, transform.localScale * 0.6f, transform.rotation, 1 << 8))
        {
            collided = true;
        }

        if (collided)
        {
            History.Add(1);

            collided = false;
        }
        else
        {
            History.Add(0);
        }

        while (History.Count > 10)
        {
            History.RemoveAt(0);
        }

        ///if (History.FindAll(x => x == 1).Count > 2)
        if (History[0] == 1)
            {
			//rend.enabled = false;
			on = false;
			//rend.material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f),0.7f);
		}
		else
		{
			//rend.enabled = true;
			on = true;
			//rend.material.color = new Color(1, 1, 1, 0.7f);
		}

	}

	private void LateUpdate()
	{
		

	}
}
