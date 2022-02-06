using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDirector : MonoBehaviour
{

    public Light globalLight;
    public Camera mainCamera;
    public float fogDensityStart;
    public float fogDensityEnd;
    public Color fogColorStart;
    public Color fogColorEnd;
    public Color backgroundColorStart;
    public Color backgroundColorEnd;
    public float maxLevelHeight = 137.0f;

    // Start is called before the first frame update
    void Start()
    {
        //globalLight.intensity = 4.0f;
    }

    // Update is called once per frame
    void Update()
    {
        float t = 0; 
        if (maxLevelHeight > 0)
            t = (transform.position.y / maxLevelHeight);
        globalLight.intensity = 0.6f + t;
        RenderSettings.fogColor = Color.Lerp(fogColorStart,fogColorEnd, t);
        RenderSettings.fogDensity = Mathf.Lerp(fogDensityStart, fogDensityEnd, t);
        mainCamera.backgroundColor = Color.Lerp(backgroundColorStart,backgroundColorEnd, t);
    }
}
