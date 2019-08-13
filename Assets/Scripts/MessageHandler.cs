using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageHandler : MonoBehaviour
{
    private Animator anim;
    private TMPro.TextMeshProUGUI user, access, message;
    private string userText, accessText, messageText;
    private bool isRunning;
    [SerializeField]
    private bool DoWait = false;
    private SceneAudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        user = this.transform.Find("UserText").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
        access = this.transform.Find("AccessText").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
        message = this.transform.Find("MessageText").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
        audioManager = GameObject.Find("SceneAudioManager").GetComponent<SceneAudioManager>();
        userText = user.text;
        accessText = access.text;
        messageText = message.text;
        user.text = "";
        access.text = "";
        message.text = "";
        isRunning = false;
    }

    public IEnumerator doMessage()
    {
        isRunning = true;
        user.text = "";
        access.text = "";
        message.text = "";
        anim.SetTrigger("Appear");
        string current = "";
        for(int i = 0; i < userText.Length + 1; i ++)
        {
            current = userText.Substring(0, i);
            user.text = current;
            audioManager.PlayByName("Typing", audioManager.generalClips);
            yield return new WaitForSeconds(0.03f);
        }
        if(DoWait)
            yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < accessText.Length + 1; i++)
        {
            current = accessText.Substring(0, i);
            access.text = current;
            audioManager.PlayByName("Typing", audioManager.generalClips);
            yield return new WaitForSeconds(0.03f);
        }
        if (DoWait)
            yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < messageText.Length + 1; i++)
        {
            current = messageText.Substring(0, i);
            message.text = current;
            audioManager.PlayByName("Typing", audioManager.generalClips);
            yield return new WaitForSeconds(0.03f);
        }
        isRunning = false;
    }

    public void showMessage()
    {
        StartCoroutine("doMessage");
    }

    public void stopMessage()
    {
        if(isRunning)
        {
            StopCoroutine("doMessage");
            isRunning = false;
        }
        anim.SetTrigger("Disappear");
    }
}
