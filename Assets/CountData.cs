using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class CountData : MonoBehaviour
{
    Counter counter;
    public GameObject TimerGroup, hdGroup;
    public TextMeshProUGUI timer, hdCounter;
    public Image image;
    private float runTime = 0;
    private PlayerControl player;
    private Color timerColor = Color.clear;
    private bool onTransition = false, reviving = false;
    private int previousAmount = 0, currentAmount = 0; 

    void Awake()
    {
        counter = Resources.Load<Counter>("Scriptable Object/Counter");
        counter.Initialize();
    }

    // Start is called before the first frame update
    void Start()
    {
        previousAmount = counter.HardDiskAmount;
        currentAmount = counter.HardDiskAmount;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        if (timer != null)
            timerColor = timer.color;
        if (counter.enableTimer == true)
        {
            TimerGroup.SetActive(true);
            hdGroup.transform.localPosition = Vector2.zero;
        }
        else
        {
            TimerGroup.SetActive(false);
            hdGroup.transform.localPosition = new Vector2(0,85);
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentAmount = counter.HardDiskAmount;
        if(player.isCutScene == false && player.complete == false)
            runTime += Time.unscaledDeltaTime;
        if (player.dead && reviving == false)
        {
            counter.DieCounter++;
            Debug.Log(counter.DieCounter);
            reviving = true;
        }
        if (reviving && player.dead == false)
            reviving = false;
        if (counter.enableTimer == true && TimerGroup.activeSelf == false)
            TimerGroup.SetActive(true);
        else if (counter.enableTimer == false && TimerGroup.activeSelf == true)
            TimerGroup.SetActive(false);
        if (TimerGroup.activeSelf == true && hdGroup.transform.localPosition.y != 0)
            hdGroup.transform.localPosition = Vector2.zero;
        else if (TimerGroup.activeSelf == false && hdGroup.transform.localPosition.y != 85)
            hdGroup.transform.localPosition = new Vector2(0, 85);
        if(TimerGroup.activeSelf == true)
        {
            string mili = Mathf.FloorToInt((runTime * 1000f) % 1000).ToString("000");
            string sec = Mathf.FloorToInt(runTime % 60).ToString("00");
            string min = Mathf.FloorToInt((runTime / 60f) % 60).ToString("00");
            string hour = Mathf.FloorToInt((runTime / 3600f) % 60).ToString("00");
            timer.text = hour + ":" + min + ":" + sec + "." + mili;
        }
        if (counter.enableTimer == true && player.isTransitioning && onTransition == false)
            StartCoroutine(showColor());
        if (currentAmount != previousAmount)
        {
            StartCoroutine(showDisk(previousAmount, currentAmount));
            previousAmount = currentAmount;
        }
        if (player.complete)
        {
            timer.color = Color.yellow;
            counter.timer = runTime;
            enabled = false;
        }
    }

    IEnumerator showColor()
    {
        onTransition = true;
        timer.color = Color.yellow;
        timer.DOColor(timerColor, 1).SetEase(Ease.InQuart).SetUpdate(true);
        yield return new WaitUntil(() => player.isTransitioning == false);
        onTransition = false;
    }

    IEnumerator showDisk(int previous, int current)
    {
        hdCounter.text = "x " + previous.ToString();
        image.DOColor(Color.white, 1f).SetUpdate(true);
        hdCounter.DOColor(Color.white, 1f).SetUpdate(true);
        yield return new WaitForSecondsRealtime(1);
        hdCounter.color = Color.yellow;
        hdCounter.text = "x " + current.ToString();
        hdCounter.transform.DOShakePosition(1, new Vector3(0, 10, 0), 8, 0).SetUpdate(true).SetEase(Ease.OutElastic);
        hdCounter.DOColor(Color.white, 1f).SetEase(Ease.InQuart).SetUpdate(true);
        yield return new WaitForSecondsRealtime(1);
        image.DOColor(Color.clear, 1f).SetUpdate(true);
        hdCounter.DOColor(Color.clear, 1f).SetUpdate(true);
    }
}
