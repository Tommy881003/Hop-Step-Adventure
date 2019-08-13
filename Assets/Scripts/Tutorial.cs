using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Tutorial : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshPro left, right, up, down, jump, climb, dash, left2, right2, up2, down2;
    [SerializeField]
    private InputManager input;
    private PlayerControl player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
    }

    private void Update()
    {
        if (Time.timeScale < 1 || player.isCutScene || player.dead)
            return;
        if (left2 != null)
            left2.color = (Input.GetKey(input.left) ? Color.yellow : Color.white);
        if (right2 != null)
            right2.color = (Input.GetKey(input.right) ? Color.yellow : Color.white);
        if (up2 != null)
            up2.color = (Input.GetKey(input.up) ? Color.yellow : Color.white);
        if (down2 != null)
            down2.color = (Input.GetKey(input.down) ? Color.yellow : Color.white);
        if (left != null)
        {
            left.text = input.left.ToString() + " <-";
            left.color = (Input.GetKey(input.left) ? Color.yellow : Color.white);
        }
        if (right != null)
        {
            right.text = "-> " + input.right.ToString();
            right.color = (Input.GetKey(input.right) ? Color.yellow : Color.white);
        }
        if (up != null)
        {
            up.text = "up: " + input.up.ToString();
            up.color = (Input.GetKey(input.up) ? Color.yellow : Color.white);
        }
        if (down != null)
        {
            down.text = "down: " + input.down.ToString();
            down.color = (Input.GetKey(input.down) ? Color.yellow : Color.white);
        }  
        if (jump != null)
        {
            jump.text = "jump: " + input.jump.ToString();
            jump.color = (Input.GetKey(input.jump) ? Color.yellow : Color.white);
        }
        if (climb != null)
        {
            climb.text = "climb: " + input.climb.ToString() + " (hold)";
            climb.color = (Input.GetKey(input.climb) ? Color.yellow : Color.white);
        }
        if (dash != null)
        {
            dash.text = "dash: " + input.dash.ToString();
            dash.color = (Input.GetKey(input.dash) ? Color.yellow : Color.white);
        }
    }
}
