using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Attack Stats", menuName = "New Attack")]
public class BaseAttack : ScriptableObject
{
    //Use this enum to attach special attributes for arttribute upgrades
    //(e.g: Increase all electricity dmg) or smth like that
    public enum ATTRIBUTE
    {
        ELECTRICITY,
        PROJECTILE,
        ORBITAL_STRIKE
    }
    
    //Use this to find out how to spawn the object
    public enum SPAWNTYPE
    {
        PLAYER,
        POOL,
        ENEMY,
        INSTANTIATE
    }

    [Header("UI Stuff")]
    public Sprite icon;
    public string description;

    [Header("Spawn Settings")]
    public bool enableSpawn = false;
    private float spawnTimer = 0.0f;
    public SPAWNTYPE spawnSource = SPAWNTYPE.POOL;

    //Use this enum to retrieve objects in the pooling manager
    public PoolingManager.PoolingEnum poolType = PoolingManager.PoolingEnum.Bullet;

    [Header("Base values")]
    public float currentDMG = 0.0f;
    [SerializeField] private List<ATTRIBUTE> bonusEffects;
    [SerializeField] private float currentSpeed, currentFireRate, currentDuration, currentRange;
    [SerializeField] private int currentAmount;
    public UnityEvent<float, string> onValueChanged = new UnityEvent<float, string>();

    [Header("Min/Max values")]
    [SerializeField] private float minDMG;
    [SerializeField]
    private float maxDMG = 1000.0f,
    minSpeed, maxSpeed,
    minFireRate, maxFireRate,
    minDuration, maxDuration,
    minRange, maxRange;
    [SerializeField] private int minAmount = 1, maxAmount;

    public void InitValues()
    {
        //onValueChanged.RemoveAllListeners();
        currentDMG = minDMG;
        currentSpeed = minSpeed;
        currentFireRate = minFireRate;
        currentDuration = minDuration;
        currentRange = minRange;
        currentAmount = minAmount;
        enableSpawn = false;
    }

    //Check if object spawn timer has reached the fire rate
    //(_ignoreSpawn = used for the bullet because the bullet does not need a spawn timer)
    public bool SpawnCheck(bool _ignoreSpawn = false)
    {
        if (!enableSpawn && !_ignoreSpawn) return false;

        spawnTimer += Time.deltaTime;

        if (spawnTimer < maxFireRate) return false;

        spawnTimer = minFireRate + currentFireRate;
        return true;
    }

    public float FireRate
    {
        get { return currentFireRate; }
        set { 
            if (currentFireRate != value)
            {
                if (value < minFireRate)
                {
                    currentFireRate = minFireRate;
                }
                else
                {
                    currentFireRate = value;
                }

                onValueChanged.Invoke(currentFireRate, "Fire Rate");
            }
        }
    }

    public float Range
    {
        get { return currentRange; }
        set {
            if (currentRange != value)
            {
                currentRange = value;
                onValueChanged.Invoke(currentRange, "Range");
            }
        }
    }

    public float Duration
    {
        get { return currentDuration; }
        set
        {
            if (currentDuration != value)
            {
                currentDuration = value;
                onValueChanged.Invoke(currentDuration, "Duration");
            }
        }
    }

    public float Speed
    {
        get { return currentSpeed; }
        set
        {
            if (currentSpeed != value)
            {
                currentSpeed = value;
                onValueChanged.Invoke(currentSpeed, "Speed");
            }
        }
    }

    public int Amount
    {
        get { return currentAmount; }
        set
        {
            if (currentAmount != value)
            {
                currentAmount = value;
                onValueChanged.Invoke(currentAmount, "MultiShot");
            }
        }
    }
}
