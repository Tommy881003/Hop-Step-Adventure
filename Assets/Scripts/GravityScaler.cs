using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityScaler : MonoBehaviour
{
    private Animator animator;
    private bool down;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        if (Physics2D.gravity.y > 0)
            down = false;
        else
            down = true;
        animator.SetBool("Down", down);
    }

    private void Update()
    {
        if (Physics2D.gravity.y > 0)
            down = false;
        else
            down = true;
        animator.SetBool("Down", down);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Physics2D.gravity = -Physics2D.gravity;
            down = !down;
            animator.SetBool("Down", down);
        }
    }
}
