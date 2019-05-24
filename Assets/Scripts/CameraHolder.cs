using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraHolder : MonoBehaviour {

    private PlayerControl player;
    private Camera cam;
    private bool once;
    public bool shakeing;
    public bool isTransitioning;
    public Vector2 corner, corner2;
    public PlayerPosition pos;

	// Use this for initialization
	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        cam = this.GetComponent<Camera>();
        once = true;
        shakeing = false;
        isTransitioning = false;
        if (pos != null && pos.testMode == true)
        {
            corner = pos.corner;
            corner2 = pos.corner2;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(isTransitioning == true)
        {
            DOTween.Kill(this.GetComponent<Camera>());
            isTransitioning = false;
        }
        if(player.dead && once)
        {
            cam.DOShakePosition(0.3f, 0.1f);
            once = false;
            StartCoroutine(ResetOnce());
        }
        else if (Input.GetKeyDown(KeyCode.J) && player.canDash == true)
        {
            cam.DOShakePosition(0.3f, 0.1f);
            shakeing = true;
            StartCoroutine(ResetShakeing());
        }
        else if(shakeing == false)
        {
            Vector3 camPos = this.transform.position;
            Vector2 playerPos = player.transform.position;
            Vector2 distance = playerPos - new Vector2(camPos.x,camPos.y);
            float followStrengh = Mathf.Lerp(5f, 20f, distance.magnitude/100);
            camPos.x = Mathf.Lerp(camPos.x, playerPos.x, followStrengh * Time.deltaTime);
            if (camPos.x > Mathf.Max(corner.x, corner2.x))
                camPos.x = Mathf.Max(corner.x, corner2.x);
            else if (camPos.x < Mathf.Min(corner.x, corner2.x))
                camPos.x = Mathf.Min(corner.x, corner2.x);
            camPos.y = Mathf.Lerp(camPos.y, playerPos.y, followStrengh * Time.deltaTime);
            if (camPos.y > Mathf.Max(corner.y, corner2.y))
                camPos.y = Mathf.Max(corner.y, corner2.y);
            else if (camPos.y < Mathf.Min(corner.y, corner2.y))
                camPos.y = Mathf.Min(corner.y, corner2.y);
            camPos.z = -10;
            this.transform.position = camPos;
        }
    }

    IEnumerator ResetOnce()
    {
        yield return new WaitForSeconds(0.25f);
        once = true;
    }

    IEnumerator ResetShakeing()
    {
        yield return new WaitForSeconds(0.3f);
        shakeing = false;
    }
}
