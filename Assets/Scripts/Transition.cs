using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Transition : MonoBehaviour
{
    private GameObject player;
    private Camera cam;
    private CameraHolder holder;
    public bool isLR;
    public Vector2 sceneRightOrUp;  //1 for transition coordinate, 2 for another corner coordinate
    public Vector2 sceneRightOrUp2;
    public Vector2 spawnRightOrUp;
    public int indexRightOrUp;
    public Vector2 sceneLeftOrDown;
    public Vector2 sceneLeftOrDown2;
    public Vector2 spawnLeftOrDown;
    public int indexLeftOrDown;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        holder = cam.GetComponent<CameraHolder>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            StartCoroutine(MakeTransition());
        }
    }

    IEnumerator MakeTransition()
    {
        Vector2 playerPos = player.transform.position;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        PlayerControl control = player.GetComponent<PlayerControl>();
        holder.enabled = false;
        if (control.currentLevel == indexRightOrUp)
        {
            if (isLR)
                player.transform.DOMove(new Vector3(playerPos.x - 1.5f, playerPos.y, 0), 0.5f).SetUpdate(true);
            cam.transform.DOMove(new Vector3(sceneLeftOrDown.x, sceneLeftOrDown.y, -10), 0.75f).SetUpdate(true);
            control.currentLevel = indexLeftOrDown;
            control.spawnPos = spawnLeftOrDown;
        }
        else
        {
            if (isLR)
                player.transform.DOMove(new Vector3(playerPos.x + 1.5f, playerPos.y, 0), 0.5f).SetUpdate(true);
            cam.transform.DOMove(new Vector3(sceneRightOrUp.x, sceneRightOrUp.y, -10), 0.75f).SetUpdate(true);
            control.currentLevel = indexRightOrUp;
            control.spawnPos = spawnRightOrUp;
        }
        control.isTransitioning = true;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(1);
        Time.timeScale = 1;
        if(control.currentLevel == indexRightOrUp)
        {
            holder.corner = sceneRightOrUp;
            holder.corner2 = sceneRightOrUp2;
        }
        else
        {
            holder.corner = sceneLeftOrDown;
            holder.corner2 = sceneLeftOrDown2;
        }
        if(isLR == false)
        {
            if (control.currentLevel == indexRightOrUp && Physics2D.gravity.y < 0)
            {
                if(control.dashTime >= 0.01f)
                    control.dashTime = 0.05f;
                else
                    rb.velocity = new Vector2(rb.velocity.x, 25);
            }
            else if(control.currentLevel == indexLeftOrDown && Physics2D.gravity.y > 0)
                rb.velocity = new Vector2(rb.velocity.x, -25);
        }
        control.isTransitioning = false;
        holder.enabled = true;
    }
}
