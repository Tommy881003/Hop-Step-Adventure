using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasResolution : MonoBehaviour
{
    private CanvasScaler canvas;

    // Start is called before the first frame update
    void Start()
    {
        canvas = this.GetComponent<CanvasScaler>();
        if (canvas != null)
            setRes();
    }

    void setRes()
    {
        float ratio = ((float)Screen.width / (float)Screen.height);
        float defaultRatio = 16f / 9f;
        if (ratio >= defaultRatio)
            canvas.matchWidthOrHeight = 1;
        else
            canvas.matchWidthOrHeight = 0;
    }
}
