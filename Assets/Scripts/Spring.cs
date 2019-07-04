using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour {

    private Animator animator;
    private AnimatorStateInfo CurrentStateInfo;
    private int rotation;
    private float jumpY = 27.5f;
    private float jumpX = 20;


    private void Start()
    {
        animator = this.GetComponent<Animator>();
        rotation = (int)(this.transform.rotation.eulerAngles.z);
        switch(rotation)
        {
            case 0:
                jumpY = 27.5f;
                jumpX = 0;
                break;
            case 90:
                jumpY = 10;
                jumpX = -20;
                break;
            case 180:
                jumpY = -27.5f;
                jumpX = 0;
                break;
            default:
                jumpY = 10;
                jumpX = 20;
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            animator.SetBool("PlayerPress", true);
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            PlayerControl player = collision.gameObject.GetComponent<PlayerControl>();
            player.isDashing = false;
            if (jumpX != 0)
            {
                player.canControlMove = false;
                player.springJump = true;
                StartCoroutine(player.Spring());
            }
            StartCoroutine(DashReset(player));
            rb.velocity = new Vector2(jumpX,jumpY);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            animator.SetBool("PlayerPress", true);
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            PlayerControl player = collision.gameObject.GetComponent<PlayerControl>();
            player.isDashing = false;
            if (jumpX != 0)
            {
                player.canControlMove = false;
                player.springJump = true;
                StartCoroutine(player.Spring());
            }
            StartCoroutine(DashReset(player));
            rb.velocity = new Vector2(jumpX, jumpY);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            animator.SetBool("PlayerPress", false);
        }

    }

    IEnumerator DashReset(PlayerControl player)
    {
        yield return new WaitForSeconds(0.2f);
        if(!player.isDashing)
            player.canDash = true;
    }
}
