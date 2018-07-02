using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour {

    public bool isRandom = false;

    Transform spinTrans;
    public float spinForceX;
    public float spinForceY;
    public float spinForceZ;

	// Use this for initialization
	void Start () 
    {
        spinTrans = gameObject.GetComponent<Transform>();
        if (isRandom)
        {
            spinForceX = Random.Range(-2.0f, 2.0f);
            spinForceY = Random.Range(-2.0f, 2.0f);
            spinForceZ = Random.Range(-2.0f, 2.0f);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        spinTrans.Rotate(spinForceX, spinForceY ,spinForceZ);
	}
}
