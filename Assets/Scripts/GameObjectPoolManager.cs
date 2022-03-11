using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPoolManager : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string m_Tag;
        public GameObject m_Prefab;
        public int m_Size;
    }

    #region Singleton
    public static GameObjectPoolManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    #region Variables
    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    #endregion

    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // Creates a queue of instantiated objects for each pool
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.m_Size; ++i)
            {
                GameObject obj = Instantiate(pool.m_Prefab);
                obj.SetActive(false);
                obj.transform.SetParent(transform); // Keep the inactive objects inside the pool manager hierarchy
                objectPool.Enqueue(obj);
            }

            // Fills the pool dict with each queue
            poolDictionary.Add(pool.m_Tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag: " + tag + " , does not exist!");
            return  null;
        }

        // Initialises the game object stored in this queue
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // Add this object back to the queue so it can be reused
        poolDictionary[tag].Enqueue(objectToSpawn);

        Debug.Log(objectToSpawn);
        return objectToSpawn;
    }

    public bool IsInitialised()
    {
        if (poolDictionary == null || pools == null)
            return false;

        if (poolDictionary.Count != pools.Count)
            return false;

        return true;
    }
}
