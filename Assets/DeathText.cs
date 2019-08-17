using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeathText : MonoBehaviour
{
    private TextMeshProUGUI text;
    private Counter counter;

    // Start is called before the first frame update
    void Start()
    {
        text = this.GetComponent<TextMeshProUGUI>();
        counter = Resources.Load<Counter>("Scriptable Object/Counter");
    }

    // Update is called once per frame
    void Update()
    {
        text.text = counter.DieCounter.ToString();
    }
}
