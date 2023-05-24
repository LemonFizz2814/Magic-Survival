using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingShot : ProjParticles
{
    protected override void MultiShotUpDate()
    {
        ParticleSystem.EmissionModule emission = PS.emission;

        ParticleSystem.Burst burst = emission.GetBurst(0);
        float MSL = MultiShotLevel;
        burst.count = MultiShotLevel;
        emission.SetBurst(0, burst);
    }

    protected override void BulletRangeUpDate()
    {
        float BSL = BulletSpeedLevel;
        float BRL = BulletRangeLevel;
        
        ParticleSystem.MainModule main = PS.main;
        ParticleSystem ps = GetComponent<ParticleSystem>();
        var vel = ps.velocityOverLifetime;
        //vel.orbitalOffsetX = new Vector3(0,0, BSL * 5);
    }

    protected override void BulletSpeedUpdate()
    {
        base.BulletSpeedUpdate();
    }
}
