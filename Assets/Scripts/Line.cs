using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    private LineRenderer lr;
    private Rigidbody2D rb;
    public GameObject blob;
    public bool isActivate;
    public bool isMove;
    public int MoveSpeed;
    public Vector2 pingpong;
    private bool initial;
    private PlayerControl player;
    private float t = 0;
    private Vector2 blobPosA,blobPosB,rayDir;
    private GameObject BlobA, BlobB;
    private int rotation;
    private int ping = 1;
    private Vector2 velocity;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -1);
        initial = isActivate;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        lr = this.GetComponent<LineRenderer>();
        rb = this.GetComponent<Rigidbody2D>();
        rotation = (int)(this.transform.rotation.eulerAngles.z);
        switch (rotation)
        {
            case 0:
                rayDir = Vector2.right;
                break;
            case 90:
                rayDir = Vector2.up;
                break;
            case 180:
                rayDir = Vector2.left;
                break;
            default:
                rayDir = Vector2.down;
                break;
        }
        blobPosA = this.transform.position;
        blobPosB = this.transform.position;
        BlobA = Instantiate(blob,blobPosA,Quaternion.identity);
        BlobB = Instantiate(blob,blobPosB,Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        lr.enabled = isActivate;
        BlobA.SetActive(isActivate);
        BlobB.SetActive(isActivate);
        if (player.dead == true || player.isTransitioning == true)
            StartCoroutine(Reset());
        if (isMove == true && isActivate == true)
        {
            if (rotation == 0 || rotation == 180)
            {
                if(ping == 1)
                    velocity.y = -MoveSpeed;
                else
                    velocity.y = MoveSpeed;
                if (this.transform.position.y >= Mathf.Max(pingpong.x, pingpong.y))
                    ping = 1;
                else if (this.transform.position.y <= Mathf.Min(pingpong.x, pingpong.y))
                    ping = -1;
            }
            else
            {
                if (ping == 1)
                    velocity.x = -MoveSpeed;
                else
                    velocity.x = MoveSpeed;
                if (this.transform.position.x >= Mathf.Max(pingpong.x, pingpong.y))
                    ping = 1;
                else if (this.transform.position.x <= Mathf.Min(pingpong.x, pingpong.y))
                    ping = -1;
            }
            rb.velocity = velocity;
        }
        else
        {
            velocity = rb.velocity;
            rb.velocity = Vector2.zero;
        }
        if (isActivate == true)
        {
            bool dohit = false;
            lr.SetPosition(0, transform.position);
            Vector2 LimitPos = (Vector2)transform.position + rayDir * 100;
            RaycastHit2D hit = Physics2D.Raycast(transform.position + (Vector3)rayDir * 0.01f, rayDir, 100);
            if (hit.collider != null && hit.collider.isTrigger == false)
            {
                lr.SetPosition(1, hit.point);
                BlobA.transform.position = new Vector3(hit.point.x, hit.point.y, -9);
                dohit = true;
                if(hit.collider.gameObject.tag == "Player")
                {
                    player.dead = true;
                    StartCoroutine(Reset());
                }
            }
            if (dohit == false)
            {
                lr.SetPosition(1, LimitPos);
                BlobA.transform.position = LimitPos;
            }
            BlobB.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -9);
        }
    }

    private void FixedUpdate()
    {
        if(isActivate)
        {
            t += Time.fixedDeltaTime;
            float scale = (float)(Mathf.Sin(40 * t) * 0.075f + 0.6f);
            lr.widthMultiplier = (float)(Mathf.Sin(40 * t) * 0.04f + 0.4f);
            BlobA.transform.localScale = new Vector3(scale, scale);
            BlobB.transform.localScale = new Vector3(scale, scale);
        }
    }

    IEnumerator Reset()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        isActivate = initial;
    }
}
