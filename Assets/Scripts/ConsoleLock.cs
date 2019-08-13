using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleLock : MonoBehaviour
{
    [SerializeField]
    MessageHandler message;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            message.showMessage();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            message.stopMessage();
    }
}
