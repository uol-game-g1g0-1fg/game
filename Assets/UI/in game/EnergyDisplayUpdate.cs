using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnergyDisplayUpdate : MonoBehaviour
{

    private TextMeshProUGUI energyText;
    private int energy;

    void Start()
    {
        energyText = GetComponent<TextMeshProUGUI>();
        energy = 100;
    }

    // Update is called once per frame
    void Update()
    {
        energy = GameObject.Find("Player").GetComponent<PlayerEnergy>().energy;
        energyText.text = energy.ToString();

    }
}
