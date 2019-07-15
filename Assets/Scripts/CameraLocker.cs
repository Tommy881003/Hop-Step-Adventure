using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraLocker : MonoBehaviour
{
    private CameraHolder holder;
    private BoxCollider2D box;
    public Vector2 lockPos = Vector2.zero;
    private PlayerControl player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        holder = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().GetComponent<CameraHolder>();
        box = this.GetComponent<BoxCollider2D>();
        this.GetComponent<SpriteRenderer>().enabled = false;
        this.gameObject.layer = 2;
    }

    private void Update()
    {
        if (player.dead || player.isTransitioning)
            Enable();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            holder.lockPos = lockPos;
            holder.isLocked = true;
            holder.smoothLock = false;
            StopCoroutine(smooth());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            holder.isLocked = false;
            holder.smoothLock = true;
            StartCoroutine(smooth());
        }
    }

    IEnumerator smooth()
    {
        yield return new WaitForSeconds(1);
        holder.smoothLock = false;
        if(holder.isLocked == false)
            Disable();
    }

    public void Disable()
    {
        box.enabled = false;
        holder.isLocked = false;
        holder.smoothLock = false;
    }

    public void Enable()
    {
        box.enabled = true;
    }
}
