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
        control = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
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
        animator.SetTrigger("Dead");
        yield return new WaitForSecondsRealtime(0.5f);
        player.transform.position = new Vector2(2, 3);
        control.dead = false;
        control.enabled = false;
        once = true;
        yield return new WaitForSecondsRealtime(0.6f);
        control.enabled = true;
    }
}
