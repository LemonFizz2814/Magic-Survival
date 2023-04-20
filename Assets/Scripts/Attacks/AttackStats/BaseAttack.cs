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
    public UnityEvent<float, string> onValueChanged = new UnityEvent<float, string>();

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
        //onValueChanged.RemoveAllListeners();
        currentDMG = minDMG;
        currentSpeed = minSpeed;
        currentFireRate = maxFireRate;
        currentDuration = minDuration;
        currentRange = minRange;
        enableSpawn = false;
    }

    //Check if object spawn timer has reached the fire rate
    public bool SpawnCheck()
    {
        if (!enableSpawn) return false;

        spawnTimer += Time.deltaTime;

        if (spawnTimer < currentFireRate) return false;

        spawnTimer = 0;
        return true;
    }

    public float FireRate
    {
        get { return currentFireRate; }
        set { 
            if (currentFireRate != value)
            {
                currentFireRate = value;
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
}
