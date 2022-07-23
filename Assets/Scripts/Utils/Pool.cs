using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool<T> where T : MonoBehaviour
{
    private List<T> objects;
    private T prefab;
    private Transform parent;

    public List<T> ActiveItems => objects.FindAll(o => o.gameObject.activeSelf);
    public List<T> AllItems => objects;

    public Pool(T newPrefab, int initialPrefabs, Transform newParent)
    {
        objects = new List<T>();
        prefab = newPrefab;
        parent = newParent;

        for (int i = 0; i < initialPrefabs; i++)
        {
            var o = MonoBehaviour.Instantiate(prefab, parent);
            o.gameObject.SetActive(false);
            objects.Add(o);
        }
    }

    public T GetItem()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            T obj = objects[i];
            if (!obj.gameObject.activeSelf) return obj;
        }

        var p = MonoBehaviour.Instantiate(prefab, parent);
        objects.Add(p);

        return p;
    }
}
