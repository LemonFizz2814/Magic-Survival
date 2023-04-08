using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    PlayerScript player;
    [SerializeField] private BaseAttack grenadeStats;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        grenadeStats = player.GetAttackByName("Grenade Throw");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.transform.tag == "Enemy")
        {
            EnemyScript enemy = other.GetComponent<EnemyScript>();
            //Debug.Log("Grenade works");
            enemy.DamageEnemy(grenadeStats.currentDMG, true);
        }
    }
}
