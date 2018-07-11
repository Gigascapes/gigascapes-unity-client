using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{

    [System.Serializable]
    public class Pool
    {
        public string ID;
        public List<GameObject> prefab;
        public int size;
        private int Enabled = 0;
    }

    #region Singleton

    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    public Dictionary<string, GameObject> ManagedObjects;

    void Start ()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        ManagedObjects = new Dictionary<string, GameObject>();

        foreach(Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab[Random.Range(0, pool.prefab.Count-1)]);
                string netid = Random.Range(10000, 99999).ToString();
                obj.GetComponent<NetworkID>().NetID = netid;
                ManagedObjects.Add(netid, obj);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.ID, objectPool);
        }
	}
	

	public GameObject SpawnFromPool(string tag, Vector2 position)
    {
        if(!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }
        else if(poolDictionary[tag].Count <= 0)
        {
            Debug.LogWarning("Warning: Trying to spawn from " + tag + " pool, but it is empty.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;

        //poolDictionary[tag].Enqueue(objectToSpawn);
        return objectToSpawn;
    }

    public GameObject ReturnToPool(string tag, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }
        obj.SetActive(false);
        obj.transform.position = new Vector2(0, 0);
        poolDictionary[tag].Enqueue(obj);
        return obj;
    }
}
