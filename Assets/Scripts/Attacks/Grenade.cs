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
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.transform.tag == "Enemy")
        {
            EnemyScript enemy = other.GetComponent<EnemyScript>();
            //Debug.Log("Grenade works");
            enemy.DamageEnemy(attackStats.currentDMG, true);
        }
    }
}
