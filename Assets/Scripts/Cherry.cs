using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Cherry : MonoBehaviour
{
    private GameObject player;
    private PlayerControl control;
    private CircleCollider2D cc;
    private ParticleSystem ps;
    private ParticleSystem gain;
    private SpriteRenderer sr;
    private Animator anim;
    public bool touched = false,once = true,collected = false;
    private Vector2 distance;
    public Vector3 initialPos;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        control = player.GetComponent<PlayerControl>();
        cc = this.GetComponent<CircleCollider2D>();
        ps = this.transform.Find("FollowParticle").GetComponent<ParticleSystem>();
        gain = this.transform.Find("GainParticle").GetComponent<ParticleSystem>();
        sr = this.transform.Find("Main").GetComponent<SpriteRenderer>();
        anim = this.GetComponentInChildren<Animator>();
        cc.radius = 1;
        initialPos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (touched && control.dead == false)
            followPlayer();
        else if (control.dead)
            flyBack();
        if (touched && control.onGround && control.canCollect && distance.magnitude <= 2.25f)
            Collect();
        if (touched == false && control.isTransitioning)
            Reset();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            touched = true;
            cc.enabled = false;
            sr.sortingLayerName = "Cherry";
        }
    }

    void followPlayer()
    {
        Vector2 oldPos = this.transform.position;
        Vector2 playerPos = player.transform.position;
        distance = playerPos - oldPos;
        float followStrenth = Mathf.Lerp(0, 0.15f, Mathf.Clamp01((distance.magnitude - 2) / 10));
        Vector2 newPos;
        newPos.x = Mathf.Lerp(oldPos.x, playerPos.x, followStrenth);
        newPos.y = Mathf.Lerp(oldPos.y, playerPos.y, followStrenth);
        ps.transform.LookAt(player.transform, Vector3.forward);
        this.transform.position = new Vector3(newPos.x, newPos.y, this.transform.position.z);
    }

    void flyBack()
    {
        if(once)
        {
            this.transform.DOMoveX(initialPos.x, 1).SetEase(Ease.OutCubic);
            this.transform.DOMoveY(initialPos.y, 1).SetEase(Ease.OutSine);
            once = false;
            touched = false;
            sr.enabled = true;
            sr.sortingLayerName = "Default";
            StartCoroutine(ResetOnce());
        }
        ps.transform.LookAt(initialPos, Vector3.forward);
    }

    void Collect()
    {
        gain.Play();
        sr.enabled = false;
        collected = true;
        touched = false;
        anim.SetTrigger("Collect");
        StartCoroutine(GoBack());
    }

    private void Reset()
    {
        sr.enabled = true;
        cc.enabled = true;
        sr.sortingLayerName = "Default";
    }

    IEnumerator GoBack()
    {
        yield return new WaitForSeconds(1f);
        this.transform.position = initialPos;
        ps.Play(false);
    }

    IEnumerator ResetOnce()
    {
        yield return new WaitUntil(() => control.dead == false);
        once = true;
        cc.enabled = true;
    }
}
