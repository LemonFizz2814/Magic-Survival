using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : ProjParticles
{
    protected override void FireRateUpDate()
    {
        ParticleSystem.EmissionModule emission = PS.emission;

        ParticleSystem.Burst burst = emission.GetBurst(0);
        float FRL = FireRateLevel;
        burst.repeatInterval = 1 / FRL;
        emission.SetBurst(0, burst);
    }
}
