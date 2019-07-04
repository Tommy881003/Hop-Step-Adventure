using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleBlock : MonoBehaviour {

    public int currentState = 0;
    private BoxCollider2D box;
    private CircleCollider2D circle;
    private PlayerControl player;
    private SpritePicker picker;
    private SpriteRenderer render;
    private Animator animator;
    private bool isReal;
    private bool once = true;
	
	void Start ()
    {
        box = this.GetComponent<BoxCollider2D>();
        circle = this.GetComponent<CircleCollider2D>();
        picker = this.GetComponent<SpritePicker>();
        animator = this.GetComponent<Animator>();
        render = this.GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        if (box.enabled == false)
        {
            circle.enabled = false;
            this.gameObject.layer = 2;
            render.sortingLayerName = "HighBackground";
            isReal = false;
        }
        else
        {
            circle.enabled = true;
            this.gameObject.layer = 0;
            render.sortingLayerName = "Foreground";
            isReal = true;
        }
    }

    
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.J) && player.canDash == true && player.isDashing == false)
         {
            animator.SetBool("Transitioning", true);
            currentState = (currentState + 1) % 2;
            if (box.enabled == true)
            {
                box.enabled = false;
                circle.enabled = false;
                this.gameObject.layer = 2;
                render.sortingLayerName = "HighBackground";
            }
            else
            {
                box.enabled = true;
                circle.enabled = true;
                this.gameObject.layer = 0;
                render.sortingLayerName = "Foreground";
            }
            picker.SimplePick(currentState);
        }
        else
            animator.SetBool("Transitioning", false);
        if ((player.dead == true || player.isTransitioning == true) && once)
            StartCoroutine(Reset());
    }

    IEnumerator Reset()
    {
        once = false;
        StartCoroutine(ResetOnce());
        if(player.dead == true)
            yield return new WaitForSecondsRealtime(0.8f);
        currentState = 0;
        if(isReal == false)
        {
            box.enabled = false;
            circle.enabled = false;
            this.gameObject.layer = 2;
            render.sortingLayerName = "HighBackground";
        }
        else
        {
            box.enabled = true;
            circle.enabled = true;
            this.gameObject.layer = 0;
            render.sortingLayerName = "Foreground";
        }
        picker.SimplePick(currentState);
    }

    IEnumerator ResetOnce()
    {
        if (player.dead == true)
            yield return new WaitUntil(() => player.dead == false);
        else
            yield return new WaitUntil(() => player.isTransitioning == false);
        once = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            player.dead = true;
    }
}
