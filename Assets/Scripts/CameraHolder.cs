using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraHolder : MonoBehaviour {

    private PlayerControl player;
    private Camera cam;
    private bool once;
    public bool shakeing;
    public Vector2 corner, corner2;

	// Use this for initialization
	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        cam = this.GetComponent<Camera>();
        once = true;
        shakeing = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(player.dead && once)
        {
            cam.DOShakePosition(0.5f, 0.25f);
            once = false;
            StartCoroutine(ResetOnce());
        }
        else if (Input.GetKeyDown(KeyCode.J) && player.canDash == true)
        {
            cam.DOShakePosition(0.3f, 0.1f);
            shakeing = true;
            StartCoroutine(ResetShakeing());
            Debug.Log("DASH");
        }
        else if(shakeing == false)
        {
            Vector3 camPos = this.transform.position;
            Vector2 playerPos = player.transform.position;
            Vector2 distance = playerPos - new Vector2(camPos.x,camPos.y);
            float followStrengh = Mathf.Lerp(0.05f, 0.15f, distance.magnitude/1000);
            camPos.x = Mathf.Lerp(camPos.x, playerPos.x, followStrengh);
            if (camPos.x > Mathf.Max(corner.x, corner2.x))
                camPos.x = Mathf.Max(corner.x, corner2.x);
            else if (camPos.x < Mathf.Min(corner.x, corner2.x))
                camPos.x = Mathf.Min(corner.x, corner2.x);
            camPos.y = Mathf.Lerp(camPos.y, playerPos.y, followStrengh);
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
