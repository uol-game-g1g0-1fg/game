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

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private float levelHeight = 136f;

    private float currentDepth = 0;
    private float targetDepth = 0;
    private float needleSpeed = 100.0f;

    private float levelRange = 136f - 0.75f;
    private float gaugeRange = 130 - (-130);


    public void setDepthFromSlider()
    {
        targetDepth = slider.value;
    }

    public void setDepth(float amt)
    {
        //targetDepth = amt - levelHeight;

        targetDepth = (((amt - 0.75f) * gaugeRange) / levelRange) - 130f;
    }

    void updateDepth()
    {
        if (targetDepth > currentDepth)
        {
            currentDepth += Time.deltaTime * needleSpeed;
            currentDepth = Mathf.Clamp(currentDepth, -130f, targetDepth);

        }
        else if (targetDepth < currentDepth)
        {
            currentDepth -= Time.deltaTime * needleSpeed;
            currentDepth = Mathf.Clamp(currentDepth, targetDepth, 130.0f);
        }

        setNeedle();
    }

    void setNeedle()
    {
        needleImg.transform.localEulerAngles = new Vector3(0, 0, currentDepth);
        //needleImg.transform.rotation = Quaternion.Euler(0, 0, currentDepth);
    }

    // Update is called once per frame
    void Update()
    {

        setDepth(player.transform.position.y);
        Debug.Log("depth: " + targetDepth);

        if (targetDepth != currentDepth)
        {
            updateDepth();
        }
    }
}
