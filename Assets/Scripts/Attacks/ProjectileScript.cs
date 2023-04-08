using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public struct ProjectileStats
    {
        public float speed;
        public float range;
        public float damage;
        public float homingStrength;
        public float explosionSize;
        public int piercing;
        public float fireRate;
        public float accuracy;
    }

    float projectileSpeed;
    float destroyTimer;
    float damage;
    float homingStrength;
    float explosionSize;
    int piercing;
    bool homing = false;
    bool explosion = false;
    Transform closestEnemy;
    Rigidbody rb;
    PlayerScript player;
    Transform PlayerTransform;

    //Particle system
    [SerializeField]private ParticleSystem particle;
    private List<ParticleCollisionEvent> collisionEvents;


    public GameObject explosionObj;
    public float explosionWait;
    public bool isParticle = false;         //Adding this variable in to prevent it interfering with the placeholder

    private void Start()
    {
        homing = (homingStrength != 0);
        if (gameObject.GetComponent<Rigidbody>() != null) rb = gameObject.GetComponent<Rigidbody>();

        if (gameObject.GetComponent<ParticleSystem>() != null)
        {
            particle = gameObject.GetComponent<ParticleSystem>();
            collisionEvents = new List<ParticleCollisionEvent>();
        }
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void FireProjectile(ProjectileStats _projectileStats, bool _isParticle = false)
    {
        damage = _projectileStats.damage;
        homingStrength = _projectileStats.homingStrength;
        homing = (homingStrength != 0);
        explosionSize = _projectileStats.explosionSize;
        explosion = (explosionSize != 0);

        if (!isParticle)
        {
            projectileSpeed = _projectileStats.speed;
            piercing = _projectileStats.piercing;
            destroyTimer = _projectileStats.range;
        }
        else
        {
            if (gameObject.GetComponent<ParticleSystem>() != null && particle == null)
            {
                particle = gameObject.GetComponent<ParticleSystem>();
                //ParticleSystem.TriggerModule trigger;

                //GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

                //foreach(GameObject enemy in enemies)
                //{
                //    trigger.AddCollider(enemy.GetComponent<Collider>());
                //}
            }
                //Debug.Log("Particle state: " + ((particle != null) ? true : false));
            ParticleSystem.MainModule main = particle.main;
            ParticleSystem.EmissionModule emission = particle.emission;

            //Setting the fire rate
            ParticleSystem.Burst burst = emission.GetBurst(0);
            burst.repeatInterval = _projectileStats.fireRate;
            emission.SetBurst(0, burst);

            //Setting the speed
            main.startSpeed = _projectileStats.speed;

            //Setting the range
            main.startLifetime = _projectileStats.range;

            //main.loop = true;

            if (!particle.isPlaying)
            {
                particle.Play();
            }
            //ParticleSystem.EmissionModule emission = particle.emission;
            //emission.rateOverTime = _fireRate;
        }
        //if (particle != null)
        //{
        //    //ParticleSystem.MainModule main = particle.main;

        //    //main.startSpeed = projectileSpeed;
        //}

        //if (homing)
        //{
        //    float closestDistance = Mathf.Infinity;
        //    float checkForward    = 5;
        //    Vector3 newPos        = transform.position + (transform.forward * checkForward);

        //    foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        //    {
        //        float distance = Vector3.Distance(newPos, enemy.transform.position);

        //        if (distance < closestDistance)
        //        {
        //            closestEnemy = enemy.transform;
        //            closestDistance = distance;
        //        }
        //    }
        //}

        transform.forward += new Vector3(Random.Range(-_projectileStats.accuracy, _projectileStats.accuracy), 0, 0);
        if (!isParticle) StartCoroutine(DestroyWait());
    }

    private void FixedUpdate()
    {
        //transform.position = new Vector3(PlayerTransform.position.x, 0.5f, PlayerTransform.position.z);
        //transform.position += transform.forward * Time.deltaTime * projectileSpeed;
        //if (rb != null) rb.velocity = transform.forward * projectileSpeed;

        //if (homing && closestEnemy != null)
        //{
        //    Vector3 direction = closestEnemy.position - transform.position;
        //    Quaternion rotation = Quaternion.LookRotation(direction);
        //    rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, homingStrength * Time.deltaTime));
        //}
    }

    public void StartExplosion(Vector3 _pos)
    {
        if (explosion)
        {
            GameObject obj = Instantiate(explosionObj, _pos, Quaternion.identity);

            obj.transform.localScale = new Vector3(explosionSize, explosionSize, explosionSize);
            Destroy(obj, explosionWait);
            Destroy(obj.GetComponent<SphereCollider>(), 0.1f);
        }
    }

    public float GetDamage()
    {
        return damage;
    }

    public int GetPiercing()
    {
        return piercing;
    }
    public void SetPiercing()
    {
        piercing--;
        damage /= 2;
    }

    private IEnumerator DestroyWait()
    {
        yield return new WaitForSeconds(destroyTimer);
        player.GetPoolingManager().DespawnObject(gameObject);
        //Destroy(gameObject);
    }

    //private void OnParticleCollision(GameObject other)
    //{
    //    if (other.transform.tag == "Enemy")
    //    {
    //        EnemyScript enemy = other.GetComponent<EnemyScript>();

    //        //Get particle collision location
    //        int eventNum = particle.GetCollisionEvents(other, collisionEvents);

    //        enemy.HitByBullet(gameObject, collisionEvents[0].intersection);
    //    }
    //}
}
