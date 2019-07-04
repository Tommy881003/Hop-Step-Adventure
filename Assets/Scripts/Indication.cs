using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Indication : MonoBehaviour
{
    private GameObject text, sprite;
    private Animator textAnim, spriteAnim;
    public bool isConsist;
    public UnityEvent action = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isConsist && collision.gameObject.tag == "Player" && Input.GetKeyDown(KeyCode.Return))
        {
            action.Invoke();
            Debug.Log("do something");
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
        yield return new WaitForSeconds(0.8f);
        Destroy(this.gameObject);
    }
}
