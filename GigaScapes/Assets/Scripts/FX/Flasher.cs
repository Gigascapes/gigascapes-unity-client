using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flasher : MonoBehaviour {

    Light flasher;
    public AnimationCurve flashTimer;

	// Use this for initialization
	void Start ()
    {
        flasher = gameObject.GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        flasher.range = flashTimer.Evaluate(Time.time);
	}
}
