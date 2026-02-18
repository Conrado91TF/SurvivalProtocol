using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    static PoolManager _instance;
    public static PoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("PoolManager");
                _instance = go.AddComponent<PoolManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    [Tooltip("Tamaño por defecto si no se ha creado pool explícitamente")]
    public int defaultPoolSize = 10;

    readonly Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();

    public void CreatePool(GameObject prefab, int initialSize)
    {
        if (prefab == null) return;
        if (pools.ContainsKey(prefab)) return;

        var q = new Queue<GameObject>();
        for (int i = 0; i < initialSize; i++)
        {
            var inst = Instantiate(prefab);
            inst.SetActive(false);
            var po = inst.GetComponent<PooledObject>();
            if (po == null) po = inst.AddComponent<PooledObject>();
            po.Prefab = prefab;
            q.Enqueue(inst);
        }
        pools.Add(prefab, q);
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null) return null;

        if (!pools.ContainsKey(prefab))
            CreatePool(prefab, defaultPoolSize);

        var q = pools[prefab];
        GameObject instance;
        if (q.Count > 0)
        {
            instance = q.Dequeue();
            if (instance == null)
            {
                instance = Instantiate(prefab);
            }
        }
        else
        {
            instance = Instantiate(prefab);
        }

        var po = instance.GetComponent<PooledObject>();
        if (po == null) po = instance.AddComponent<PooledObject>();
        po.Prefab = prefab;

        instance.transform.SetPositionAndRotation(position, rotation);
        instance.SetActive(true);
        return instance;
    }

    public void ReturnToPool(GameObject instance)
    {
        if (instance == null) return;
        var po = instance.GetComponent<PooledObject>();
        if (po == null || po.Prefab == null)
        {
            Destroy(instance);
            return;
        }

        instance.SetActive(false);

        if (!pools.ContainsKey(po.Prefab))
            pools.Add(po.Prefab, new Queue<GameObject>());

        pools[po.Prefab].Enqueue(instance);
    }

    // Helper component para recordar el prefab de origen
    class PooledObject : MonoBehaviour
    {
        public GameObject Prefab;
    }
}
