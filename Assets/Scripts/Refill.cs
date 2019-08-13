using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Refill : MonoBehaviour
{
    private PlayerControl player;
    private ObjAudioManager audioManager;
    private Animator anim;
    private SpriteRenderer sr;
    private CircleCollider2D cc;
    [SerializeField]
    private ParticleSystem ps, eat;
    private Light point;
    public bool once = true;
    private Coroutine coroutine = null;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        audioManager = this.GetComponent<ObjAudioManager>();
        anim = this.GetComponent<Animator>();
        sr = this.GetComponent<SpriteRenderer>();
        cc = this.GetComponent<CircleCollider2D>();
        point = this.GetComponentInChildren<Light>();
        cc.enabled = true;
    }

    private void Update()
    {
        if ((player.dead || player.isTransitioning) && once)
        {
            if(coroutine != null)
                StopCoroutine(coroutine);
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
                eat.Play();
                ps.Clear();
                ps.Stop();
                anim.SetTrigger("Halt");
                audioManager.PlayByName("Touch");
                coroutine = StartCoroutine(Appear(false));
            }
        }
    }

    IEnumerator Appear(bool fast)
    {
        if(fast)
        {
            yield return new WaitForSeconds((player.isTransitioning)? 0 : 1);
            once = true;
            sr.enabled = true;
            cc.enabled = true;
            point.enabled = true;
            ps.Play();
            anim.SetBool("Init",once);
            yield return new WaitForEndOfFrame();
            anim.SetBool("Init", false);
        }
        else
        {
            yield return new WaitForSeconds(3);
            sr.enabled = true;
            cc.enabled = true;
            point.enabled = true;
            ps.Play();
            anim.SetTrigger("Appear");
            anim.ResetTrigger("Halt");
            audioManager.PlayByName("Reappear");
            yield return new WaitForEndOfFrame();
            anim.ResetTrigger("Appear");
        }   
    }
}
