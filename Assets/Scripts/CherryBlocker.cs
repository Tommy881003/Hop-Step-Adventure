using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherryBlocker : MonoBehaviour
{
    private SpriteRenderer sr;

    private void Start()
    {
        sr = this.GetComponent<SpriteRenderer>();
        sr.enabled = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            collision.gameObject.GetComponent<PlayerControl>().canCollect = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            collision.gameObject.GetComponent<PlayerControl>().canCollect = true;
    }
}
