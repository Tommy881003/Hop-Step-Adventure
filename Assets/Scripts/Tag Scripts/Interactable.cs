using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public int level;
    private PlayerControl control;
    private MonoBehaviour[] objs;
    private bool once = true;

    private void Start()
    {
        objs = GetComponents<MonoBehaviour>();
        foreach(MonoBehaviour obj in objs)
        {
            if (obj != this)
                obj.enabled = false;
        }
        this.GetComponent<SpriteRenderer>().enabled = true;
        control = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
    }

    private void Update()
    {
        if (level == control.currentLevel)
            EnableAll();
        else if (once)
            CloseAll();
    }
    
    void EnableAll()
    {
        once = true;
        foreach (MonoBehaviour obj in objs)
        {
            obj.enabled = true;
        }
    }

    void CloseAll()
    {
        once = false;
        foreach (MonoBehaviour obj in objs)
        {
            if (obj != this)
                obj.enabled = false;
        }
        this.GetComponent<SpriteRenderer>().enabled = true;
    }
}
