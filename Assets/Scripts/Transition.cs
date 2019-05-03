using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Transition : MonoBehaviour
{
    private GameObject player;
    private Camera cam;
    private CameraHolder holder;
    public GameObject level;
    public GameObject Phantom;
    public bool isLR;
    public Vector2 sceneRightOrUp;  //1 for transition coordinate, 2 for another corner coordinate
    public Vector2 sceneRightOrUp2;
    public Vector2 spawnRightOrUp;
    public int indexRightOrUp;
    public Vector2 sceneLeftOrDown;
    public Vector2 sceneLeftOrDown2;
    public Vector2 spawnLeftOrDown;
    public int indexLeftOrDown;
    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        holder = cam.GetComponent<CameraHolder>();
        sr = this.GetComponent<SpriteRenderer>();
        sr.enabled = false;
        if (isLR)
            this.transform.localScale = new Vector3(0.05f, this.transform.localScale.y, this.transform.localScale.z);
        else
            this.transform.localScale = new Vector3(this.transform.localScale.x, 0.05f, this.transform.localScale.z);
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
        holder.isTransitioning = true;
        holder.shakeing = true;
        if (control.currentLevel == indexRightOrUp)
        {
            if (isLR)
                player.transform.DOMove(new Vector3(playerPos.x - 1.5f, playerPos.y, 0), 0.5f).SetUpdate(true);
            cam.transform.DOMove(new Vector3(sceneLeftOrDown.x, sceneLeftOrDown.y, -10), 0.75f).SetUpdate(true);
            if(holder.pos.testMode == true)
            {
                holder.pos.corner = sceneLeftOrDown;
                holder.pos.corner2 = sceneLeftOrDown2;
                control.pos.currentLevel = indexLeftOrDown;
                control.pos.spawnPos = spawnLeftOrDown;
            }
            control.currentLevel = indexLeftOrDown;
            control.spawnPos = spawnLeftOrDown;
        }
        else
        {
            if (isLR)
                player.transform.DOMove(new Vector3(playerPos.x + 1.5f, playerPos.y, 0), 0.5f).SetUpdate(true);
            cam.transform.DOMove(new Vector3(sceneRightOrUp.x, sceneRightOrUp.y, -10), 0.75f).SetUpdate(true);
            if (holder.pos.testMode == true)
            {
                holder.pos.corner = sceneRightOrUp;
                holder.pos.corner2 = sceneRightOrUp2;
                control.pos.currentLevel = indexRightOrUp;
                control.pos.spawnPos = spawnRightOrUp;
            }
            control.currentLevel = indexRightOrUp;
            control.spawnPos = spawnRightOrUp;      
        }
        control.isTransitioning = true;
        if (control.currentLevel == indexRightOrUp)
        {
            holder.corner = sceneRightOrUp;
            holder.corner2 = sceneRightOrUp2;
        }
        else
        {
            holder.corner = sceneLeftOrDown;
            holder.corner2 = sceneLeftOrDown2;
        }
        if (isLR == false)
        {
            if (control.currentLevel == indexRightOrUp)
            {
                if (control.isDashing == true)
                    control.isDashing = false;
                rb.velocity = new Vector2(rb.velocity.x, 22.5f);
            }
        }
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(1);
        Time.timeScale = 1;
        if (isLR)
            control.isTransitioning = false;
        else
            StartCoroutine(control.EndTransition());
        holder.shakeing = false;
    }

    public void Show()
    {
        Vector2 levelRightOrUpPos = (sceneRightOrUp + sceneRightOrUp2) / 2;
        Vector2 levelLeftOrDownPos = (sceneLeftOrDown + sceneLeftOrDown2) / 2;
        GameObject RightOrUp = Instantiate(level, levelRightOrUpPos, Quaternion.identity);
        GameObject LeftOrDown = Instantiate(level, levelLeftOrDownPos, Quaternion.identity);
        GameObject SpawnRU = Instantiate(Phantom, spawnRightOrUp, Quaternion.identity);
        GameObject SpawnLD = Instantiate(Phantom, spawnLeftOrDown, Quaternion.identity);
        RightOrUp.name = "RightOrUp";
        LeftOrDown.name = "LeftOrDown";
        SpawnRU.name = "SpawnRU";
        SpawnLD.name = "SpawnLD";
        RightOrUp.transform.SetParent(this.gameObject.transform, true);
        LeftOrDown.transform.SetParent(this.gameObject.transform, true);
        SpawnRU.transform.SetParent(this.gameObject.transform, true);
        SpawnLD.transform.SetParent(this.gameObject.transform, true);
        RightOrUp.transform.localScale = new Vector3(Mathf.Abs(sceneRightOrUp.x - sceneRightOrUp2.x) + 40, Mathf.Abs(sceneRightOrUp.y - sceneRightOrUp2.y) + 22.5f, 1);
        LeftOrDown.transform.localScale = new Vector3(Mathf.Abs(sceneLeftOrDown.x - sceneLeftOrDown2.x) + 40, Mathf.Abs(sceneLeftOrDown.y - sceneLeftOrDown2.y) + 22.5f, 1);
    }

    public void Kill()
    {
        if (this.gameObject.GetComponentsInChildren<Transform>() == null)
            return;
        Transform[] transforms = this.gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in transforms)
        {
            if(child.gameObject != this.gameObject)
                GameObject.DestroyImmediate(child.gameObject);
        }
    }

    public void modify()
    {
        Transform[] transforms = this.gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in transforms)
        {
            if (child.gameObject.name == "RightOrUp")
            {
                child.transform.position = (sceneRightOrUp + sceneRightOrUp2) / 2;
                child.transform.parent = null;
                child.transform.localScale = new Vector3(Mathf.Abs(sceneRightOrUp.x - sceneRightOrUp2.x) + 40, Mathf.Abs(sceneRightOrUp.y - sceneRightOrUp2.y) + 22.5f, 1);
                child.transform.parent = this.transform;
            }
            if(child.gameObject.name == "LeftOrDown")
            {
                child.transform.position = (sceneLeftOrDown + sceneLeftOrDown2) / 2;
                child.transform.parent = null;
                child.transform.localScale = new Vector3(Mathf.Abs(sceneLeftOrDown.x - sceneLeftOrDown2.x) + 40, Mathf.Abs(sceneLeftOrDown.y - sceneLeftOrDown2.y) + 22.5f, 1);
                child.transform.parent = this.transform;
            }
            if (child.gameObject.name == "SpawnRU")
                child.transform.position = spawnRightOrUp;
            if (child.gameObject.name == "SpawnLD")
                child.transform.position = spawnLeftOrDown;
        }
    }

    public void Set(int input)//0 for RU 1 for LD
    {
        PlayerControl control = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        if(input == 0)
        {
            control.pos.corner = sceneRightOrUp;
            control.pos.corner2 = sceneRightOrUp2;
            control.pos.currentLevel = indexRightOrUp;
            control.pos.spawnPos = spawnRightOrUp;
        }
        else if(input == 1)
        {
            control.pos.corner = sceneLeftOrDown;
            control.pos.corner2 = sceneLeftOrDown2;
            control.pos.currentLevel = indexLeftOrDown;
            control.pos.spawnPos = spawnLeftOrDown;
        }
    }
}
