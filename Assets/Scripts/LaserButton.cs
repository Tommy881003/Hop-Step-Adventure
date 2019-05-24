using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserButton : MonoBehaviour {

    public bool press;
    private bool initial;
    private Animator animator;
    private GameObject[] crystalAndButton;
    private PlayerControl player;

	// Use this for initialization
	void Start ()
    {
        animator = this.GetComponent<Animator>();
        if (this.name == "LaserButton")
            press = false;
        else
            press = true;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        crystalAndButton = GameObject.FindGameObjectsWithTag("Laser");
        initial = press;
	}
	
	// Update is called once per frame
	void Update ()
    {
        animator.SetBool("Press", press);
        if (player.dead == true || player.isTransitioning == true)
            StartCoroutine(Reset());
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerControl player = collision.gameObject.GetComponent<PlayerControl>();
            if (player.isDashing == true || player.isTransitioning == true)
            {
                foreach(GameObject temp in crystalAndButton)
                { 
                    LaserButton laserButton = temp.GetComponent<LaserButton>();
                    if(laserButton != null && laserButton.enabled == true)
                        laserButton.press = !laserButton.press;
                    else
                    {
                        Line line = temp.GetComponent<Line>();
                        if(line != null && line.enabled == true)
                            line.isActivate = !line.isActivate;
                    }
                }
            }
        }
    }

    IEnumerator Reset()
    {
        if (player.isTransitioning == true)
            yield return new WaitForSecondsRealtime(0.5f);
        press = initial;
    }
}
