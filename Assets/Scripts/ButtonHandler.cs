using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ButtonHandler : MonoBehaviour
{
    private PlayerControl control;
    public int selectedButton = 0;
    public TMPro.TextMeshProUGUI[] buttonList;
    public bool canPress;

    void OnEnable()
    {
        Time.timeScale = 0;
        buttonList[0].color = Color.yellow;
        control = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        control.enabled = false;
        selectedButton = 0;
        canPress = false;
    }

    void OnDisable()
    {
        buttonList[selectedButton].color = Color.white;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            MoveToNextButton();
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            MoveToPreviousButton();
        }

        if (canPress == true && Input.GetKeyDown(KeyCode.Return))
        {
            this.BroadcastMessage(buttonList[selectedButton].name + "Action");
        }

        canPress = true;
    }

    void MoveToNextButton()
    {
        buttonList[selectedButton].color = Color.white;
        selectedButton++;
        if (selectedButton >= buttonList.Length)
            selectedButton = buttonList.Length - 1;
        buttonList[selectedButton].color = Color.yellow;
    }

    void MoveToPreviousButton()
    {
        buttonList[selectedButton].color = Color.white;
        selectedButton--;
        if (selectedButton < 0)
            selectedButton = 0;
        buttonList[selectedButton].color = Color.yellow;
    }

    void StartAction()
    {
        this.transform.Find("Fade").GetComponent<Image>().DOFade(1, 0.5f).SetUpdate(true);
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Time.timeScale = 1;
    }

    void BackAction()
    {
        Time.timeScale = 1;
        control.enabled = true;
        this.gameObject.SetActive(false);
    }

    void QuitAction()
    {
        Application.Quit();
    }
}
