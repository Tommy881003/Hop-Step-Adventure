using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loop : MonoBehaviour
{
    private GameObject left;
    private GameObject median;
    private GameObject right;
    public GameObject[] adjust;
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        left = GameObject.Find("Left");
        median = GameObject.Find("Median");
        right = GameObject.Find("Right");
    }

    private void Update()
    {
        float playerX = player.transform.position.x - 20;
        float distR = Mathf.Abs(playerX - right.transform.position.x);
        float distL = Mathf.Abs(playerX - left.transform.position.x);
        if(distL < 10 || distR < 10)
        {
            if (distL < 10)
            {
                right.transform.position = new Vector2(left.transform.position.x - 40, right.transform.position.y);
                GameObject temp = right;
                right = median;
                median = left;
                left = temp;
            }
            if (distR < 10)
            {
                left.transform.position = new Vector2(right.transform.position.x + 40, right.transform.position.y);
                GameObject temp = left;
                left = median;
                median = right;
                right = temp;
            }
            foreach (GameObject adj in adjust)
                adj.GetComponent<SpritePicker>().PickSprite("GrassLand");
        }
    }
}
