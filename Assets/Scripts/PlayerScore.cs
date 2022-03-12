using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{

    public int playerScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        ResetScore();
    }

    public void increaseScore(int scoreIncreaseAmount){
        playerScore += scoreIncreaseAmount;
        Debug.Log("PLAYERSCORE:  " + playerScore);
    }

    public void ResetScore(){
        playerScore = 0;
    }
}