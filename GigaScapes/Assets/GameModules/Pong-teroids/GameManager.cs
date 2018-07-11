using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public float sDampTime = 1.0f;

    public float teamOneScore;
    public float teamTwoScore;

    public Collider2D TopGoal;
    public Collider2D BotGoal;

    public List<GameObject> CurrentLocalPlayers;

    public bool TrackMouse = false;
    Vector2 vec = Vector2.one;

    private GameObject Player;
    public List<GameObject> LocalPlayersObj;
    private Vector2 p1StartVector;
    private Vector2 p2StartVector;

    private Dictionary<string, GameObject> managedObjects;
    public Dictionary<string, GameObject> ManagedObjects { get; set; }

    private bool IsMaster = true;

    private void Awake()
    {
        ManagedObjects = new Dictionary<string, GameObject>();
        LocalPlayersObj = new List<GameObject>();
    }

    void Start ()
    {
        Instance = this;
        ResetGame();
        if(IsMaster)
            InvokeRepeating("SpawnAsteroid", 0.5f, 5.0f);

        LocalPlayersObj.Add(ObjectPooler.Instance.SpawnFromPool("Ship", new Vector2(0, 0)));
        LocalPlayersObj.Add(ObjectPooler.Instance.SpawnFromPool("Ship", new Vector2(0, 0)));

        p1StartVector = CurrentLocalPlayers[0].transform.position;
        p2StartVector = CurrentLocalPlayers[1].transform.position;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown("space"))
            SpawnAsteroid();

        if (Input.GetKeyDown("w"))
            SpawnMineL();
        if (Input.GetKeyDown("s"))
            SpawnMineR();

        //for(int i = 0; i < LocalPlayersObj.Count; i++)
        //MovePlayer(LocalPlayersObj[i],i);
        MovePlayer(LocalPlayersObj[0], 0);
    }

    public void AddToManagedDictionary(string NetID, GameObject obj)
    {
        ManagedObjects.Add(NetID, obj);
    }

    private void ResetGame()
    {
        teamOneScore = 0;
        teamTwoScore = 0;  
    }

    public void ScoreGoal(Collider2D collider, GameObject asteroid, bool homeTeam)
    {
       if(homeTeam == true && collider == TopGoal)
       {
            Debug.Log("Scored for the home team!");
       }
       else
        {
            Debug.Log("Other Team scored!");
        }
        //Debug.Log("ScoreGoal Called");
        ObjectPooler.Instance.ReturnToPool("Asteroid", asteroid);
    }

    //Blanket function to move the local players in the scene and report it to the network manager
    public void MovePlayer(GameObject player,int i)
    {
        Vector2 thisVec = player.transform.position;
        Vector2 mouseVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 targetVec = new Vector2((CurrentLocalPlayers[i].transform.position.x), (CurrentLocalPlayers[i].transform.position.z));

        if (TrackMouse)
        {
            player.transform.position = Vector2.SmoothDamp(thisVec, mouseVec, ref vec, sDampTime, 15.0f, Time.deltaTime);
            player.transform.right = mouseVec - thisVec;   
        }
        else
        {
            player.transform.position = Vector2.SmoothDamp(thisVec, targetVec, ref vec, sDampTime, 15.0f, Time.deltaTime);
            player.transform.right = targetVec - thisVec;
        }
    }

    public void MovePlayer(GameObject player, int i, Vector2 destination)
    {
        Vector2 thisVec = player.transform.position;
        //Vector2 mouseVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        
            player.transform.position = Vector2.SmoothDamp(thisVec, destination, ref vec, sDampTime, 15.0f, Time.deltaTime);
            player.transform.right = destination - thisVec;
        
    }

    //Blanket function to move a non-local-player entity in the game world and report it to the network manager
    public void MoveEntity(GameObject Entity, Vector2 moveDir, ContactPoint2D contact, float force)
    {
        Debug.Log("Moving Entity " + Entity.name);
        Rigidbody2D collided = Entity.GetComponent<Rigidbody2D>();
        collided.AddForceAtPosition(moveDir * force, contact.point);
    }

    //Override for handling local movements(as it has the impact point tracked.
    public void MoveEntity(GameObject Entity, Vector2 moveDir, Vector2 impactPoint, float force)
    {
        Debug.Log("Moving Entity " + Entity.name);
        Rigidbody2D collided = Entity.GetComponent<Rigidbody2D>();
        collided.AddForceAtPosition(moveDir * force, impactPoint);
    }

    public void SpawnAsteroid()
    {
        ObjectPooler.Instance.SpawnFromPool("Asteroid", new Vector2 (Random.Range(-9.0f,9.0f), Random.Range(-4.0f, 4.0f))); 
    }

    public void SpawnMineR()
    {
        GameObject obj = ObjectPooler.Instance.SpawnFromPool("Mine", new Vector2(Random.Range(11.0f, 15.0f), Random.Range(-9.0f, 9.0f)));
        MoveEntity(obj, new Vector2(-1,0), obj.transform.position, 50);
    }

    public void SpawnMineL()
    {
        GameObject obj = ObjectPooler.Instance.SpawnFromPool("Mine", new Vector2(Random.Range(-11.0f, -15.0f), Random.Range(-9.0f, 9.0f)));
        MoveEntity(obj, new Vector2(1, 0), obj.transform.position, 50);
    }

}
