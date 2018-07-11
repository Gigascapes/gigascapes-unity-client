using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    private int PlayerID;
    Vector2 thisVec;
    Vector2 mouseVec;
    Vector2 targetVec;
    Vector2 vec = Vector2.one;
    public float sDampTime = 1.0f;

    //public GameObject trackerOBJ;

    Rigidbody2D shipRB;
    public GameObject shipShield;
    Collider2D shipShieldCol;
    public bool TrackMouse = true;


    public bool shieldDown = false;

	// Use this for initialization
	void Awake ()
    {
        shipRB = gameObject.GetComponentInChildren<Rigidbody2D>();
        shipShieldCol = gameObject.GetComponentInChildren<Collider2D>();
        //shipShield = shipShieldCol.gameObject;
    }

    private void FixedUpdate()
    {
        if (shieldDown)
            StartCoroutine("ShieldRegen");
    }


    // Update is called once per frame
    void Update ()
    {/*
        Vector2 thisVec = gameObject.transform.position;
        Vector2 mouseVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 targetVec = new Vector2((trackerOBJ.transform.position.x + 12), (trackerOBJ.transform.position.z + 26.6f));

        if(TrackMouse)
        {
            gameObject.transform.position =  Vector2.SmoothDamp(thisVec, mouseVec, ref vec, sDampTime, 15.0f, Time.deltaTime);
            transform.right = mouseVec - thisVec;
            
            Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            diff.Normalize();
 
            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
            
    }
        else
        {
            gameObject.transform.position = Vector2.SmoothDamp(thisVec, targetVec, ref vec, sDampTime, 15.0f, Time.deltaTime);
            transform.right = targetVec - thisVec;
        }
        */
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D collided;
        ContactPoint2D contact = collision.contacts[0];

        if(collision.gameObject.tag == "Asteroid")
        {
            /*
            collided = collision.gameObject.GetComponent<Rigidbody2D>();
            if(collided != null)
            {
                Debug.Log("Collision With Asteroid!");
                collided.AddForceAtPosition(new Vector2((collided.gameObject.transform.position.x - gameObject.transform.position.x) * 20,
                    (collided.gameObject.transform.position.y - gameObject.transform.position.y) * 50), 
                    contact.point);
            }
            */
            Debug.Log("Calling MoveEntity on " + collision.gameObject.name);
            GameManager.Instance.MoveEntity(collision.gameObject, new Vector2((collision.gameObject.transform.position.x - gameObject.transform.position.x),
                    (collision.gameObject.transform.position.y - gameObject.transform.position.y)), contact.point, 50);
                    
        }
        else if(collision.gameObject.tag == "Mine")
        {
            Debug.Log("Collision With Mine!");
            shieldDown = true;
        }
    }
    
    IEnumerator ShieldRegen()
    {
        shipShield.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        for(float f = 0.1f; f < 1; f+= 0.1f)
        {
            shipShield.transform.localScale = new Vector3(f,f,f);
            yield return null;
        }
        shieldDown = false;
        yield return null;
    }

    /*
    void forceCalc(Vector2 heading, float distance)
    {
        shipRB.AddForce(heading.normalized * forceOverDistance.Evaluate(distance));
    }
    */
}
