using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMG : ProjParticles
{
    protected override void FireRateUpDate()
    {
        var fireRate = PS.emission;

        fireRate.burstCount = 7;
    }
}
