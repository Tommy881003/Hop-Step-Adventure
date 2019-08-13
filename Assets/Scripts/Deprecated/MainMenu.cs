using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private GameObject text ,start, quit;

    void Start()
    {
        text = this.transform.Find("MainText").gameObject;
        start = this.transform.Find("StartCanvas").gameObject;
        quit = this.transform.Find("QuitCanvas").gameObject;
    }

    public void EnableStart()
    {
        start.SetActive(true);
        text.SetActive(false);
        StartCoroutine(TextReactivate());
    }

    public void EnableQuit()
    {
        quit.SetActive(true);
        text.SetActive(false);
        StartCoroutine(TextReactivate());
    }

    IEnumerator TextReactivate()
    {
        yield return new WaitUntil(() => start.activeSelf == false && quit.activeSelf == false);
        text.SetActive(true);
    }
}
