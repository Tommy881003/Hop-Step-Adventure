using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBlock : MonoBehaviour
{
    private SpriteRenderer sr;

    private void Start()
    {
        sr = this.GetComponent<SpriteRenderer>();
        sr.enabled = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            collision.gameObject.GetComponent<PlayerControl>().dead = true;
    }
}
