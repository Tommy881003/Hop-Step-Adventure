using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Console : MonoBehaviour
{
    private PlayerControl player;
    private Animator anim;
    private BoxCollider2D box;
    private CameraHolder holder;
    
    void Start()
    {
        box = this.GetComponent<BoxCollider2D>();
        anim = this.GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        holder = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraHolder>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            player.complete = true;
            box.isTrigger = true;
            StartCoroutine(EndLevel());
        }
    }

    IEnumerator EndLevel()
    {
        yield return new WaitUntil(() => player.onGround == true);
        yield return new WaitForSeconds(1.5f);
        anim.enabled = true;
        yield return new WaitForSeconds(3.5f);
        holder.EndLevel();
    }
}
