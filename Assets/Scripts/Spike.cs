using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    private Vector2 normalVector;

    private void Start()
    {
        int rotation = (int)(this.transform.rotation.eulerAngles.z);
        switch (rotation)
        {
            case 0:
                normalVector = Vector2.up;
                break;
            case 90:
                normalVector = Vector2.left;
                break;
            case 180:
                normalVector = Vector2.down;
                break;
            default:
                normalVector = Vector2.right;
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Rigidbody2D player = collision.gameObject.GetComponent<Rigidbody2D>();
            if (Vector2.Dot(player.velocity.normalized, normalVector) == 0)
                collision.gameObject.GetComponent<PlayerControl>().dead = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Rigidbody2D player = collision.gameObject.GetComponent<Rigidbody2D>();
            if (Vector2.Dot(player.velocity.normalized, normalVector) == 0)
                collision.gameObject.GetComponent<PlayerControl>().dead = true;
        }
    }
}
