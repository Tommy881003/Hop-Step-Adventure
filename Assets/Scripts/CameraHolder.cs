using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraHolder : MonoBehaviour {

    private PlayerControl player;
    public bool isTransitioning;
    private bool isShakeing;
    private float duration = 0, seed1 ,seed2;
    public Vector2 corner, corner2;
    public PlayerPosition pos;

	// Use this for initialization
	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        isTransitioning = false;
        isShakeing = false;
        if (pos != null && pos.testMode == true)
        {
            corner = pos.corner;
            corner2 = pos.corner2;
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(isTransitioning == true)
        {
            DOTween.Kill(this.GetComponent<Camera>());
            isTransitioning = false;
        }
        if(player.dead)
        {
            StartCoroutine(CamShake());
        }
        else if (Input.GetKeyDown(KeyCode.J) && player.canDash == true)
        {
            StartCoroutine(CamShake());
        }
    }

    private void LateUpdate()
    {
        Vector3 camPos = this.transform.position;
        Vector2 playerPos = player.transform.position;
        Vector2 distance = playerPos - new Vector2(camPos.x, camPos.y);
        float followStrengh = Mathf.Lerp(5f, 40f, distance.magnitude / 100);
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
        if (isShakeing)
        {
            int frequency = 8;
            Vector2 pos = this.transform.position;
            pos.x += (Mathf.PerlinNoise(seed1, duration * frequency) - 0.5f) * (0.5f - (1.2f * duration));
            pos.y += (Mathf.PerlinNoise(seed2, duration * frequency) - 0.5f) * (0.5f - (1.2f * duration));
            this.transform.position = new Vector3(pos.x, pos.y, this.transform.position.z);
        }
    }

    IEnumerator CamShake()
    {
        seed1 = Random.Range(-5, 5);
        seed2 = Random.Range(-5, 5);
        isShakeing = true;
        while(duration < 0.3f)
        {
            duration += Time.unscaledDeltaTime;
            yield return null;
        }
        isShakeing = false;
        duration = 0;
    }
}
