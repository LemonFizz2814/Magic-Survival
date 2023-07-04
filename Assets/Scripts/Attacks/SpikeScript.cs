using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScript : ProjParticles
{
    public GameObject SpikeHitbox;
    //private ParticleSystem PartSys;
    [SerializeField] private List<GameObject> hitboxList = new List<GameObject>();
    private ParticleSystem.Particle[] PartList;

    // Start is called before the first frame update
    //protected override void Start()
    //{
    //    base.Start();
    //    hitboxList 
    //}

    //IEnumerator coroutine()
    //{
    //    yield return new WaitForSeconds(5.0f);
    //    Destroy(gameObject);
    //}

    protected override void MultiShotUpDate()
    {
        ParticleSystem.EmissionModule emission = PS.emission;
        ParticleSystem.ShapeModule shape = PS.shape;

        ParticleSystem.Burst burst = emission.GetBurst(0);
        burst.count = MultiShotLevel;
        emission.SetBurst(0, burst);
    }

    private void OnParticleCollision(GameObject col)
    {
        if (col.tag == "Ground")
        {
            foreach (GameObject hitbox in GameObject.FindGameObjectsWithTag("Spike"))
            {
                hitboxList.Add(hitbox);
            }

            //Despawning any hitboxes in current list
            if (hitboxList.Count > 0)
            {
                for (int i = 0; i < hitboxList.Count; i++)
                {
                    Destroy(hitboxList[i]);
                }
                hitboxList.Clear();
            }

            ParticleSystem subPS = GetComponent<ParticleSystem>();
            PartList = new ParticleSystem.Particle[subPS.main.maxParticles];
            int current_particles = subPS.GetParticles(PartList);

            for (int i = 0; i < current_particles; i++)
            {
                GameObject newSpike = Instantiate(SpikeHitbox, PartList[i].position, Quaternion.identity);
                
                //Despawn the hibox once the spike particle is gone
                Destroy(newSpike, PS.main.duration);
                //hitboxList.Add(newSpike);

            }
        }

        //    if (col.CompareTag("Enemy"))
        //    {
        //        EnemyScript enemy = col.GetComponent<EnemyScript>();
        //        enemy.DamageEnemy(enemy.CalculateDMG(player.GetAttackByName("Spike")), false);
        //    }
    }
}
