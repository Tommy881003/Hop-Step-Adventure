using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseHandler : MonoBehaviour
{
    private GameObject first, second, third, credit, keyInput, hint;
    private PlayerControl player;
    private bool setActive;

    private void Start()
    {
        first = this.transform.Find("FirstCarrier").gameObject;
        second = this.transform.Find("SecondCarrier").gameObject;
        third = this.transform.Find("ThirdCarrier").gameObject;
        credit = this.transform.Find("Credit").gameObject;
        hint = this.transform.Find("HintText").gameObject;
        if (third != null)
            keyInput = third.transform.Find("KeyInput").gameObject;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (first == null || second == null || third == null)
            return;
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (player != null && !player.dead && !player.isTransitioning && !player.complete && !player.isCutScene && !keyInput.activeSelf)
            {
                if (!first.activeSelf && !second.activeSelf && !third.activeSelf && !credit.activeSelf)
                    first.SetActive(true);
                else if (first.activeSelf)
                    first.SetActive(false);
                else if (credit.activeSelf)
                {
                    credit.SetActive(false);
                    first.SetActive(true);
                }
                else if (second.activeSelf)
                {
                    second.SetActive(false);
                    first.SetActive(true);
                }
                else
                {
                    third.SetActive(false);
                    second.SetActive(true);
                }
                Time.timeScale = ((first.activeSelf == false && second.activeSelf == false && third.activeSelf == false && credit.activeSelf == false) ? 1 : 0);
            }
            if (first.activeSelf || second.activeSelf || third.activeSelf || credit.activeSelf)
                hint.SetActive(true);
            else
                hint.SetActive(false);
        }
    }
}
