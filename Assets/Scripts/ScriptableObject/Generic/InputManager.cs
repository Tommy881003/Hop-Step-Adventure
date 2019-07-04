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
}
