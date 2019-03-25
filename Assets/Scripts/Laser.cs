using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {

    public bool isAcivate;
    private Animator animator;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.GetComponentInParent<LaserMachine>() != null)
            isAcivate = this.GetComponentInParent<LaserMachine>().isAcivate;
        animator.SetBool("isActivate", isAcivate);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            collision.gameObject.GetComponent<PlayerControl>().dead = true;
    }
}
