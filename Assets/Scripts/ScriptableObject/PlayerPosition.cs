using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerPosition : ScriptableObject
{
    public Vector2 spawnPos;
    public int currentLevel;
    public Vector2 corner;
    public Vector2 corner2;
    public bool testMode;
}
