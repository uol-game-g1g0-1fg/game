using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergy : MonoBehaviour {
    [SerializeField] public float energyReserve = 100.0f;
    [SerializeField] float lowEnergyThreshold = 60.0f;
    [SerializeField] AnimationCurve energyRate;
    [SerializeField, Range(0.1f, 1f)] float loss = 0.1f;

    [Header("Events")]
    [SerializeField] GameEvent OnPlayerDeath;

    readonly float maxEnergy = 100.0f;
    readonly float minEnergy = 0.0f;

    public float Power => energyRate.Evaluate(energyReserve);
    public float Velocity => energyReserve / maxEnergy;

    private NotificationManager notificationManager;

    void Start()
    {
        notificationManager = GameObject.Find("/UI Manager").GetComponent<NotificationManager>();
    }

    void Update() {
        var isAboveLow = energyReserve > lowEnergyThreshold;

        energyReserve -= loss * Time.deltaTime;
        if (energyReserve <= minEnergy) energyReserve = minEnergy;

        // trigger low energy notification when crossing the threshold
        if (isAboveLow && energyReserve <= lowEnergyThreshold)
        {
            notificationManager.activate("Your Energy level is getting low! Find an Energy item before you run out", 10);
        }

        // die when no energy remaining
        if (energyReserve == minEnergy)
        {
            OnPlayerDeath.Invoke();
        }
    }

    public void Increase(float amount) {
        energyReserve += amount;
        if (energyReserve > maxEnergy) energyReserve = maxEnergy;
    }
}
