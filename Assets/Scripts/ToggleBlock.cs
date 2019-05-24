using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleBlock : MonoBehaviour {

    public int currentState;
    private int initial;
    private BoxCollider2D box;
    private CircleCollider2D circle;
    private PlayerControl player;
    private SpritePicker picker;
    private SpriteRenderer render;
    private Animator animator;
	
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
            currentState = 0;
            this.gameObject.layer = 2;
            render.sortingLayerName = "HighBackground";
        }
        else
        {
            circle.enabled = true;
            currentState = 1;
            this.gameObject.layer = 0;
            render.sortingLayerName = "Foreground";
        }
        initial = currentState;
    }

    
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.J) && player.canDash == true)
         {
            animator.SetBool("Transitioning", true);
            currentState = (currentState + 1) % 2;
            if (currentState == 0)
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
        if (player.dead == true || player.isTransitioning == true)
            StartCoroutine(Reset());
    }

    IEnumerator Reset()
    {
        if(player.dead == true)
            yield return new WaitForSecondsRealtime(0.5f);
        currentState = initial;
        if (currentState == 0)
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            player.dead = true;
    }
}
