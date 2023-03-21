using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScript : MonoBehaviour
{
    public GameObject SpikeHitbox;
    private ParticleSystem PartSys;
    private ParticleSystem.Particle[] PartList;
    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.tag == "Spike")
        {
            StartCoroutine(coroutine());
        }
    }

    IEnumerator coroutine()
    {
        yield return new WaitForSeconds(5.0f);
        Destroy(gameObject);
    }

    private void OnParticleCollision(GameObject col)
    {
        if (col.tag == "Ground")
        {
            PartSys = GetComponent<ParticleSystem>();
            PartList = new ParticleSystem.Particle[PartSys.main.maxParticles];
            int current_particles = PartSys.GetParticles(PartList);

            for (int i = 0; i < current_particles; i++)
            {
                Instantiate(SpikeHitbox, PartList[i].position, Quaternion.identity, transform);
            }
        }
    }
}
