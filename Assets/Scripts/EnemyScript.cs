using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float maxHealth;
    public float damage;
    public float speed;
    public float range;
    public float attackWait;
    public float despawnWait;
    public int scoreIncrease;
    public int xp;

    bool paused = false;
    bool isDead = false;

    const float critcalTimer = 1.5f;

    int layerMask = 1 << 8;

    public float t;
    float health;

    GameObject player;
    MenuUIManager menuUI;
    PlayerScript playerScript;
    PoolingManager poolingManager;
    Rigidbody rb;
    EnemySpawner enemySpawner;
    public Animator enemyAnimator;
    public GameObject xpPellet;
    public GameObject criticalText;
    public GameObject impactVFX;

    [Header("Audio")]
    public AudioSource SFX_Destroy;

    public enum ANIMATIONS
    {
        Walk,
        Attack,
        Die,
        Injured,
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        menuUI = FindObjectOfType<MenuUIManager>();
        poolingManager = FindObjectOfType<PoolingManager>();
        enemySpawner = FindObjectOfType<EnemySpawner>();
        playerScript = player.GetComponent<PlayerScript>();
        rb = GetComponent<Rigidbody>();
        t = attackWait;
        Init();
    }

    public void Init()
    {
        t = attackWait;
        health = maxHealth;
        isDead = false;

        GetComponent<BoxCollider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
    }

    private void FixedUpdate()
    {
        if (!paused && !isDead)
        {
            //rotate to look at player
            transform.LookAt(player.transform);
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);

            //move towards player
            //transform.position += transform.forward * Time.deltaTime * speed;

            rb.AddForce(transform.forward * Time.deltaTime * speed, ForceMode.Force);

            if (t > 0)
            {
                t -= Time.deltaTime;
            }

            //Hit detection is only restricted in a straightline
            /*if (t <= 0)
            {
                RaycastHit hit;
                Debug.DrawRay(transform.position, transform.forward, Color.green, 1);

                if (Physics.Raycast(transform.position, transform.forward, out hit, range, layerMask))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        PlayAnimation(ANIMATIONS.Attack);
                        playerScript.UpdateHealth(damage);

                        t = attackWait + Random.Range(0.00f, 0.15f);
                    }
                }

                
            }*/
        }
    }

    private void OnCollisionStay(Collision col)
    {
        //Hits player when the enemy touches them
        if (col.transform.CompareTag("Player") && t <= 0)
        {
            PlayAnimation(ANIMATIONS.Attack);
            playerScript.UpdateHealth(damage);

            t = attackWait + Random.Range(0.00f, 0.15f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Matthew: Grenade throw collision is at Grenade script since particle system collisions work differently

        if (other.CompareTag("Projectile"))
        {
            //For particle system related collisions, check projParticle script onParticleCollision
            //Debug.Log("Collision works");
            HitByBullet(other.gameObject, other.transform.position);
        }
        if (other.CompareTag("Saw"))
        {
            DamageEnemy(playerScript.GetUpgradableStats().bulletDamage, true);
        }
        if (other.CompareTag("Explosion"))
        {
            DamageEnemy(playerScript.GetUpgradableStats().explosionDamage, true);
        }
        if (other.CompareTag("Spike"))
        {
            DamageEnemy(CalculateDMG(playerScript.GetAttackByName("Bullet")) / 4, true);
        }

        if (other.CompareTag("LazerStrike"))
        {
            DamageEnemy(CalculateDMG(playerScript.GetAttackByName("Lazer Strike")), true);
        }

        if (other.CompareTag("ElectricPulse"))
        {
            //Matthew: If anyone is curious, the electric pulse VFX prefab is using the lazer strike script
            //because they are similar to each other so it didn't take much for me to tweak the code for this
            BaseAttack attack = playerScript.GetAttackByName("Electric Pulse");
            DamageEnemy(CalculateDMG(attack), true);

        }



        if (other.CompareTag("ChainLightning"))
        {
            LightningChain lightning = other.GetComponent<LightningChain>();
            if (lightning.CurrentState != LightningChain.chainLightningState.ATTACK)
            {
                lightning.CurrentState = LightningChain.chainLightningState.ATTACK;

            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //Checking if the enemy is within the electric field (in which case we apply DOT dmg)
        //Based on fire rate
        if (other.CompareTag("ElectricField"))
        {
            //DamageEnemy(playerScript.GetUpgradableStats().electricFieldDMG, true);
            BaseAttack attack = playerScript.GetAttackByName("Electric Field");
            if (attack.SpawnCheck())
            {
                DamageEnemy(CalculateDMG(attack), true);
            }
            //Debug.Log("electric field works");
        }
    }

    public void HitByBullet(GameObject _bullet, Vector3 _pos)
    {
        //ProjectileScript projectile = _bullet.GetComponent<ProjectileScript>();
        //projectile.StartExplosion(transform.position);
        ProjParticles proj = _bullet.GetComponent<ProjParticles>();
        BaseAttack bulletStats = proj.attackStats;
        proj.StartExplosion(transform.position);


        DamageEnemy(CalculateDMG(bulletStats), false);
        Knockback();

        Vector3 direction = transform.position - _pos;
        Destroy(Instantiate(impactVFX, _pos, Quaternion.LookRotation(-direction)), impactVFX.GetComponent<ParticleSystem>().main.duration);

        //if (projectile.GetPiercing() <= 0)
        //{
        //    poolingManager.DespawnObject(_bullet);
        //    //Destroy(_bullet);
        //}
        //else
        //{
        //    projectile.SetPiercing();
        //}
    }

    void Knockback()
    {
        rb.AddForce(-transform.forward * playerScript.GetUpgradableStats().bulletKnockback, ForceMode.Force);
    }

    public void SetPaused(bool _pause)
    {
        paused = _pause;
    }

    //Return the damage after adding in all the bonus damage multipliers from attributes
    public float CalculateDMG(BaseAttack _attack)
    {
        if (_attack.bonusEffects.Count < 1) return _attack.currentDMG;

        PlayerScript.UpgradableStats stats = playerScript.GetUpgradableStats();
        float bonusDMG = 0.0f;

        foreach (BaseAttack.ATTRIBUTE statAttribute in _attack.bonusEffects)
        {
            switch (statAttribute)
            {
                case BaseAttack.ATTRIBUTE.ELECTRICITY:
                    bonusDMG += (_attack.currentDMG * stats.electricityDMGMultiplier);
                    break;
                case BaseAttack.ATTRIBUTE.LAZER:
                    bonusDMG += (_attack.currentDMG * stats.lazerDMGMultiplier);
                    break;
                case BaseAttack.ATTRIBUTE.ORBITAL_STRIKE:
                    bonusDMG += (_attack.currentDMG * stats.orbitalDMGMultiplier);
                    break;
                case BaseAttack.ATTRIBUTE.PROJECTILE:
                    bonusDMG += (_attack.currentDMG * stats.projectileDMGMultiplier);
                    break;
                case BaseAttack.ATTRIBUTE.EXPLOSIVE:
                    bonusDMG += (_attack.currentDMG * stats.explosiveDMGMultiplier);
                    break;
            }
        }

        return _attack.currentDMG + bonusDMG;
    }

    public void DamageEnemy(float _damage, bool _setDamage)
    {
        int rand = Random.Range(0, 100);

        if (rand < playerScript.GetUpgradableStats().criticalChance)
        {
            _damage *= 2;
            Destroy(Instantiate(criticalText, transform.position, Quaternion.identity), critcalTimer);
        }

        //Spawning damage numbers
        Vector3 spawnRange = transform.position;
        spawnRange.x += Random.Range(-2.0f, 2.0f);
        spawnRange.z += Random.Range(-2.0f, 2.0f);
        GameObject dmgNum = poolingManager.SpawnObject(PoolingManager.PoolingEnum.DMGNum, spawnRange, Quaternion.identity);
        //Checking whether there is an available damage number text on the pooling manager to prevent errors
        if (dmgNum != null)
        {
            TextMesh dmgText = dmgNum.transform.Find("TextMesh").GetComponent<TextMesh>();
            dmgText.text = _damage.ToString();

            //Despawning the damage number on a timer
            StartCoroutine(poolingManager.DespawnObjectTimer(dmgNum, 1.0f));
        }

        if (!_setDamage && playerScript.GetUpgradableStats().damageDistance > 0)
        {
            //float newDamage = _damage * ((Vector3.Distance(player.transform.position, transform.position) * playerScript.GetUpgradableStats().damageDistance));
            _damage += (Vector3.Distance(player.transform.position, transform.position) / 10) * playerScript.GetUpgradableStats().damageDistance;
            print("_damage: " + _damage);
        }

        health -= _damage;

        if (health <= 0)
        {
            EnemyDied();
        }
        else
        {
            PlayAnimation(ANIMATIONS.Injured);
        }
    }

    void EnemyDied()
    {
        if (menuUI.hasSound)
        {
            AudioSource.PlayClipAtPoint(SFX_Destroy.clip, new Vector3(0, 0, 0), 1.0f);
        }

        playerScript.IncreaseScore(scoreIncrease);
        if (poolingManager.CheckIfPoolFree(PoolingManager.PoolingEnum.XP))
        {
            GameObject xpObj = poolingManager.SpawnObject(PoolingManager.PoolingEnum.XP, transform.position, Quaternion.Euler(0, 45, 0));
            xpObj.GetComponent<XPScript>().Init();
            xpObj.GetComponent<XPScript>().SetXPGain(xp);
        }
        PlayAnimation(ANIMATIONS.Die);
        isDead = true;
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;

        StartCoroutine(DepsawnWait(despawnWait));
    }

    IEnumerator DepsawnWait(float _delay)
    {
        enemySpawner.RemoveEnemy(gameObject);
        yield return new WaitForSeconds(_delay);
        poolingManager.DespawnObject(this.gameObject);
    }

    public void PlayAnimation(ANIMATIONS _animation)
    {
        switch (_animation)
        {
            case ANIMATIONS.Attack:
                enemyAnimator.SetTrigger("Attack");
                break;

            case ANIMATIONS.Die:
                enemyAnimator.SetBool("isDead", true);
                break;

            case ANIMATIONS.Injured:
                enemyAnimator.SetTrigger("Injured");
                break;
        }
    }
}
