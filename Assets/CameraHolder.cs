using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraHolder : MonoBehaviour {

    private PlayerControl player;
    private Camera cam;
    private bool once;

	// Use this for initialization
	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        cam = this.GetComponent<Camera>();
        once = true;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.J) && player.canDash == true && player.dashCooldown >= 0)
        {
            cam.DOShakePosition(0.3f,0.1f).SetUpdate(true);
        }
        if(player.dead && once)
        {
            cam.DOShakePosition(0.5f, 0.25f);
            once = false;
            StartCoroutine(ResetOnce());
        }
    }

    IEnumerator ResetOnce()
    {
        yield return new WaitForSeconds(0.25f);
        once = true;
    }
}
