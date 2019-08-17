using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Counter : ScriptableObject
{
    [HideInInspector]
    public float timer = 0;
    public bool enableTimer = false;
    public int HardDiskAmount = 0;
    public int MaxHardDiskAmount = 16;
    public int DieCounter = 0;

    public void Initialize()
    {
        timer = 0;
        HardDiskAmount = 0;
        DieCounter = 0;
    }
}
