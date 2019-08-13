using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using DG.Tweening;

public class ButtonHandler : MonoBehaviour
{
    public event EventHandler OnRetry;
    private PlayerControl control;
    private int selectedButton = 0;
    [SerializeField]
    private TMPro.TextMeshProUGUI[] buttonList;
    private GameObject showKey;
    private TMPro.TextMeshProUGUI key;
    [SerializeField]
    private InputManager input;
    private bool canPress, canMove, canInput;
    private Event e;
    private KeyCode newKey;
    [SerializeField]
    private GameObject keyOption;

    void OnEnable()
    {
        buttonList[0].color = Color.yellow;
        control = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        control.enabled = false;
        selectedButton = 0;
        canPress = false;
        canMove = true;
        canInput = false;
        if (input != null)
        {
            if(this.name == "SecondCarrier")
            showKey = this.transform.Find("KeyInput").gameObject;
            if (showKey != null)
                key = showKey.transform.Find("Key").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
            foreach (TMPro.TextMeshProUGUI gui in buttonList)
            {
                if (gui.name == "Key" && gui.GetComponentInChildren<TMPro.TextMeshProUGUI>() != null)
                    gui.transform.Find(gui.text).gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = input.giveKeyCode(gui.text);
            }
        }
    }

    void OnDisable()
    {
        if (this.name == "FirstCarrier")
            control.enabled = true;
        buttonList[selectedButton].color = Color.white;
    }

    void Update()
    {        
        if (Input.GetKeyDown(input.down) && canMove)
            MoveToNextButton();
        else if (Input.GetKeyDown(input.up) && canMove)
            MoveToPreviousButton();
        if (canPress == true && Input.GetKeyDown(KeyCode.Return))
            this.BroadcastMessage(buttonList[selectedButton].name + "Action");
        canPress = true;
    }

    private void OnGUI()
    {
        e = Event.current;
        if (e.isKey && !canMove && canInput && Input.anyKeyDown)
        {
            newKey = e.keyCode;
            canInput = false;
        }
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
    }

    void BackAction()
    {
        Time.timeScale = 1;
        control.enabled = true;
        this.gameObject.SetActive(false);
    }

    void OptionAction()
    {
        if (keyOption != null)
            keyOption.SetActive(true);
        this.gameObject.SetActive(false);
    }

    void KeyAction()
    {
        StartCoroutine(keyInput());
    }

    IEnumerator WaitForKey()
    {
        while (!e.isKey || !Input.anyKeyDown)
            yield return null;
    }

    IEnumerator keyInput()
    {
        canInput = true;
        canMove = false;
        showKey.SetActive(true);
        key.text = buttonList[selectedButton].text;
        yield return new WaitForEndOfFrame();
        yield return WaitForKey();
        if(newKey != KeyCode.None && newKey != KeyCode.Return && newKey != KeyCode.Escape)
        {
            input.setKey(buttonList[selectedButton].text, newKey);
            buttonList[selectedButton].transform.Find(buttonList[selectedButton].text).gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = newKey.ToString();
        }
        showKey.SetActive(false);
        canMove = true;
    }

    void ResetToDefaultAction()
    {
        input.Reset();
        foreach (TMPro.TextMeshProUGUI gui in buttonList)
        {
            if (gui.name == "Key" && gui.GetComponentInChildren<TMPro.TextMeshProUGUI>() != null)
                gui.transform.Find(gui.text).gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = input.giveKeyCode(gui.text);
        }
    }

    void RetryAction()
    {
        if (OnRetry != null)
        {
            OnRetry(this, EventArgs.Empty);
            Time.timeScale = 1;
        }
        BackAction();
    }

    void ReplayAction()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void ToMenuAction()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    void QuitAction()
    {
        Application.Quit();
    }
}
