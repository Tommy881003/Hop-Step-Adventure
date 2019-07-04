using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadScene : MonoBehaviour
{
    private PlayerControl control;
    private GameObject player;
    private Animator animator;
    private bool once;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        control = player.GetComponent<PlayerControl>();
        animator = this.GetComponent<Animator>();
        once = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (control.dead == true && once == true)
        {
            StartCoroutine(Scene());
            once = false;
        }
    }

    IEnumerator Scene()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        animator.SetTrigger("Dead");
        yield return new WaitForSecondsRealtime(0.3f);
        //StartCoroutine(enableControl());
        control.dead = false;
        control.enabled = false;
        player.transform.position = control.spawnPos;
        control.canDash = true;
        once = true;
        yield return new WaitForSecondsRealtime(0.3f);
        control.enabled = true;
    }

    IEnumerator enableControl()
    {
        yield return new WaitForSecondsRealtime(0.8f);
        control.enabled = true;
    }
}
