using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Followed this tut: https://www.youtube.com/watch?v=tdSmKaJvCoA&t=907s
/// </summary>

public class ObjectPooler : MonoBehaviour
{

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
        private int counter = 0;
        public Transform poolParent;

        public int getCounter() {
            return counter;
        }

        public void IncrementCounter(int num = 1) {
            counter += num;
        }
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    #region singleton
    private static ObjectPooler _instance;
    public static ObjectPooler instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }
    #endregion

    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            GameObject parent;

            if (pool.poolParent == null)
            {
                parent = new GameObject(pool.tag + "_parent");
            }
            else
            {
                parent = pool.poolParent.gameObject;
            }

            Queue<GameObject> objectPool = new Queue<GameObject>();

            int poolSize = pool.size;

            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab, pool.poolParent);
                obj.name = pool.tag + " " + pool.getCounter();
                pool.IncrementCounter();
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, int growAmt = 10)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("pool with tag " + tag + " doesnt exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);

        GrowPool(tag, objectToSpawn, growAmt);

        return objectToSpawn;
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, int growAmt = 10)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("pool with tag " + tag + " doesnt exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.transform.position = position;
        objectToSpawn.SetActive(true);

        GrowPool(tag, objectToSpawn, growAmt);

        return objectToSpawn;
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, int growAmt = 10)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("pool with tag " + tag + " doesnt exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        GrowPool(tag, objectToSpawn, growAmt);

        return objectToSpawn;
    }

    private void GrowPool(string tag, GameObject obj, int growAmt)
    {
        if (poolDictionary[tag].Count <= 2)
        {
            Pool p = null;
            foreach (Pool pool in pools)
            {
                if (pool.tag == tag)
                {
                    p = pool;
                    break;
                }
            }

            for (int i = 0; i < growAmt; i++)
            {
                GameObject clone = Instantiate(obj);
                clone.name = tag + " " + p.getCounter();
                p.IncrementCounter();
                clone.transform.parent = obj.transform.parent;
                clone.SetActive(false);
                poolDictionary[tag].Enqueue(clone);
            }
        }
    }

    public void PutBackIntoPool(string tag, GameObject obj)
    {
        obj.SetActive(false);
        poolDictionary[tag].Enqueue(obj);
    }


    //Testing purposes:
    //void Update() {
    //    if (Input.GetKeyDown(KeyCode.Space)) {
    //        SpawnFromPool("cube", transform.position, Quaternion.identity);
    //    }
    //}


}
