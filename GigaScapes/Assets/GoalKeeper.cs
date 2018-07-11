using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalKeeper : MonoBehaviour
{
    public bool homeGoal = false;
    private Collider2D thisCol;

    private void Start()
    {
        thisCol = gameObject.GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision Detected! " + this.name);

        if (collision.tag == "Asteroid")
        {
            GameManager.Instance.ScoreGoal(thisCol, collision.gameObject, homeGoal);
        }
    }
   
}
