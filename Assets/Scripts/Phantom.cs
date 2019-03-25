using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phantom : MonoBehaviour
{ 
    private float killTime = 0;

    private void Update()
    {
        killTime += Time.deltaTime;
        if (killTime >= 0.25f)
            Destroy(this.gameObject);
    }
}
