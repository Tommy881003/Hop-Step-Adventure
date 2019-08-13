using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseHandler : MonoBehaviour
{
    private GameObject first, second, keyInput;
    private PlayerControl player;
    private bool setActive;

    private void Start()
    {
        first = this.transform.Find("FirstCarrier").gameObject;
        second = this.transform.Find("SecondCarrier").gameObject;
        if(second != null)
            keyInput = second.transform.Find("KeyInput").gameObject;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (first == null || second == null)
            return;
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (player != null && !player.dead && !player.isTransitioning && !player.complete && !player.isCutScene && !keyInput.activeSelf)
            {
                if (!first.activeSelf || (first.activeSelf && !second.activeSelf))
                    first.SetActive(!first.activeSelf);
                if (second.activeSelf)
                    second.SetActive(false);
                Time.timeScale = ((first.activeSelf == false && second.activeSelf == false) ? 1 : 0);
            }
        }
    }
}
