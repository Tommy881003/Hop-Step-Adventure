using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Result : MonoBehaviour
{
    private Counter counter;
    private TextMeshProUGUI title, textRect, resultText, indication;
    private SceneAudioManager audioManager;
    public string[] titleStr, rectStr, resultStr;
    [HideInInspector]
    public bool done = false;

    void Start()
    {
        counter = Resources.Load<Counter>("Scriptable Object/Counter");
        audioManager = GameObject.Find("SceneAudioManager").GetComponent<SceneAudioManager>();
        title = this.transform.Find("Title").gameObject.GetComponent<TextMeshProUGUI>();
        textRect = this.transform.Find("TextRect").gameObject.GetComponent<TextMeshProUGUI>();
        resultText = this.transform.Find("ResultText").gameObject.GetComponent<TextMeshProUGUI>();
        indication = this.transform.Find("Indication").gameObject.GetComponent<TextMeshProUGUI>();
        title.text = "";
        resultText.text = "";
        textRect.text = "";
    }

    public IEnumerator showResult()
    {
        resultStr[0] += calTime(counter.timer);
        resultStr[1] += counter.DieCounter.ToString();
        resultStr[2] += counter.HardDiskAmount.ToString() + " / " + counter.MaxHardDiskAmount.ToString();
        title.color = Color.gray;
        textRect.color = Color.gray;
        resultText.color = Color.white;
        for (int i = 0; i < titleStr.Length; i++)
        {
            string newStr = titleStr[i] + "\n";
            for (int j = 0; j < newStr.Length; j++)
            {
                title.text += newStr.Substring(j, 1);
                audioManager.PlayByName("Typing", audioManager.generalClips);
                yield return new WaitForSecondsRealtime(0.03f);
            }
        }
        for (int i = 0; i < rectStr.Length; i++)
        {
            string newStr = rectStr[i] + "\n";
            rectStr[i] = textRect.text;
            textRect.text += newStr;
            yield return new WaitForSecondsRealtime(0.03f);
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < resultStr.Length; i++)
        {
            string newStr = resultStr[i] + "\n";
            for (int j = 0; j < newStr.Length; j++)
            {
                resultText.text += newStr.Substring(j, 1);
                audioManager.PlayByName("Typing", audioManager.generalClips);
                yield return new WaitForSecondsRealtime(0.03f);
            }
            yield return new WaitForSecondsRealtime(0.1f);
        }
        indication.color = Color.white;
        yield return new WaitUntil(() => Input.anyKeyDown);
        indication.color = Color.clear;
        StartCoroutine(Erase(resultText, 0.01f));
        yield return new WaitUntil(() => resultText.text.Length <= 1);
        StartCoroutine(Erase(title, 0.03f));
        StartCoroutine(Erase(textRect, 0.03f));
        yield return new WaitUntil(() => title.text.Length <= 1 && textRect.text.Length <= 1);
        done = true;
    }

    IEnumerator Erase(TextMeshProUGUI toOperate, float delay)
    {
        if(toOperate == textRect)
        {
            for(int i = rectStr.Length - 1; i >= 0; i --)
            {
                toOperate.text = rectStr[i];
                yield return new WaitForSecondsRealtime(delay);
            }
        }
        else
        {
            while(toOperate.text.Length != 0)
            {
                toOperate.text = toOperate.text.Substring(0, toOperate.text.Length - 1);
                yield return new WaitForSecondsRealtime(delay);
            }
        }
    }

    string calTime(float runTime)
    {
        string mili = Mathf.FloorToInt((runTime * 1000f) % 1000).ToString("000");
        string sec = Mathf.FloorToInt(runTime % 60).ToString("00");
        string min = Mathf.FloorToInt((runTime / 60f) % 60).ToString("00");
        string hour = Mathf.FloorToInt((runTime / 3600f) % 60).ToString("00");
        string result = hour + ":" + min + ":" + sec + "." + mili;
        return result;
    }
}
