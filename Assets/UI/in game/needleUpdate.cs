using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class needleUpdate : MonoBehaviour
{
    [SerializeField]
    private Image needleImg;

    [SerializeField]
    private Slider slider;

    private float currentDepth = 0;
    private float targetDepth = 0;
    private float needleSpeed = 100.0f;


    public void setDepthFromSlider()
    {
        targetDepth = slider.value;
    }

    public void setDepth(float amt)
    {
        targetDepth = amt;
    }

    void updateDepth()
    {
        if(targetDepth > currentDepth)
        {
            currentDepth += Time.deltaTime * needleSpeed;
            currentDepth = Mathf.Clamp(currentDepth, 0.0f, targetDepth);

        } else if(targetDepth < currentDepth)
        {
            currentDepth -= Time.deltaTime * needleSpeed;
            currentDepth = Mathf.Clamp(currentDepth, targetDepth, 260.0f);
        }

        setNeedle();
    }

    void setNeedle()
    {
        needleImg.transform.localEulerAngles = new Vector3(0, 0, currentDepth);
    }

    // Update is called once per frame
    void Update()
    {
        if (targetDepth != currentDepth)
        {
            updateDepth();
        }
    }
}
