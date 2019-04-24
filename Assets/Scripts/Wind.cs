using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    private WindZone wind;

    // Start is called before the first frame update
    void Start()
    {
        wind = this.GetComponent<WindZone>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics2D.gravity.y > 0)
            wind.windMain = 5;
        else
            wind.windMain = 0;
    }
}
