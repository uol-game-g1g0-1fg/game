using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreTextUpdate : MonoBehaviour
{

    private TextMeshProUGUI finalScoreText;
    
    [SerializeField]
    private int score = 0;

    void Start()
    {
        finalScoreText = GetComponent<TextMeshProUGUI>();
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        score = GameObject.Find("Player").GetComponent<PlayerScore>().playerScore;

        if(score == 0){
            finalScoreText.text = "You didn't find any treasures on your journey to the surface.";
        }else{
            finalScoreText.text = "You found a total of " + score.ToString() + " treasures on your journey to the surface!";
        }

    }
}
