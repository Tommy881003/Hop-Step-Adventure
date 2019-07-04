using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Map : MonoBehaviour 
{
    [HideInInspector]
    public GameObject levelParent,followCursor;
    public GameObject levelPrefab;
    public Vector2Int toLink = new Vector2Int(-1, -1);
    public int currentLevel = -1;

    public List<GameObject> levels = new List<GameObject>();

    public bool checkValid(int a)
    {
        if (a < 0 || a >= levels.Count() || levels[a] == null)
            return false;
        else
            return true;
    }

    public bool checkValid(int a, int b)
    {
        if (a < 0 || a >= levels.Count() || levels[a] == null || b < 0 || b >= levels.Count() || levels[b] == null || a == b)
            return false;
        else
            return true;
    }

    public int haveEmpty()
    {
        int count = levels.Count();
        for (int i = 0; i < count; i++)
        {
            if (levels[i] == null)
                return i;
        }
        return -1;
    }

    public int addLevel(GameObject level, int i)
    {
        if (levels.Exists(x => x == level) == false)
        {
            if (i < 0)
                levels.Add(level);
            else
                levels[i] = level;
        }
        return ((i < 0) ? levels.Count() - 1 : i);
    }

    public void removeLevel(int a ,bool destroy)
    {
        List<int> link = levels[a].GetComponent<Transition>().link;
        foreach (int i in link)
        {
            List<int> toRemove = levels[i].GetComponent<Transition>().link;
            toRemove.Remove(a);
        }
        if (destroy)
            DestroyImmediate(levels[a]);
        else
            levels[a] = null;
    }

    public void buildLink(int a, int b) //true if build successful, false if not
    {
        List<int> linkA = levels[a].GetComponent<Transition>().link;
        List<int> linkB = levels[b].GetComponent<Transition>().link;
        if (linkA.Exists(x => x == b) == false)
        {
            linkA.Add(b);
            linkB.Add(a);
        }
    }

    public void removeLink(int a, int b) //true if remove successful, false if not
    {
        List<int> linkA = levels[a].GetComponent<Transition>().link;
        List<int> linkB = levels[b].GetComponent<Transition>().link;
        if (linkA.Exists(x => x == b) == true)
        {
            linkA.Remove(b);
            linkB.Remove(a);
        }
    }
}
