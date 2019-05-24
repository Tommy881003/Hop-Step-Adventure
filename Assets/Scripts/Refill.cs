using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Refill : MonoBehaviour
{
    private PlayerControl player;
    private Animator anim;
    private Vector2 position;
    private SpriteRenderer sr;
    private CircleCollider2D cc;
    private ParticleSystem ps;
    private Light point;
    private GameObject outline;
    public bool once = true;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        anim = this.GetComponent<Animator>();
        position = this.transform.position;
        sr = this.GetComponent<SpriteRenderer>();
        cc = this.GetComponent<CircleCollider2D>();
        ps = this.GetComponentInChildren<ParticleSystem>();
        point = this.GetComponentInChildren<Light>();
        outline = this.transform.Find("Outline").gameObject;
        cc.enabled = true;
    }

    private void Update()
    {
        if (player.dead && once)
        {
            StartCoroutine(Appear(true));
            once = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.GetComponent<PlayerControl>().canDash == false)
            {
                collision.gameObject.GetComponent<PlayerControl>().canDash = true;
                sr.enabled = false;
                cc.enabled = false;
                point.enabled = false;
                ps.Clear();
                ps.Stop();
                anim.SetTrigger("Halt");
                StartCoroutine(Appear(false));
            }
        }
    }

    IEnumerator Appear(bool fast)
    {
        if(fast)
        {
            yield return new WaitForSeconds(1);
            once = true;
        }
        else
            yield return new WaitForSeconds(3);
        sr.enabled = true;
        cc.enabled = true;
        point.enabled = true;
        ps.Play();
        anim.SetTrigger("Appear");
    }
}
