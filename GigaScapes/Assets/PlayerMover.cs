using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    Vector2 thisVec;
    Vector2 mouseVec;
    Vector2 targetVec;
    Vector2 vec = Vector2.one;
    public float sDampTime = 1.0f;

    public GameObject trackerOBJ;

    Rigidbody2D shipRB;
    Collider2D shipShield;
    public AnimationCurve forceOverDistance;
    public bool TrackMouse = true;

	// Use this for initialization
	void Awake ()
    {
        shipRB = gameObject.GetComponentInChildren<Rigidbody2D>();
        shipShield = gameObject.GetComponentInChildren<Collider2D>();
    }

    //We'll use this to update the Player's target when pulled from the ObjectPool.
    private void OnEnable()
    {
        
    }

    //Used to send play info to the NetworkManager
    private void SendPlayerData()
    {

    }

    // Update is called once per frame
    void Update ()
    {
        thisVec = gameObject.transform.position;
        mouseVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetVec = new Vector2((trackerOBJ.transform.position.x + 12), (trackerOBJ.transform.position.z + 26.6f));

        if(TrackMouse)
        {
            //forceCalc(mouseVec - thisVec, (mouseVec - thisVec).magnitude);
            gameObject.transform.position =  Vector2.SmoothDamp(thisVec, mouseVec, ref vec, sDampTime, 15.0f, Time.deltaTime);
        }
        else
        {
            //forceCalc(targetVec - thisVec, (targetVec - thisVec).magnitude);
            gameObject.transform.position = Vector2.SmoothDamp(thisVec, targetVec, ref vec, sDampTime, 15.0f, Time.deltaTime);
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D collided;
        ContactPoint2D contact = collision.contacts[0];

        if(collision.gameObject.layer == 8)
        {
            collided = collision.gameObject.GetComponent<Rigidbody2D>();
            if(collided != null)
            {
                Debug.Log("Collision With Asteroid!");
                collided.AddForceAtPosition(new Vector2(collided.gameObject.transform.position.x - gameObject.transform.position.x,
                    collided.gameObject.transform.position.y - gameObject.transform.position.y), 
                    contact.point);
            }

        }
        else
            Debug.Log("Collision!");
    }

    void forceCalc(Vector2 heading, float distance)
    {
        shipRB.AddForce(heading.normalized * forceOverDistance.Evaluate(distance));
    }
}
