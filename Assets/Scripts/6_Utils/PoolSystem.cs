using System.Collections.Generic;
using UnityEngine;

public class PoolSystem : MonoBehaviour
{
    [SerializeField] Transform root;
    [SerializeField] string folder;
    [SerializeField] GameObject prefab;
    [SerializeField] List<GameObject> prefabs;

    private Dictionary<string, Queue<Transform>> pools = new Dictionary<string, Queue<Transform>>();

    private void Awake()
    {
        if (!string.IsNullOrEmpty(folder))
        {
            prefabs.AddRange(Resources.LoadAll<GameObject>(folder));
        }
        if (prefab != null)
        {
            prefab.SetActive(false);
        }
        if (prefabs != null)
        {
            for (int i = 0; i < prefabs.Count; i++)
            {
                prefabs[i].SetActive(false);
            }
        }
    }

    public Transform GetUIObjectFromPool()
    {
        return GetObjectFromPool(prefab, Vector3.zero, Quaternion.identity, Vector3.one, true);
    }

    public Transform GetObjectFromPool()
    {
        return GetObjectFromPool(prefab, Vector3.zero, Quaternion.identity, Vector3.one);
    }

    public Transform GetObjectFromPool(string prafabName, bool isLocal = false)
    {
        return GetObjectFromPool(prefabs.Find(x => x.name == prafabName), Vector3.zero, Quaternion.identity, Vector3.one, isLocal);
    }

    public Transform GetObjectFromPool(string prafabName, Vector3 pos, bool isLocal = false)
    {
        return GetObjectFromPool(prefabs.Find(x => x.name == prafabName), pos, Quaternion.identity, Vector3.one, isLocal);
    }

    private Transform GetObjectFromPool(GameObject prefab, Vector3 pos, Quaternion rot, Vector3 scale, bool isLocal = false)
    {
        if (prefab == null)
        {
            return null;
        }

        string key = prefab.name;

        if (!pools.ContainsKey(key))
        {
            pools.Add(key, new Queue<Transform>());
        }

        Queue<Transform> q = pools[key];
        if (q.Count > 0)
        {
            Transform objTr = q.Dequeue();
            objTr.SetParent(root);
            if (isLocal)
            {
                objTr.localPosition = pos;
                objTr.localRotation = rot;
            }
            else
            {
                objTr.position = pos;
                objTr.rotation = rot;
            }
            objTr.localScale = scale;
            return objTr;
        }
        else
        {
            Transform newTr = Instantiate(prefab).transform;
            newTr.name = key;
            newTr.SetParent(root);
            if (isLocal)
            {
                newTr.localPosition = pos;
                newTr.localRotation = rot;
            }
            else
            {
                newTr.position = pos;
                newTr.rotation = rot;
            }
            newTr.localScale = scale;
            return newTr;
        }
    }

    public void InitPrefab(GameObject prefab)
    {
        this.prefab = prefab;
    }

    public void InitPrefabs(List<GameObject> prefabs)
    {
        this.prefabs = prefabs;
    }

    public void Clear()
    {
        List<Transform> childs = new List<Transform>();
        foreach (Transform child in root)
        {
            childs.Add(child);
        }

        for (int i = 0; i < childs.Count; i++)
        {
            Return(childs[i]);
        }
    }

    public void Return(Transform tr)
    {
        pools[tr.name].Enqueue(tr);
        tr.SetParent(transform);
        tr.gameObject.SetActive(false);
    }
}
