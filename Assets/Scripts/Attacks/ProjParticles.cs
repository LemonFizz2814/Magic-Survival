using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ProjParticles : MonoBehaviour
{
    [Header("Attack stats")]
    public BaseAttack attackStats;
    [SerializeField] protected PlayerScript player;        //Need to grab upgradable stats for unique values
    public float explosionWait;
    public GameObject explosionObj;

    [Header("Particle values")]
    [SerializeField] protected ParticleSystem PS;
    private List<ParticleCollisionEvent> ProjEvents;

    //[Range(1,5)]
    [SerializeField]
    protected float FireRateLevel = 1;
    [SerializeField] [Range(1, 10)]
    protected int MultiShotLevel = 1;
    //[Range(1, 5)]
    [SerializeField]
    protected float BulletSpeedLevel = 1;
    [SerializeField]
    //[Range(1, 5)]
    protected float BulletRangeLevel = 1;
    [SerializeField]
    protected float HomingLevel = 0;
    [SerializeField]
    protected float BulletSizeLevel = 1;
    //[Range(0, 5)]
    //[Range(1, 5)]

    // Start is called before the first frame update
    protected virtual void Start()
    {
        InitValues();
        attackStats.onValueChanged.AddListener(OnValueChangeHandler);
        player.onValueChanged.AddListener(OnValueChangeHandler);
        ProjEvents = new List<ParticleCollisionEvent>();

    }

    // Update is called once per frame
    void Update()
    {
        //FireRateUpDate();
        //MultiShotUpDate();
        //BulletRangeUpDate();
        //HomingUpDate();
        //BulletSizeUpDate();
    }

    protected virtual void FireRateUpDate()
    {
        ParticleSystem.EmissionModule emission = PS.emission;

        ParticleSystem.Burst burst = emission.GetBurst(0);
        float FRL = FireRateLevel;
        burst.repeatInterval = 1 / FRL;
        emission.SetBurst(0, burst);
    }

    protected virtual void MultiShotUpDate()
    {
        // Remapping Equation
        // low2 + (value - low1) * (high2 - low2) / (high1 - low1)
        ParticleSystem.EmissionModule emission = PS.emission;
        ParticleSystem.ShapeModule shape = PS.shape;

        ParticleSystem.Burst burst = emission.GetBurst(0);
        float MSL = MultiShotLevel;
        burst.count = MultiShotLevel;
        emission.SetBurst(0, burst);

        shape.arc = (MSL - 1) * 120 / 9;
        shape.rotation = new Vector3(0, 0, 90 + (MSL - 1) * -60 / 9);

        //Type your other case here

    }

    protected virtual void BulletRangeUpDate()
    {
        float BSL = BulletSpeedLevel;
        float BRL = BulletRangeLevel;
        float MultiOffset = BRL / BSL;
        ParticleSystem.MainModule main = PS.main;
        main.startSpeed = BSL * 10;
        main.startLifetime = 0.5f * MultiOffset;
    }

    protected virtual void HomingUpDate()
    {
        ParticleSystem.ExternalForcesModule externalmodule = PS.externalForces;

        float HL = HomingLevel;
        externalmodule.multiplier = HL;
    }

    protected virtual void BulletSizeUpDate()
    {
        ParticleSystem.MainModule main = PS.main;

        float BSL = BulletSizeLevel;
        main.startSize = (BSL / 10) + 0.1f;
    }

    public void StartExplosion(Vector3 _pos)
    {
        PlayerScript.UpgradableStats specialStats = player.GetUpgradableStats();
        if (specialStats.explosionSize != 0)
        {
            GameObject obj = Instantiate(explosionObj, _pos, Quaternion.identity);

            obj.transform.localScale = new Vector3(specialStats.explosionSize, 
                specialStats.explosionSize, specialStats.explosionSize);
            Destroy(obj, explosionWait);
            Destroy(obj.GetComponent<SphereCollider>(), 0.1f);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.transform.tag == "Enemy")
        {
            EnemyScript enemy = other.GetComponent<EnemyScript>();

            //Get particle collision location
            int eventNum = PS.GetCollisionEvents(other, ProjEvents);

            enemy.HitByBullet(gameObject, ProjEvents[0].intersection);
        }
    }

    public void InitValues()
    {
        if (PS == null) PS = gameObject.GetComponent<ParticleSystem>();
        
        if (player == null) player = FindObjectOfType<PlayerScript>();
        
    }

    //For Sentries: Set the values of the bullet values after they're spawned in
    public void ValueCheck()
    {
        PlayerScript.UpgradableStats stats = player.GetUpgradableStats();
        MultiShotLevel = stats.projectiles;
        HomingLevel = stats.homingStrength;
        FireRateLevel = attackStats.FireRate;
        BulletRangeLevel = attackStats.Range;

        FireRateUpDate();
        MultiShotUpDate();
        BulletRangeUpDate();
        HomingUpDate();
    }

    protected void OnValueChangeHandler(float _newValue, string _variableName)
    {
        Debug.Log(_variableName + "'s value has changed to: " + _newValue);

        switch (_variableName)
        {
            case "Fire Rate":
                FireRateLevel = _newValue;
                FireRateUpDate();
                //Debug.Log("Fire rate is faster");
                break;
            case "Range":
                BulletRangeLevel = _newValue;
                BulletRangeUpDate();
                break;
            case "Homing":
                HomingLevel = _newValue;
                HomingUpDate();
                break;
            case "MultiShot":
                MultiShotLevel = (int)_newValue;
                MultiShotUpDate();
                break;
            case "Speed":
                BulletSpeedLevel = _newValue;
                BulletRangeUpDate();
                break;
        }
    }
}
