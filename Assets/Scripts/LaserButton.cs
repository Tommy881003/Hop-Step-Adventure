using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserButton : MonoBehaviour {

    public bool press;
    private bool initial;
    private Animator animator;
    private GameObject[] crystalAndButton;
    private PlayerControl player;
    private BoxCollider2D box;
    private bool jumpX,jumpY;
    private int x = 0, y = 0;

    // Use this for initialization
    void Start ()
    {
        box = this.GetComponent<BoxCollider2D>();
        animator = this.GetComponent<Animator>();
        if (this.name == "LaserButton")
            press = false;
        else
            press = true;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        crystalAndButton = GameObject.FindGameObjectsWithTag("Laser");
        initial = press;
        int rotation = (int)(this.transform.rotation.eulerAngles.z);
        switch (rotation)
        {
            case 0:
                jumpY = true;
                jumpX = false;
                y = -1;
                break;
            case 90:
                jumpY = false;
                jumpX = true;
                x = 1;
                break;
            case 180:
                jumpY = true;
                jumpX = false;
                y = 1;
                break;
            default:
                jumpY = false;
                jumpX = true;
                x = -1;
                break;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        animator.SetBool("Press", press);
        box.enabled = true;
        if (player.dead == true || player.isTransitioning == true)
        {
            StartCoroutine(Reset());
        }
	}

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            float checkX = (this.transform.position.x - player.transform.position.x) * x;
            float checkY = (this.transform.position.y - player.transform.position.y) * y;
            if (player.isDashing == true && !press && ((jumpX && player.dashDirection.x == x && checkX > 0) || (jumpY && player.dashDirection.y == y && checkY > 0)))
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
                        {
                            line.isActivate = !line.isActivate;
                            line.PlaySound(line.isActivate);
                        }  
                    }
                }
            }
        }
    }

    IEnumerator Reset()
    {
        if(player.dead)
            yield return new WaitForSecondsRealtime(0.8f);
        press = initial;
        animator.SetBool("Press", press);
    }
}
