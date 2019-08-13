using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class StartCutScene : MonoBehaviour
{
    [SerializeField]
    private GameObject level;
    private GameObject startScreen, cutScene, player;
    private PlayerControl control;
    private Image image;
    private TMPro.TextMeshProUGUI title, subTitle, indication, underLine, runMessage, error, suggestion;
    [SerializeField]
    private float typeSpeed = 0.03f, pauseSpeed = 0.3f;
    [SerializeField]
    private int errorBranchPoint, suggestionBranchPoint;
    [SerializeField]
    private string[] message, errorMessage, suggestionMessage, transMessage;
    private SpriteRenderer[] levelSprites;
    private SpriteRenderer playerRenderer;
    private SceneAudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        control = player.GetComponent<PlayerControl>();
        playerRenderer = player.GetComponent<SpriteRenderer>();
        image = this.GetComponentInChildren<Image>();
        startScreen = this.transform.Find("StartScreen").gameObject;
        cutScene = this.transform.Find("CutScene").gameObject;
        if(startScreen != null)
        {
            title = startScreen.transform.Find("Title").GetComponent<TMPro.TextMeshProUGUI>();
            subTitle = startScreen.transform.Find("SubTitle").GetComponent<TMPro.TextMeshProUGUI>();
            indication = startScreen.transform.Find("Indication").GetComponent<TMPro.TextMeshProUGUI>();
            underLine = startScreen.transform.Find("UnderLine").GetComponent<TMPro.TextMeshProUGUI>();
        }
        if(cutScene != null)
        {
            runMessage = cutScene.transform.Find("RunMessage").GetComponent<TMPro.TextMeshProUGUI>();
            error = cutScene.transform.Find("Error").GetComponent<TMPro.TextMeshProUGUI>();
            suggestion = cutScene.transform.Find("Suggestion").GetComponent<TMPro.TextMeshProUGUI>();
        }
        if (level != null)
            levelSprites = level.GetComponentsInChildren<SpriteRenderer>();
        control.isCutScene = true;
        audioManager = GameObject.Find("SceneAudioManager").GetComponent<SceneAudioManager>();
        StartCoroutine(PlayCutScene());
    }

    IEnumerator PlayCutScene()
    {
        yield return new WaitUntil(() => Input.anyKeyDown);
        yield return new WaitUntil(() => underLine.color.a < 0.5f);
        indication.color = Color.clear;
        underLine.enabled = false;
        StartCoroutine(Erase(title));
        StartCoroutine(Erase(subTitle));
        yield return new WaitUntil(() => title.color.a < 0.5f && subTitle.color.a < 0.5f);
        for(int i = 0; i < message.Length; i ++)
        {
            string current = message[i] + "\n";
            if (i == errorBranchPoint)
            {
                StartCoroutine(TypeWritter(error, message[i], true));
                yield return StartCoroutine(TypeWritter(runMessage, current, true));
                yield return new WaitForSeconds(3*pauseSpeed);
                yield return StartCoroutine(TypeWritter(error, errorMessage[0] + "\n", true));
            }
            else if(i == suggestionBranchPoint)
            {
                StartCoroutine(TypeWritter(runMessage, current, true));
                yield return StartCoroutine(TypeWritter(suggestion, suggestionMessage[0] + "\n", true));
            }
            else
                yield return StartCoroutine(TypeWritter(runMessage, current, true));
            yield return new WaitForSeconds(pauseSpeed);
        }
        cutScene.transform.DOLocalMoveY(678, 1.5f).SetEase(Ease.Linear);
        image.DOFade(0, 5);
        audioManager.VolumeChange("BGM", audioManager.sceneClips, 1, 5);
        yield return new WaitForSeconds(2);
        for (int i = 0; i < transMessage.Length; i++)
        {
            string current = transMessage[i] + "\n";
            if (i == 1)
            {
                yield return StartCoroutine(TypeWritter(runMessage, transMessage[i], true));
                yield return StartCoroutine(init());
            }
            else
                yield return StartCoroutine(TypeWritter(runMessage, current, true));
            yield return new WaitForSeconds(pauseSpeed);
        }
        control.isCutScene = false;
    }

    IEnumerator init()
    {
        string current = runMessage.text;
        float progress = 0;
        for(int i = 0; i < levelSprites.Length; i ++)
        {
            progress = (((float)i) / levelSprites.Length) * 100;
            levelSprites[i].enabled = true;
            runMessage.text = current + Mathf.RoundToInt(progress) + "%";
            yield return new WaitForSeconds(typeSpeed);
        }
        playerRenderer.color = Color.white;
        runMessage.text = current + "100% complete" + "\n";
    }

    IEnumerator Erase(TMPro.TextMeshProUGUI toProcess) 
    {
        string current = toProcess.text;
        for (int i = current.Length; i >= 0; i--)
        {
            toProcess.text = current.Substring(0, i);
            yield return new WaitForSeconds(typeSpeed);
        }
        toProcess.color = Color.clear;
    }

    IEnumerator TypeWritter(TMPro.TextMeshProUGUI toProcess, string toShow, bool keepText)
    {
        if(keepText)
        {
            for (int i = 0; i < toShow.Length; i++)
            {
                toProcess.text += toShow.Substring(i, 1);
                audioManager.PlayByName("Typing", audioManager.generalClips);
                yield return new WaitForSeconds(typeSpeed);
            }
        }
        else
        {
            for (int i = 0; i <= toShow.Length; i++)
            {
                toProcess.text = toShow.Substring(0, i);
                audioManager.PlayByName("Typing", audioManager.generalClips);
                yield return new WaitForSeconds(typeSpeed);
            }
        }
    }
}
