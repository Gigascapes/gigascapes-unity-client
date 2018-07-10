using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    public float teamOneScore;
    public float teamTwoScore;

    public Collider2D TopGoal;
    public Collider2D BotGoal;

    [SerializeField]
    public List<GameObject> AsteroidTypes;
    [SerializeField]
    public List<GameObject> ShipTypes;
    [SerializeField]
    public List<GameObject> MineTypes;

    private List<TrackableObject> TrackedList;
    //private Dictionary<> KillList;

    void Start ()
    {
        Instance = this;
        ResetGame();
        InvokeRepeating("SpawnAsteroid", 0.5f, 5.0f);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown("space"))
            SpawnAsteroid();
	}

    private void ResetGame()
    {
        teamOneScore = 0;
        teamTwoScore = 0;
        
    }

    private void ClearBoard()
    {

    }

    public void ScoreGoal(Collider2D collider, GameObject asteroid, bool homeTeam)
    {
       if(homeTeam == true && collider == BotGoal)
       {
            Debug.Log("Scored for the home team!");
       }
       else
        {
            Debug.Log("Other Team scored!");
        }
        Debug.Log("ScoreGoal Called");
        ObjectPooler.Instance.ReturnToPool("Asteroid", asteroid);
    }

    public void SpawnAsteroid()
    {
        ObjectPooler.Instance.SpawnFromPool("Asteroid", new Vector2 (Random.Range(-9.0f,9.0f), Random.Range(-4.0f, 4.0f)));
    }

   

}
