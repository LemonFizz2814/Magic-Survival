using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletUpgradeList : MonoBehaviour
{
    private ParticleSystem PS;
    [Range(1,5)]
    public int FireRateLevel = 1;
    [Range(1, 10)]
    public int MultiShotLevel = 1;
    [Range(1, 5)]
    public int BulletSpeedLevel = 1;
    [Range(1, 5)]
    public int BulletRangeLevel = 1;
    [Range(0, 5)]
    public int HomingLevel = 0;
    [Range(1, 5)]
    public int BulletSizeLevel = 1;

    // Start is called before the first frame update
    void Start()
    {
        PS = gameObject.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        FireRateUpDate();
        MultiShotUpDate();
        BulletRangeUpDate();
        HomingUpDate();
        BulletSizeUpDate();
    }

    void FireRateUpDate()
    {
        ParticleSystem.MainModule main = PS.main;
        ParticleSystem.EmissionModule emission = PS.emission;

        ParticleSystem.Burst burst = emission.GetBurst(0);
        float FRL = FireRateLevel;
        burst.repeatInterval = 1 / FRL;
        emission.SetBurst(0, burst);
    }

    void MultiShotUpDate()
    {
        // Remapping Equation
        // low2 + (value - low1) * (high2 - low2) / (high1 - low1)
        ParticleSystem.MainModule main = PS.main;
        ParticleSystem.EmissionModule emission = PS.emission;
        ParticleSystem.ShapeModule shape = PS.shape;

        ParticleSystem.Burst burst = emission.GetBurst(0);
        float MSL = MultiShotLevel;
        burst.count = MultiShotLevel;
        emission.SetBurst(0, burst);

        shape.arc = (MSL - 1) * 120 / 9;
        shape.rotation = new Vector3(0, 0, 90 + (MSL - 1) * -60 / 9);

    }

    void BulletRangeUpDate()
    {
        float BSL = BulletSpeedLevel;
        float BRL = BulletRangeLevel;
        float MultiOffset = BRL / BSL;
        ParticleSystem.MainModule main = PS.main;
        main.startSpeed = BSL * 10;
        main.startLifetime = 0.5f * MultiOffset;
    }

    void HomingUpDate()
    {
        ParticleSystem.MainModule main = PS.main;
        ParticleSystem.ExternalForcesModule externalmodule = PS.externalForces;

        float HL = HomingLevel;
        externalmodule.multiplier = HL;
    }

    void BulletSizeUpDate()
    {
        ParticleSystem.MainModule main = PS.main;

        float BSL = BulletSizeLevel;
        main.startSize = (BSL / 10) + 0.1f;
    }
}
