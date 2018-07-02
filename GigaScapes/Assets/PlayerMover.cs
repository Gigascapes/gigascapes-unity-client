using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    Vector2 thisVec;
    Vector2 mouseVec;
    Rigidbody2D shipRB;
    public AnimationCurve forceOverDistance;

	// Use this for initialization
	void Awake ()
    {
        shipRB = gameObject.GetComponentInChildren<Rigidbody2D>();

    }

    //We'll use this to update the Player's target when pulled from the ObjectPool.
    private void OnEnable()
    {
        
    }

    // Update is called once per frame
    void Update ()
    {
        thisVec = gameObject.transform.position;
        mouseVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 heading = mouseVec - thisVec;
        var distance = heading.magnitude;

        shipRB.AddForce(heading.normalized * forceOverDistance.Evaluate(distance));
    }
}
