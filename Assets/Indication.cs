using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indication : MonoBehaviour
{
    private CircleCollider2D cc;
    private GameObject text, sprite;
    private Animator textAnim, spriteAnim;
    public bool isConsist;

    // Start is called before the first frame update
    void Start()
    {
        cc = this.GetComponent<CircleCollider2D>();
        text = this.transform.Find("Text").gameObject;
        sprite = this.transform.Find("Indication").gameObject;
        textAnim = text.GetComponent<Animator>();
        spriteAnim = sprite.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if(isConsist == false && collision.gameObject.transform.position.y > this.transform.position.y)
                Destroy(this.gameObject);
            else
            {
                textAnim.SetTrigger("In");
                spriteAnim.SetTrigger("In");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            textAnim.SetTrigger("Out");
            spriteAnim.SetTrigger("Out");
            if (isConsist == false && collision.gameObject.transform.position.y > this.transform.position.y)
                StartCoroutine(Kill());
        }
    }

    IEnumerator Kill()
    {
        Debug.Log("KILL!!!");
        yield return new WaitForSeconds(0.8f);
        Destroy(this.gameObject);
    }
}
