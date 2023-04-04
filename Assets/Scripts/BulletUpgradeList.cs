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
        ParticleSystem.MainModule main = PS.main;
        ParticleSystem.EmissionModule emission = PS.emission;
        ParticleSystem.ShapeModule shape = PS.shape;

        ParticleSystem.Burst burst = emission.GetBurst(0);
        float MSL = MultiShotLevel;
        burst.count = MultiShotLevel;
        emission.SetBurst(0, burst);

        shape.arc = 120 * (MSL / 10);
        shape.rotation = new Vector3(0, 0, shape.arc * (4 / MSL));
    }
}
