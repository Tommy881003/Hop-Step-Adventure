using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCarrier : MonoBehaviour
{
    [HideInInspector]
    public int[] X = new int[1001];
    [HideInInspector]
    public int[] Y = new int[1001];

    private int count = 0;
    public Vector2 max = Vector2.zero, min = Vector2.zero;

    public void Insert(Vector2 value)
    {
        int VX = (int)(value.x * 2) + 500, VY = (int)(value.y * 2) + 500;
        X[VX]++;
        Y[VY]++;
        if (count == 0 || max.x < value.x)
            max.x = value.x;
        if (count == 0 || max.y < value.y)
            max.y = value.y;
        if (count == 0 || min.x > value.x)
            min.x = value.x;
        if (count == 0 || min.y > value.y)
            min.y = value.y;
        count++;
    }

    public void Delete(Vector2 value)
    {
        int VX = (int)(value.x * 2) + 500, VY = (int)(value.y * 2) + 500;
        X[VX]--;
        Y[VY]--;
        if (value.x == max.x && X[VX] == 0)
        {
            for(int i = 1000; i >= 0; i --)
            {
                if(X[i] != 0)
                {
                    max.x = (float)((float)(VX) - 500) / (float)2;
                    break;
                }
            }
        }
        if (value.x == min.x && X[VX] == 0)
        {
            for (int i = 0; i < 1001; i++)
            {
                if (X[i] != 0)
                {
                    min.x = (float)((float)(VX) - 500) / (float)2;
                    break;
                }
            }
        }
        if (value.y == max.y && Y[VY] == 0)
        {
            for (int i = 1000; i >= 0; i--)
            {
                if (Y[i] != 0)
                {
                    max.y = (float)((float)(VY) - 500) / (float)2;
                    break;
                }
            }
        }
        if (value.y == min.y && Y[VY] == 0)
        {
            for (int i = 0; i < 1001; i++)
            {
                if (Y[i] != 0)
                {
                    min.y = (float)((float)(VY) - 500) / (float)2;
                    break;
                }
            }
        }
        count--;
    }
}
