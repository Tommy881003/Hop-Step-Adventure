using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    private GameObject left, median, right;
    private Rigidbody2D player;
    private PlayerControl control;
    private Rigidbody2D back;
    private GameObject cam;

    void Start()
    {
        left = GameObject.Find("BL");
        median = GameObject.Find("BM");
        right = GameObject.Find("BR");
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        control = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        back = this.GetComponent<Rigidbody2D>();
        cam = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void Update()
    {
        if(control.isDashing == false || Mathf.Abs(player.velocity.magnitude) < 10)
            back.velocity = new Vector2(player.velocity.x * 0.3f - 0.5f, 0);
        else
            back.velocity = new Vector2(-0.5f, 0);
        float distL = Mathf.Abs(left.transform.position.x - cam.transform.position.x);
        float distR = Mathf.Abs(right.transform.position.x - cam.transform.position.x);
        if (distL < 10)
        {
            right.transform.position = new Vector2(left.transform.position.x - 40, right.transform.position.y);
            GameObject temp = right;
            right = median;
            median = left;
            left = temp;
        }
        if (distR < 10)
        {
            left.transform.position = new Vector2(right.transform.position.x + 40, right.transform.position.y);
            GameObject temp = left;
            left = median;
            median = right;
            right = temp;
        }
    }
}
