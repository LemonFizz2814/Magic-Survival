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

    public enum ATTRIBUTE
    {
        ELECTRICITY,
        PROJECTILE,
        ORBITAL_STRIKE
    }

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
    [SerializeField] float currentSpeed, currentFireRate, currentDuration, currentRange;

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
        enableSpawn = false;
    }

    public bool SpawnCheck()
    {
        if (!enableSpawn) return false;

        spawnTimer += Time.deltaTime;

        if (spawnTimer < currentFireRate) return false;

        spawnTimer = 0;
        return true;
    }

    public float fireRate
    {
        get { return currentFireRate; }
        set { }
    }
}
