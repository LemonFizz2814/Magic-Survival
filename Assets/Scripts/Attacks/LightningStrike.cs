using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningStrike : MonoBehaviour
{
    [SerializeField] private BaseAttack lightningStats;
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void FixedUpdate()
    {
        transform.position = target.position;
    }

    private void OnEnable()
    {
        if (target == null) target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy collision by lightning strike");
            EnemyScript enemy = other.GetComponent<EnemyScript>();
            enemy.DamageEnemy(enemy.CalculateDMG(lightningStats), false);
        }
    }
}
