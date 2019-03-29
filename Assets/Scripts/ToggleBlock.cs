using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleBlock : MonoBehaviour {

    public int currentState;
    private int initial;
    private BoxCollider2D box;
    private PlayerControl player;
    private SpritePicker picker;
    private SpriteRenderer render;
    private Animator animator;
	
	void Start ()
    {
        box = this.GetComponent<BoxCollider2D>();
        picker = this.GetComponent<SpritePicker>();
        animator = this.GetComponent<Animator>();
        render = this.GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        if (box.isTrigger)
        {
            currentState = 0;
            this.gameObject.layer = 2;
        }
        else
        {
            currentState = 1;
            this.gameObject.layer = 0;
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
                box.isTrigger = true;
                this.gameObject.layer = 2;
            }
            else
            {
                box.isTrigger = false;
                this.gameObject.layer = 0;
            }
            picker.SimplePick(currentState);
        }
        else
            animator.SetBool("Transitioning", false);
        if (player.dead == true)
            StartCoroutine(Reset());
    }

    IEnumerator Reset()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        currentState = initial;
        if (currentState == 0)
        {
            box.isTrigger = true;
            this.gameObject.layer = 2;
        }
        else
        {
            box.isTrigger = false;
            this.gameObject.layer = 0;
        }
        picker.SimplePick(currentState);
    }
}
