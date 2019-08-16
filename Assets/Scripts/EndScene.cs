using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class EndScene : MonoBehaviour
{
    private TextMeshProUGUI texts, indication, thank;
    private Image image;
    public string toShow, toShowNext;
    private string currentString = "";
    private float delay = 0.05f;
    [SerializeField]
    private GameObject button;
    private SceneAudioManager audioManager;
    [SerializeField]
    private Result result = null;

    // Start is called before the first frame update
    void Start()
    {
        texts = this.transform.Find("LevelComplete").gameObject.GetComponent<TextMeshProUGUI>();
        indication = this.transform.Find("Indication").gameObject.GetComponent<TextMeshProUGUI>();
        thank = this.transform.Find("ThankYou").gameObject.GetComponent<TextMeshProUGUI>();
        image = this.GetComponentInChildren<Image>();
        if (button == null)
            DontDestroyOnLoad(this.gameObject);
        audioManager = GameObject.Find("SceneAudioManager").GetComponent<SceneAudioManager>();
    }

    public IEnumerator EndCutScene()
    {
        for (int i = 0; i < toShow.Length + 1; i ++)
        {
            currentString = toShow.Substring(0, i);
            texts.text = currentString;
            audioManager.PlayByName("Typing", audioManager.generalClips);
            yield return new WaitForSecondsRealtime(delay);
        }
        yield return new WaitForSeconds(1);
        indication.color = Color.white;
        yield return new WaitUntil(() => Input.anyKeyDown);
        image.DOFade(1, 1);
        audioManager.VolumeChange(audioManager.sceneClips, 0, 1);
        for (int i = toShow.Length; i >= 0; i--)
        {
            currentString = toShow.Substring(0, i);
            texts.text = currentString;
            yield return new WaitForSecondsRealtime(0.03f);
        }
        if (button == null)
        {
            yield return new WaitUntil(() => image.color == Color.black);
            indication.text = "";
            currentString = "";
            for (int i = 0; i < toShowNext.Length + 1; i++)
            {
                currentString = toShowNext.Substring(0, i);
                thank.text = currentString;
                audioManager.PlayByName("Typing", audioManager.generalClips);
                yield return new WaitForSecondsRealtime(delay);
            }
            AsyncOperation operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
            operation.allowSceneActivation = false;
            while (operation.progress < 0.9f)
                yield return null;
            yield return new WaitForSecondsRealtime(1);
            for (int i = toShowNext.Length; i >= 0; i--)
            {
                currentString = toShowNext.Substring(0, i);
                thank.text = currentString;
                yield return new WaitForSecondsRealtime(delay);
            }
            StartCoroutine(kill(operation));
            operation.allowSceneActivation = true;
        }
        else
        {
            yield return new WaitUntil(() => image.color == Color.black);
            if (result != null)
            {
                StartCoroutine(result.showResult());
                yield return new WaitUntil(() => result.done == true);
            }
            currentString = "";
            for (int i = 0; i < toShowNext.Length + 1; i++)
            {
                currentString = toShowNext.Substring(0, i);
                thank.text = currentString;
                audioManager.PlayByName("Typing", audioManager.generalClips);
                yield return new WaitForSecondsRealtime(delay);
            }
            button.SetActive(true);
        }
    }
    
    IEnumerator kill(AsyncOperation operation)
    {
        yield return new WaitUntil(() => operation.isDone);
        Destroy(this.gameObject);
    }
}
