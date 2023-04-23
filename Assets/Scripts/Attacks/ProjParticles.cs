using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ProjParticles : MonoBehaviour
{
    [Header("Attack stats")]
    public BaseAttack bulletStats;
    private PlayerScript player;        //Need to grab upgradable stats for unique values
    public float explosionWait;
    public GameObject explosionObj;

    [Header("Particle values")]
    public ParticleSystem PS;
    private List<ParticleCollisionEvent> ProjEvents;

    //[Range(1,5)]
    public float FireRateLevel = 1;
    [Range(1, 10)]
    public int MultiShotLevel = 1;
    //[Range(1, 5)]
    public float BulletSpeedLevel = 1;
    //[Range(1, 5)]
    public float BulletRangeLevel = 1;
    //[Range(0, 5)]
    public float HomingLevel = 0;
    //[Range(1, 5)]
    public float BulletSizeLevel = 1;

    // Start is called before the first frame update
    void Start()
    {
        bulletStats.InitValues();
        PS = gameObject.GetComponent<ParticleSystem>();
        bulletStats.onValueChanged.AddListener(OnValueChangeHandler);
        player = FindObjectOfType<PlayerScript>();
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

    protected void OnValueChangeHandler(float _newValue, string _variableName)
    {
        Debug.Log(_variableName + "'s value has changed to: " + _newValue);

        switch (_variableName)
        {
            case "Fire Rate":
                FireRateLevel = _newValue;
                FireRateUpDate();
                break;
            case "Range":
                BulletRangeLevel = _newValue;
                BulletRangeUpDate();
                break;
                
        }
    }
}