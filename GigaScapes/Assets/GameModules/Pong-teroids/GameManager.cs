using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public float teamOneScore;
    public float teamTwoScore;

    public Collider2D TopGoal;
    public Collider2D BotGoal;

	// Use this for initialization
	void Start ()
    {
        teamOneScore = 0;
        teamTwoScore = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {

	}

    void topGoalScore()
    {
        teamTwoScore += 100;
    }

    void botGoalScore()
    {
        teamOneScore += 100;
    }


}
