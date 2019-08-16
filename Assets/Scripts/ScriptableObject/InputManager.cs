using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class InputManager : ScriptableObject
{
    public KeyCode up;
    public KeyCode down;
    public KeyCode left;
    public KeyCode right;
    public KeyCode dash;
    public KeyCode jump;
    public KeyCode climb;

    public string giveKeyCode(string name)
    {
        switch(name)
        {
            case "Up":
                return up.ToString();
            case "Down":
                return down.ToString();
            case "Left":
                return left.ToString();
            case "Right":
                return right.ToString();
            case "Dash":
                return dash.ToString();
            case "Jump":
                return jump.ToString();
            case "Climb":
                return climb.ToString();
            default:
                return null;
        }
    }

    public void setKey(string name, KeyCode newKey)
    {
        switch (name)
        {
            case "Up":
                up = newKey;
                break;
            case "Down":
                down = newKey;
                break;
            case "Left":
                left = newKey;
                break;
            case "Right":
                right = newKey;
                break;
            case "Dash":
                dash = newKey;
                break;
            case "Jump":
                jump = newKey;
                break;
            case "Climb":
                climb = newKey;
                break;
            default:
                break;
        }
    }

    public void Reset()
    {
        up = KeyCode.W;
        down = KeyCode.S;
        left = KeyCode.A;
        right = KeyCode.D;
        dash = KeyCode.J;
        jump = KeyCode.K;
        climb = KeyCode.L;
    }

    public bool checkValid(KeyCode newKey)
    {
        if (newKey == up || newKey == down || newKey == left || newKey == right || newKey == jump || newKey == climb || newKey == dash || newKey == KeyCode.None || newKey == KeyCode.Return || newKey == KeyCode.Escape)
            return false;
        else
            return true;
    }
}
