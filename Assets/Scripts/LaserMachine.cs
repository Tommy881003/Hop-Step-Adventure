using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserMachine : MonoBehaviour {

    public bool isAcivate = false;
    private bool initial;
    private int laserLength;
    private PlayerControl player;
    public GameObject mid;
    public GameObject bottom;
    private Animator animator;
	
	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        animator = this.GetComponent<Animator>();
        int rotation = (int)(this.transform.rotation.eulerAngles.z);
        Vector2 rayPos,rayDir;
        switch (rotation)
        {
            case 0:
                rayPos = new Vector2(this.transform.position.x, this.transform.position.y - 0.1f);
                rayDir = Vector2.down;
                break;
            case 90:
                rayPos = new Vector2(this.transform.position.x + 0.1f, this.transform.position.y);
                rayDir = Vector2.right;
                break;
            case 180:
                rayPos = new Vector2(this.transform.position.x, this.transform.position.y + 0.1f);
                rayDir = Vector2.up;
                break;
            default:
                rayPos = new Vector2(this.transform.position.x - 0.1f, this.transform.position.y);
                rayDir = Vector2.left;
                break;
        }
        
        Vector2 spawnPos = this.transform.position;
        RaycastHit2D hitInfo = Physics2D.Raycast(rayPos,rayDir);
        laserLength = Mathf.FloorToInt(hitInfo.distance);
        for(int i = 0; i < laserLength; i ++)
        {
            if(i == 0)
            {
                spawnPos = spawnPos + (1.5f * rayDir);
                Instantiate(mid, spawnPos, this.transform.rotation, this.transform);
            }
            else if(i == laserLength - 1)
            {
                spawnPos = spawnPos + rayDir;
                Instantiate(bottom, spawnPos, this.transform.rotation, this.transform);
            }
            else
            {
                spawnPos = spawnPos + rayDir;
                Instantiate(mid, spawnPos, this.transform.rotation, this.transform);
            }
        }
        initial = isAcivate;
    }
	
	// Update is called once per frame
	void Update ()
    {
        animator.SetBool("isActivate", isAcivate);
        if (player.dead == true)
            StartCoroutine(Reset());
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            collision.gameObject.GetComponent<PlayerControl>().dead = true;
    }

    IEnumerator Reset()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        isAcivate = initial;
    }
}
