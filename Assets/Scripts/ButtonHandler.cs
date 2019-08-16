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
    public TMPro.TextMeshProUGUI[] buttonList;
    private GameObject showKey;
    private TMPro.TextMeshProUGUI key;
    public InputManager input;
    private bool canPress, canMove, canInput;
    private Event e;
    private KeyCode newKey;
    public GameObject nextCarrier;
    private SceneAudioManager audioManager = null;
    private Counter counter = null;

    void OnEnable()
    {
        buttonList[0].color = Color.yellow;
        control = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        audioManager = GameObject.Find("SceneAudioManager").GetComponent<SceneAudioManager>();
        counter = Resources.Load<Counter>("Scriptable Object/Counter");
        control.enabled = false;
        selectedButton = 0;
        canPress = false;
        canMove = true;
        canInput = false;
        if (input != null)
        {
            if(this.name == "ThirdCarrier")
            showKey = this.transform.Find("KeyInput").gameObject;
            if (showKey != null)
                key = showKey.transform.Find("Key").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
            foreach (TMPro.TextMeshProUGUI gui in buttonList)
            {
                if (gui.name == "Key" && gui.GetComponentInChildren<TMPro.TextMeshProUGUI>() != null)
                    gui.transform.Find(gui.text).gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = input.giveKeyCode(gui.text);
                else if (gui.name == "BGM")
                    gui.transform.Find("Volume").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "< " + Mathf.RoundToInt(audioManager.musicAmp * 10).ToString() + " >";
                else if (gui.name == "BGFX")
                    gui.transform.Find("Volume").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "< " + Mathf.RoundToInt(audioManager.fxAmp * 10).ToString() + " >";
                else if (gui.name == "Timer")
                    gui.transform.Find("Switch").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = ((counter.enableTimer == true)? "On" : "Off");
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
        if (buttonList[selectedButton].name == "BGM" || buttonList[selectedButton].name == "BGFX")
        {
            if (Input.GetKeyDown(input.right))
                ChangeVolume(buttonList[selectedButton].name, true);
            else if (Input.GetKeyDown(input.left))
                ChangeVolume(buttonList[selectedButton].name, false);
        }
        else
        {
            if (canPress == true && Input.GetKeyDown(KeyCode.Return))
                this.BroadcastMessage(buttonList[selectedButton].name + "Action");
        }
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
        if (nextCarrier != null)
            nextCarrier.SetActive(true);
        this.gameObject.SetActive(false);
    }

    void TimerAction()
    {
        counter.enableTimer = !counter.enableTimer;
        buttonList[selectedButton].transform.Find("Switch").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = ((counter.enableTimer == true) ? "On" : "Off");
    }

    void ChangeVolume(string name, bool plus)
    {
        float temp = 0;
        if(name == "BGM")
        {
            if(plus)
                audioManager.musicAmp = Mathf.Clamp(audioManager.musicAmp + 0.1f, 0, 1);
            else
                audioManager.musicAmp = Mathf.Clamp(audioManager.musicAmp - 0.1f, 0, 1);
            temp = audioManager.musicAmp;
        }
        else if(name == "BGFX")
        {
            if (plus)
                audioManager.fxAmp = Mathf.Clamp(audioManager.fxAmp + 0.1f, 0, 1);
            else
                audioManager.fxAmp = Mathf.Clamp(audioManager.fxAmp - 0.1f, 0, 1);
            temp = audioManager.fxAmp;
        }
        buttonList[selectedButton].transform.Find("Volume").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "< " + Mathf.RoundToInt(temp * 10).ToString() + " >";
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
        if(input.checkValid(newKey) == true)
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
