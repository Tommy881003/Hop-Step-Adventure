using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Transition : MonoBehaviour
{
    private GameObject player;
    private Camera cam;
    public bool isLR;
    public Vector2 sceneRightOrUp;
    public int indexRightOrUp;
    public Vector2 sceneLeftOrDown;
    public int indexLeftOrDown;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
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
        if (control.currentLevel == indexRightOrUp)
        {
            if (isLR)
                player.transform.DOMove(new Vector3(playerPos.x - 1.5f, playerPos.y, 0), 0.5f).SetUpdate(true);
            cam.transform.DOMove(new Vector3(sceneLeftOrDown.x, sceneLeftOrDown.y, -10), 0.75f).SetUpdate(true);
            player.GetComponent<PlayerControl>().currentLevel = indexLeftOrDown;
        }
        else
        {
            if (isLR)
                player.transform.DOMove(new Vector3(playerPos.x + 1.5f, playerPos.y, 0), 0.5f).SetUpdate(true);
            cam.transform.DOMove(new Vector3(sceneRightOrUp.x, sceneRightOrUp.y, -10), 0.75f).SetUpdate(true);
            player.GetComponent<PlayerControl>().currentLevel = indexRightOrUp;
        }
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(1);
        Time.timeScale = 1;
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
    }
}
