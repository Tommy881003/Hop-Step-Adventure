using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Transition : MonoBehaviour
{
    private GameObject player;
    private Camera cam;
    private PlayerControl control;
    private CameraHolder holder;
    private Map map;
    public Vector2 LevelSize, SpawningPoint;
    public int spawnIndex;
    private Vector2 LeftDown, RightUp;
    [HideInInspector]
    public int LevelIndex;
    [HideInInspector]
    public List<int> link = new List<int>();
    public List<Vector2> Spawn = new List<Vector2>();


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        control = player.GetComponent<PlayerControl>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        holder = cam.GetComponent<CameraHolder>();
        map = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
        LeftDown = new Vector2(this.transform.position.x - (0.5f * (LevelSize.x - 40f)), this.transform.position.y - (0.5f * (LevelSize.y - 22)));
        RightUp = new Vector2(this.transform.position.x + (0.5f * (LevelSize.x - 40f)), this.transform.position.y + (0.5f * (LevelSize.y - 22)));
        if (LevelIndex != control.currentLevel && link.Exists(x => x == control.currentLevel) == false)
            this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerControl control = player.GetComponent<PlayerControl>();
        if (collision.gameObject.tag == "Player" && control.currentLevel != LevelIndex)
            StartCoroutine(MakeTransition());
    }

    IEnumerator MakeTransition()
    {
        Vector2 playerPos = player.transform.position;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        PlayerControl control = player.GetComponent<PlayerControl>();
        bool isUp = false;

        control.canDash = true;
        control.SetAnimAndFlip();
        holder.isTransitioning = true;
        holder.enabled = false;
        Vector3 Pos = CamPos(playerPos);
        Vector3 oldPos = cam.transform.position;
        StartCoroutine(holder.MoveParallax(Pos, oldPos, 0.75f));
        cam.transform.DOMove(Pos, 0.75f).SetUpdate(true);

        map.levels[control.currentLevel].GetComponent<Transition>().close(LevelIndex);

        if (holder.corner2.y < LeftDown.y - 21f)
        {
            player.transform.DOMove(new Vector3(playerPos.x, playerPos.y + 1.5f, 0), 0.5f).SetUpdate(true);
            isUp = true;
        }
        else if (holder.corner.y > RightUp.y + 21f)
            player.transform.DOMove(new Vector3(playerPos.x, playerPos.y - 1.5f, 0), 0.5f).SetUpdate(true);
        else if (holder.corner2.x < LeftDown.x - 39.5f)
            player.transform.DOMove(new Vector3(playerPos.x + 1.5f, playerPos.y, 0), 0.5f).SetUpdate(true);
        else if(holder.corner.x > RightUp.x + 39.5f)
            player.transform.DOMove(new Vector3(playerPos.x - 1.5f, playerPos.y, 0), 0.5f).SetUpdate(true);

        Vector2 toSpawn = new Vector2();
        float distance = 1000;
        foreach(Vector2 spawnPoint in Spawn)
        {
            Vector2 dis = new Vector2(spawnPoint.x + this.transform.position.x, spawnPoint.y + this.transform.position.y);
            float temp = (dis - playerPos).magnitude;
            if (temp < distance)
            {
                distance = temp;
                toSpawn = spawnPoint;
            }
        }
        holder.corner = LeftDown;
        holder.corner2 = RightUp;
        control.isTransitioning = true;
        control.currentLevel = LevelIndex;
        control.spawnPos = toSpawn + new Vector2(this.transform.position.x,this.transform.position.y);
        control.enabled = false;
        if (holder.pos != null && holder.pos.testMode == true)
        {
            holder.pos.corner = LeftDown;
            holder.pos.corner2 = RightUp;
            control.pos.currentLevel = LevelIndex;
            control.pos.spawnPos = toSpawn + new Vector2(this.transform.position.x, this.transform.position.y);
        }
        open();
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(1);

        Time.timeScale = 1;
        control.enabled = true;
        holder.enabled = true;

        if (isUp)
        {
            if (control.isDashing)
                control.isDashing = false;
            rb.velocity = new Vector2(0.5f * rb.velocity.x, 17.5f);
            StartCoroutine(control.EndTransition());
        }
        else
            control.isTransitioning = false;
    }

    public Vector3 CamPos(Vector2 playerPos)
    {
        LeftDown = new Vector2(this.transform.position.x - (0.5f * (LevelSize.x - 40f)), this.transform.position.y - (0.5f * (LevelSize.y - 22)));
        RightUp = new Vector2(this.transform.position.x + (0.5f * (LevelSize.x - 40f)), this.transform.position.y + (0.5f * (LevelSize.y - 22)));
        Vector3 Pos = new Vector3(0, 0, -10);
        if (playerPos.x < LeftDown.x - 19.5f)
            Pos.x = LeftDown.x;
        else if (playerPos.x > RightUp.x + 19.5f)
            Pos.x = RightUp.x;
        else
            Pos.x = Mathf.Clamp(playerPos.x, LeftDown.x, RightUp.x);
        if (playerPos.y < LeftDown.y - 11f)
            Pos.y = LeftDown.y;
        else if (playerPos.y > RightUp.y + 11f)
            Pos.y = RightUp.y;
        else
            Pos.y = Mathf.Clamp(playerPos.y, LeftDown.y, RightUp.y);
        return Pos;
    }

    void open()
    {
        foreach(int i in link)
        {
            map.levels[i].SetActive(true);
        }
    }

    public void close(int exception)
    {
        foreach (int i in link)
        {
            if(i != exception)
                map.levels[i].SetActive(false);
        }
    }

    public void apply(int option)
    {
        BoxCollider2D collider = this.GetComponent<BoxCollider2D>();
        collider.size = LevelSize;
        GameObject level = this.transform.Find("LevelCarrier").gameObject;
        float halfX = (LevelSize.x - 40f) * 0.5f, halfY = (LevelSize.y - 22f) * 0.5f;
        switch(option)
        {
            case 1:
                level.transform.localPosition = new Vector3(-halfX, -halfY, 0);
                break;
            case 2:
                level.transform.localPosition = new Vector3(-halfX, halfY, 0);
                break;
            case 3:
                level.transform.localPosition = new Vector3(halfX, -halfY, 0);
                break;
            case 4:
                level.transform.localPosition = new Vector3(halfX, halfY, 0);
                break;
            default:
                level.transform.localPosition = Vector3.zero;
                break;
        }
    }

    public void ResetSize(Vector2 size)
    {
        LevelSize = size;
    }

    public void FixEdge()
    {
        float halfX = (LevelSize.x - 1) / 2, halfY = (LevelSize.y - 1) / 2;
        for(float i = -halfX; i <= halfX; i ++)
        {
            RaycastHit2D upHit = Physics2D.Raycast(new Vector2(this.transform.position.x + i, this.transform.position.y + halfY), Vector2.zero);
            RaycastHit2D downHit = Physics2D.Raycast(new Vector2(this.transform.position.x + i, this.transform.position.y - halfY), Vector2.zero);
            if (upHit.collider != null && upHit.collider.gameObject.GetComponent<Multisprites>() != null)
                upHit.collider.gameObject.GetComponent<SpritePicker>().PickSprite(upHit.collider.gameObject.name);
            if (downHit.collider != null && downHit.collider.gameObject.GetComponent<Multisprites>() != null)
                downHit.collider.gameObject.GetComponent<SpritePicker>().PickSprite(downHit.collider.gameObject.name);
        }
        for (float i = -halfY; i <= halfY; i++)
        {
            RaycastHit2D leftHit = Physics2D.Raycast(new Vector2(this.transform.position.x - halfX, this.transform.position.y + i), Vector2.zero);
            RaycastHit2D rightHit = Physics2D.Raycast(new Vector2(this.transform.position.x + halfX, this.transform.position.y + i), Vector2.zero);
            if (leftHit.collider != null && leftHit.collider.gameObject.GetComponent<Multisprites>() != null)
                leftHit.collider.gameObject.GetComponent<SpritePicker>().PickSprite(leftHit.collider.gameObject.name);
            if (rightHit.collider != null && rightHit.collider.gameObject.GetComponent<Multisprites>() != null)
                rightHit.collider.gameObject.GetComponent<SpritePicker>().PickSprite(rightHit.collider.gameObject.name);
        }
    }

    public void AddSpawn(Vector2 point)
    {
        if (Spawn.Exists(x => x == point) == false)
            Spawn.Add(point);
    }

    public void ModifySpawn(int index, Vector2 point)
    {
        if(Spawn.Exists(x => x == point) == false)
            Spawn[index] = point;
    }

    public void RemoveSpawn(int index)
    {
        Spawn.RemoveAt(index);
    }
}
