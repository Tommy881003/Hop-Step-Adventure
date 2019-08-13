using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserLock : MonoBehaviour
{
    private Vector3 Pos, rayPos;
    private LineRenderer lr;
    public GameObject blob;
    [HideInInspector]
    public bool isActivate = true;
    private float hitDist;
    private int rotation;
    private PlayerControl player;
    private float t = 0;
    private Vector2 blobPosA, blobPosB, rayDir;
    private GameObject BlobA, BlobB;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -1);
        Pos = this.transform.position;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        lr = this.GetComponent<LineRenderer>();
        lr.useWorldSpace = true;
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
        rayPos = Pos + new Vector3(0.3f * rayDir.x, 0.3f * rayDir.y, 0);
        blobPosA = rayPos;
        blobPosB = rayPos;
        BlobA = Instantiate(blob, blobPosA, Quaternion.identity, this.transform);
        BlobB = Instantiate(blob, blobPosB, Quaternion.identity, this.transform);
    }

    private void Update()
    {
        rayPos = this.transform.position + new Vector3(0.3f * rayDir.x, 0.3f * rayDir.y, 0);
        lr.enabled = isActivate;
        BlobA.SetActive(isActivate);
        BlobB.SetActive(isActivate);
        if (isActivate)
        {
            t += Time.deltaTime;
            float scale = (Mathf.Sin(40 * t) * 0.075f + 0.6f);
            lr.widthMultiplier = (Mathf.Sin(40 * t) * 0.04f + 0.4f);
            BlobA.transform.localScale = new Vector3(scale, scale);
            BlobB.transform.localScale = new Vector3(scale, scale);

            bool dohit = false;
            lr.SetPosition(0, rayPos);
            Vector2 LimitPos = (Vector2)transform.position + rayDir * 100;
            RaycastHit2D hit = Physics2D.Raycast(transform.position + (Vector3)rayDir * 0.01f, rayDir, 200, ~((1 << 10) | (1 << 2) | (1 << 11) | (1 << 9)));
            if (hit.collider != null && hit.collider.isTrigger == false)
            {
                lr.SetPosition(1, hit.point);
                BlobA.transform.position = new Vector3(hit.point.x, hit.point.y, -9);
                dohit = true;
                if (hit.collider.gameObject.tag == "Player")
                    player.dead = true;
            }
            if (dohit == false)
            {
                lr.SetPosition(1, LimitPos);
                BlobA.transform.position = LimitPos;
            }
            BlobB.transform.position = rayPos;
        }
    }
}
