using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergy : MonoBehaviour {

    [SerializeField] float energyReserve = 100.0f;
    [SerializeField] AnimationCurve energyRate;
    [SerializeField, Range(0.1f, 1f)] float loss = 0.1f;
    readonly float maxEnergy = 100.0f;
    readonly float minEnergy = 0.0f;

    public float Power => energyRate.Evaluate(energyReserve);
    public float Velocity => energyReserve / maxEnergy;

    void Update() {
        energyReserve -= loss * Time.deltaTime;
        if (energyReserve <= minEnergy) energyReserve = minEnergy;
    }

    public void Increase(float amount) {
        energyReserve += amount;
        if (energyReserve > maxEnergy) energyReserve = maxEnergy;
    }
}
