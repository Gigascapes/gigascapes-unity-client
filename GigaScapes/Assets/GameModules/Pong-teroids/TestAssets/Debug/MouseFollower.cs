using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Small script used to track the mouse movement over a few frames.

public class MouseFollower : MonoBehaviour
{
    private void Awake()
    {
        //Hard setting the framerate to 60, as my machine runs WAY faster than that...
        Application.targetFrameRate = 60;
    }

    public GameObject tracker;
    private Queue<GameObject> trackingQueue;

    private void Start()
    {
        InvokeRepeating("SpawnTracker", 0.0f, 0.05f);
        trackingQueue = new Queue<GameObject>(0);
    }

    //Function that is invoked 1/20 frames. Handles all mouse tracker spawning and despawning. Warning: Inefficient!!! (Need to build a little object pooler for this)
    public void SpawnTracker()
    {
        Vector2 v2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 v3 = new Vector3(v2.x, v2.y, 0);

        gameObject.transform.position = v3;

        GameObject temp = Instantiate(tracker, v3, Quaternion.identity);

        //Debug.Log("Spawning Tracker " + temp.name + " at position " + v3.x + " " + v3.y);

        trackingQueue.Enqueue(temp);

        if(trackingQueue.Count > 3)
        {
            Destroy(trackingQueue.Dequeue());
        }
    }
}
