using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : ProjParticles
{


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        if (attackStats == null) attackStats = player.GetAttackByName("Grenade Throw");
        //player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        //grenadeStats = player.GetAttackByName("Grenade Throw");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void FireRateUpDate()
    {
        Debug.Log("Grenade Fire rate was triggered");
        //base.FireRateUpDate();
    }

    protected override void HomingUpDate()
    {
        Debug.Log("Grenade homing was triggered");
    }

    protected override void MultiShotUpDate()
    {
        base.MultiShotUpDate();
       
        /*ParticleSystem.EmissionModule emission = PS.emission;
        PlayerScript.UpgradableStats stats = player.GetUpgradableStats();

        ParticleSystem.Burst burst = emission.GetBurst(0);
        burst.count = stats.projectiles;*/
    }

    protected override void BulletRangeUpDate()
    {
        //base.BulletRangeUpDate();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.transform.tag == "Enemy")
        {
            EnemyScript enemy = other.GetComponent<EnemyScript>();
            //Debug.Log("Grenade works");
            float totalDMG = enemy.CalculateDMG(attackStats);
            enemy.DamageEnemy(totalDMG, true);
        }
    }
}
