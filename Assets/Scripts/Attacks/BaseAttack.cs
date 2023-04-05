using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Attack Stats", menuName = "New Attack")]
public class BaseAttack : ScriptableObject
{
    //public enum ATTACKSTAT
    //{
    //    DAMAGE,
    //    SPEED,
    //    FIRERATE,
    //    DURATION,
    //    RANGE
    //}

    [Header("UI Stuff")]
    public Sprite icon;
    public string description;

    [Header("Base values")]
    public float currentDMG = 0.0f;
    public float currentSpeed, currentFireRate, currentDuration, currentRange;
    private float spawnTimer = 0.0f;

    [Header("Min/Max values")]
    [SerializeField] private float minDMG;
    [SerializeField]
    private float maxDMG = 1000.0f,
    minSpeed, maxSpeed,
    minFireRate, maxFireRate,
    minDuration, maxDuration,
    minRange, maxRange;

    public void InitValues()
    {
        currentDMG = minDMG;
        currentSpeed = minSpeed;
        currentFireRate = maxFireRate;
        currentDuration = minDuration;
        currentRange = minRange;
    }
}
