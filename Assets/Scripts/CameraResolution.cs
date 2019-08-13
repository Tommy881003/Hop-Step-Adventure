using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = this.GetComponent<Camera>();
        if(cam != null)
            setRes();
    }

    void setRes()
    {
        float ratio = ((float)Screen.width / (float)Screen.height);
        float defaultRatio = ((float)16f / (float)9f);
        if (ratio >= defaultRatio)
            cam.orthographicSize = 11;
        else
            cam.orthographicSize = 40 / (2 * ratio);
    }
}
