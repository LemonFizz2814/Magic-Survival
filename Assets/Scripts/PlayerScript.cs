using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

public class PlayerScript : MonoBehaviour
{
    [Header("Player spawned attacks (Check child objects)")]
    public ParticleSystem projectile;
    public GameObject spinningSawObject;
    public GameObject sentryObject;
    public GameObject spikeObject;
    public GameObject heads;
    public GameObject grenadeThrow;
    public GameObject electricPulse;
    public GameObject electricField;
    public GameObject ringShot;
    public ParticleSystem lightningVFX;

    [Header("Other player child objects")]
    public GameObject playerModel;
    public Animator modelAnimator;
    public ParticleSystem playerHurtVFX;
    public ParticleSystem absorbVFX;
    public Transform muzzleVFX;


    [Header("UI related scripts")]
    public InGameUIManager inGameUI;
    public MenuUIManager menuUI;
    public UpgradeManager upgradeManager;
    public UIManager uiManager;
    public UpgradeStats upgradeStats;
    public PlayerMovement playerMovement;
    public Joystick aimingJoystick;

    [Header("Manager scripts")]
    public PoolingManager poolingManager;
    public EnemySpawner enemySpawner;

    public CustomizeMenuManager customizeMenuManager;

    [Header("Player stats")]
    public int xpToLevelUp;
    public float xpIncr;
    public float maxHealthLevelUp;
    public float speedLevelUp;
    public List<KeyCode> allDebugKeys;

    int xp;
    int level;
    int coins;
    int score;
    float health;

    float fireRateTimer;
    float sentriesFireRateTimer;
    float lightningTimer;
    float spikeSpawnTimer;
    float regenerationTimer;

    private CameraTracking camTrack;

    bool paused = true;

    bool playingOnComputer = false;
    bool playingOnPhone = false;

    //int groundLayerMask = 1 << 0;
    public LayerMask groundLayerMask;

    List<GameObject> spinningSaws = new List<GameObject>();
    List<GameObject> sentries = new List<GameObject>();

    [System.Serializable]
    public struct UpgradableStats
    {
        [Header("Player stats")]
        public float playerSpeed;
        public float magnetStrength;

        [Header("Health stats")]
        public float maxHealth;
        public float regeneration;

        [Header("Projectile stats")]
        public int projectileSpeed;
        public int projectilePierce;
        public int criticalChance;
        public int projectiles;
        public float bulletDamage;
        public float bulletsSize;
        public float bulletRange;
        public float bulletKnockback;

        public float fireRate;
        public float accuracy;

        [Header("Projectile special stats")]
        public float homingStrength;
        public float explosionSize;
        public float explosionDamage;
        public float damageDistance;

        [Header("Special stats")]
        public float sawSpinSpeed;
        public float sentrySpinSpeed;
        public float sentryFireRate;
        public float lightningRate;
        public float lightningDamage;
        public float spikeDestroyDuration;
        public float spikeSpawnRate;
        public float baseDMGMultiplier;
        public float projectileDMGMultiplier;
        public float electricityDMGMultiplier;
        public float orbitalDMGMultiplier;
        public float lazerDMGMultiplier;
        public float explosiveDMGMultiplier;

    }

    [Header("Upgrade stats and attacks")]
    [SerializeField] private UpgradableStats upgradableStats;
    [SerializeField] private BaseAttack[] allAttacks;
    public UnityEvent<float, string> onValueChanged = new UnityEvent<float, string>();


    public enum UPGRADES
    {
        none,
        playerSpeed,
        maxHealth,
        projectileSpeed,
        fireRate,
        spread,
        magnet,
        knockback,
        glassCannon,
        homing,
        critical,
        sniper,
        extraProjectile,
        submachineGun,
        regeneration,
        explosion,
        spinningSaw,
        sentry,
        jackOfAllTrades,
        distanceDamage,
        lightningStrike,
        spike,
        lazerStrike,
        attributeDMG,
        chainLightning,
        electricPulse,
        electricField,
    };
    public enum ANIMATIONS
    {
        Idle,
        Walk,
        Die,
    };

    [Header("Audio")]
    public AudioClip SFX_Collect;
    public AudioClip SFX_Death;
    public AudioClip SFX_Menu_Pop_up;

    private void Start()
    {
        Time.timeScale = 1;
        health = upgradableStats.maxHealth;
        regenerationTimer = upgradableStats.regeneration;
        xp = 0;
        level = 0;
        score = 0;
        coins = PlayerPrefs.GetInt("Coins", 0);
        fireRateTimer = upgradableStats.fireRate;
        sentriesFireRateTimer = upgradableStats.sentryFireRate;
        lightningTimer = upgradableStats.lightningRate;
        spikeSpawnTimer = upgradableStats.spikeSpawnRate;

        if (allAttacks.Length > 0)
        {
            foreach (BaseAttack attack in allAttacks)
            {
                attack.InitValues();
            }
        }

        //update UI
        inGameUI.UpdateHealthBar(health, upgradableStats.maxHealth);
        inGameUI.UpdateXPBar(xp, xpToLevelUp);
        inGameUI.UpdateLevelText(level);
        inGameUI.UpdateCoinText(coins);
        inGameUI.UpdateScoreText(score);

        //Get camera
        camTrack = FindObjectOfType<CameraTracking>();

        playerMovement.UpdateMovemnentSpeed(upgradableStats.playerSpeed);

        SetPaused(true);
        SetHead();

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        playingOnComputer = true;
#endif
#if UNITY_IOS || UNITY_ANDROID || UNITY_IPHONE
        playingOnPhone = true;
#endif

        if (playingOnComputer)
        {
            aimingJoystick.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        //Matthew: Returning the update function if the game is paused so that none of the other
        //lines of code in this function will happen
        if (paused) return;

        fireRateTimer -= Time.deltaTime;
        sentriesFireRateTimer -= Time.deltaTime;
        lightningTimer -= Time.deltaTime;
        spikeSpawnTimer -= Time.deltaTime;

        if (playingOnComputer)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
            {
                playerModel.transform.LookAt(hit.point);
                playerModel.transform.localEulerAngles = new Vector3(0, playerModel.transform.localEulerAngles.y, 0);
            }
        }
        if (playingOnPhone && aimingJoystick.Direction.magnitude != 0)
        {
            playerModel.transform.localEulerAngles = new Vector3(0, Angle(aimingJoystick.Direction), 0);
        }

        //ParticleSystem tempBullet = projectile.GetComponent<ParticleSystem>();
        if ((aimingJoystick.Direction.magnitude != 0 && playingOnPhone) || (Input.GetMouseButton(0) && playingOnComputer))
        {
            //Matthew: Currently seperating the particle system bullets and placeholder bullets
            //to test things

            camTrack.TrackAim = true;
            //if (fireRateTimer <= 0)
            //{

            //    int amount = upgradableStats.projectiles;
            //    int angleRange = 5 + ((amount - 1) * 20);
            //    int newAngle = (amount == 1) ? 0 : angleRange / (amount - 1);
            //    Vector3 angle = playerModel.transform.rotation.eulerAngles;
            //    if (tempBullet == null)
            //    {
            //        for (int i = 0; i < amount; i++)
            //        {
            //            angle += new Vector3(0, (i * newAngle) + (-angleRange / 2), 0);
            //            FireProjectile(angle, new Vector3(transform.position.x, 0.5f, transform.position.z));
            //        }
            //    }
            //    else if (tempBullet != null)
            //    {
            //    }

            //    //Debug.Log("Pool amount: " + poolingManager.GetPoolAmount(PoolingManager.PoolingEnum.Bullet));

            //    fireRateTimer = upgradableStats.fireRate;
            //}

            FireProjectile();

            //if (tempBullet != null)
            //{
            //    projectile.SetActive(true);

            //    //Slighyly shifting the projectile upwards
            //    Vector3 pos = transform.position;
            //    pos.y = 0.5f;
            //    projectile.transform.position = pos;
            //    //projectile.transform.position = transform.position;

            //    Vector3 angle = playerModel.transform.rotation.eulerAngles;
            //    angle.x = 90.0f;
            //    projectile.transform.rotation = Quaternion.Euler(angle);
            //    //projectileObj.transform.rotation = Quaternion.identity;
            //}
        }
        else if ((aimingJoystick.Direction.magnitude == 0 && playingOnPhone) || (Input.GetMouseButtonUp(0) && playingOnComputer))
        {
            camTrack.TrackAim = false;

            if (!projectile.isStopped) projectile.Stop();
        }

        if (upgradableStats.regeneration > 0)
        {
            regenerationTimer -= Time.deltaTime;

            if (regenerationTimer <= 0)
            {
                regenerationTimer = 1; //every 1 seconds
                UpdateHealth(upgradableStats.regeneration);
            }
        }

        for (int i = 0; i < spinningSaws.Count; i++)
        {
            spinningSaws[i].transform.localEulerAngles += new Vector3(0, Time.deltaTime * upgradableStats.sawSpinSpeed, 0);
        }

        if (sentries.Count > 0)
        {
            for (int i = 0; i < sentries.Count; i++)
            {
                sentries[i].transform.localEulerAngles += new Vector3(0, Time.deltaTime * upgradableStats.sentrySpinSpeed, 0);
            }

            //if (sentriesFireRateTimer <= 0)
            //{
            //    sentriesFireRateTimer = upgradableStats.sentryFireRate;

            //    for (int i = 0; i < sentries.Count; i++)
            //    {
            //        Vector3 newAngle = new Vector3(
            //            sentries[i].transform.localEulerAngles.x + 45,
            //            sentries[i].transform.localEulerAngles.y,
            //            sentries[i].transform.localEulerAngles.z);
            //        FireProjectile(newAngle, sentries[i].transform.GetChild(0).position);
            //        //print(sentries[i].transform.GetChild(0).position);
            //    }
            //}
        }

        if (upgradableStats.lightningRate > 0 && lightningTimer <= 0)
        {
            lightningTimer = 10 - upgradableStats.lightningRate;

            List<GameObject> activeEnemies = enemySpawner.GetActiveEnemies();
            if (activeEnemies.Count > 0)
            {
                int randomEnemy = Random.Range(0, activeEnemies.Count);

                Vector3 pos = activeEnemies[randomEnemy].transform.position;
                pos = new Vector3(pos.x, 0.1f, pos.z);
                Destroy(Instantiate(lightningVFX, pos, Quaternion.identity), 5);
                activeEnemies[randomEnemy].GetComponent<EnemyScript>().DamageEnemy(upgradableStats.lightningDamage, true);
            }
        }

        //Spawning chain lightning
        //if (upgradableStats.chainLightningRate > 0 && chainLightningTimer <= 0)
        //{
        //    chainLightningTimer = 10 - upgradableStats.chainLightningRate;

        //    poolingManager.SpawnObject(PoolingManager.PoolingEnum.ChainLightning, new Vector3(transform.position.x, 0.5f, transform.position.z), Quaternion.identity);
        //}

        //Spawning pulse lightning
        //if (upgradableStats.electricPulseRate > 0 && electricPulseTimer <= 0)
        //{
        //    electricPulseTimer = 8 - upgradableStats.electricPulseRate;

        //    if (!electricPulse.activeSelf)
        //    {
        //        electricPulse.SetActive(true);
        //    }
        //}

        ////Spawning electric field
        //if (upgradableStats.electricFieldRate && !electricField.activeSelf)
        //{
        //    electricField.SetActive(true);
        //}

        foreach (BaseAttack attack in allAttacks)
        {
            if (!attack.SpawnCheck()) continue;
            bool attackNameFound = true;

            //If the attack is spawned from the player prefab, we'll need to locate the right child object
            //and check if it has not been activated
            if (attack.spawnSource == BaseAttack.SPAWNTYPE.PLAYER)
            {
                GameObject playerVFX = GetPlayerAttackObj(attack.name);   //Assigning it Electric field to prevent errors
                

                if (playerVFX != null && !playerVFX.activeSelf) playerVFX.SetActive(true);
                continue;
            }

            //If the attack spawns from the pooling manager, we need to set it's spawn position
            if (attack.spawnSource == BaseAttack.SPAWNTYPE.POOL)
            {
                Vector3 spawnPos = transform.position;
                spawnPos.y = 0.5f;

                for (int i = 0; i < attack.Amount; i++)
                {
                    switch (attack.name)
                    {
                        case "Lazer Strike":
                            spawnPos.x += Random.Range(-attack.Range, attack.Range);
                            spawnPos.z += Random.Range(-attack.Range, attack.Range);
                            break;
                        case "Chain Lightning":
                            break;
                        case "Bullet":
                            //Just make it do nothing. Bullets are enabled/disabled by the player
                            continue;
                            break;
                        default:
                            attackNameFound = false;
                            Debug.LogError(attack.name + " does not spawn any attacks on the Pool Manager. " +
                                "Please double check the name (Matthew)");
                            break;
                    }

                    if (attackNameFound) poolingManager.SpawnObject(attack.poolType, spawnPos, Quaternion.identity);

                }
                continue;
            }

            //If the attack is destroyed after a delay, set destroy timer
            //if (attack.spawnSource == BaseAttack.SPAWNTYPE.INSTANTIATE)
            //Note: Wait why can't we just use the pool manager to "destroy" them afterward?
        }

        if (upgradableStats.spikeSpawnRate > 0 && spikeSpawnTimer <= 0)
        {
            spikeSpawnTimer = 5 - upgradableStats.spikeSpawnRate;
            Destroy(Instantiate(spikeObject, transform.position, Quaternion.identity), upgradableStats.spikeDestroyDuration);
        }

        //Spawning grenades
        //if (upgradableStats.grenadeRate > 0)
        //{
        //    if (!grenadeThrow.activeSelf)
        //    {
        //        grenadeThrow.SetActive(true);

        //    }
        //}


        //Spawning Lazer Strikes
        //if (upgradableStats.lazerRate > 0 && lazerStrikeTimer <= 0)
        //{
        //    lazerStrikeTimer = 18 - upgradableStats.lazerRate;

        //    //Spawn a lazer near the player
        //    Vector3 spawnPos = transform.position;

        //    spawnPos.x += Random.Range(-6.0f, 6.0f);
        //    spawnPos.z += Random.Range(-6.0f, 6.0f);

        //    poolingManager.SpawnObject(PoolingManager.PoolingEnum.LazerStrike, spawnPos, Quaternion.identity);
        //}

        //DEBUG
        foreach (KeyCode key in allDebugKeys)
        {
            if (Input.GetKeyDown(key))
            {
                IncreaseXP(xpToLevelUp - xp, key);
            }
        }


        if (Input.GetKey(KeyCode.E))
        {
            for (int i = 1; i < customizeMenuManager.GetCustomizationSelectionsArray().Length; i++)
            {
                PlayerPrefs.SetInt("customization" + i, 1);
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("xp"))
        {
            IncreaseXP(other.GetComponent<XPScript>().GetXPGain());
            absorbVFX.Play();
            poolingManager.DespawnObject(other.gameObject);
            //Destroy(other.gameObject);
            //Debug.Log(gameObject.transform.name);
        }
        if (other.CompareTag("Coin"))
        {
            IncreaseCoins(1);
            absorbVFX.Play();
            poolingManager.DespawnObject(other.gameObject);
            //Destroy(other.gameObject);
        }
    }

    /*private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("xp"))
        {
            IncreaseXP(other.GetComponent<XPScript>().GetXPGain());
            absorbVFX.Play();
            poolingManager.DespawnObject(other.gameObject);
            //Destroy(other.gameObject);
        }
        if (other.CompareTag("Coin"))
        {
            IncreaseCoins(1);
            poolingManager.DespawnObject(other.gameObject);
            absorbVFX.Play();
            //Destroy(other.gameObject);
        }
    }*/

    public void SetPaused(bool _pause)
    {
        paused = _pause;

        playerMovement.SetPaused(_pause);

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemy.GetComponent<EnemyScript>().SetPaused(_pause);
        }
    }

    public void StartPressed()
    {
        SetPaused(false);
    }

    public void UpdateHealth(float _damage)
    {
        health += _damage;
        inGameUI.UpdateHealthBar(health, upgradableStats.maxHealth);

        if (_damage < 0)
        {
            playerHurtVFX.Play();
        }

        if (health <= 0)
        {
            PlayerDied();
        }
    }

    void PlayerDied()
    {
        if (menuUI.hasSound)
        {
            AudioSource.PlayClipAtPoint(SFX_Death, new Vector3(0, 0, 0), 1.0f);
        }

        if (score > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", score);
        }

        PlayAnimation(ANIMATIONS.Die);

        uiManager.ShowGameOverScreen(true);
        uiManager.ShowInGameUI(false);
        uiManager.ShowUpgradeUI(false);
        menuUI.UpdateScoreText(score);
        menuUI.UpdateHighScoreText(PlayerPrefs.GetInt("HighScore"));
        menuUI.GameOver(coins);

        enemySpawner.GameOver();

        SetPaused(true);
        //Time.timeScale = 0;
    }

    void IncreaseXP(int _xp, KeyCode _debugKey = KeyCode.None)
    {
        if (menuUI.hasSound)
        {
            AudioSource.PlayClipAtPoint(SFX_Collect, new Vector3(0, 0, 0), 1.0f);
        }

        xp += _xp;

        if (xp >= xpToLevelUp)
        {
            LeveledUp(_debugKey);
        }
        inGameUI.UpdateXPBar(xp, xpToLevelUp);
        inGameUI.UpdateLevelText(level);
    }
    public void IncreaseCoins(int _coin)
    {
        if (menuUI.hasSound)
        {
            AudioSource.PlayClipAtPoint(SFX_Collect, new Vector3(0, 0, 0), 1.0f);
        }

        coins += _coin;

        PlayerPrefs.SetInt("Coins", coins);

        inGameUI.UpdateCoinText(coins);
    }

    public void IncreaseScore(int _score)
    {
        score += _score;
        inGameUI.UpdateScoreText(score);
    }

    void LeveledUp(KeyCode _debugKey = KeyCode.None)
    {
        xp = 0;
        level++;

        upgradableStats.maxHealth += maxHealthLevelUp;
        upgradableStats.playerSpeed += speedLevelUp;
        UpdateStats();

        xpToLevelUp = (int)Mathf.Ceil(xpToLevelUp * xpIncr);

        if (menuUI.hasSound)
        {
            AudioSource.PlayClipAtPoint(SFX_Menu_Pop_up, new Vector3(0, 0, 0), 1.0f);
        }

        uiManager.ShowUpgradeUI(true);
        upgradeManager.QueueUpgrades(_debugKey);
        //SetPaused(true);
        //Time.timeScale = 0;
    }

    float Angle(Vector2 vector2)
    {
        if (vector2.x < 0)
        {
            return 360 - (Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg * -1);
        }
        else
        {
            return Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg;
        }
    }

    //This one is used to fire bullets from the player
    void FireProjectile()
    {
        if (!projectile.isPlaying) projectile.Play();

        Vector3 angle = playerModel.transform.rotation.eulerAngles;
        angle.y += -2.5f;
        //projectile.transform.localEulerAngles = angle;

        BaseAttack bullet = GetAttackByName("Bullet");

        //Timing the fire rate with the muzzle (here's hoping that works)
        if (!bullet.SpawnCheck(true)) return;
        muzzleVFX.rotation = Quaternion.Euler(angle);
        muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
    }

    //This one is used to fire bullets from the sentries
    void FireProjectile(Vector3 _direction, Vector3 _pos)
    {
        //GameObject projectileObj = Instantiate(projectile, _pos, Quaternion.identity);
        GameObject projectileObj = (projectile.gameObject == null) ?
            poolingManager.SpawnObject(PoolingManager.PoolingEnum.Bullet, _pos, Quaternion.identity) : projectile.gameObject;


        projectileObj.transform.localEulerAngles = _direction;

        ProjectileScript.ProjectileStats stats;
        stats.speed = upgradableStats.projectileSpeed;
        stats.range = upgradableStats.bulletRange;
        stats.damage = upgradableStats.bulletDamage / upgradableStats.projectiles;
        stats.piercing = upgradableStats.projectilePierce;
        stats.accuracy = upgradableStats.accuracy;
        stats.homingStrength = upgradableStats.homingStrength;
        stats.explosionSize = upgradableStats.explosionSize;
        stats.fireRate = upgradableStats.fireRate;


        //projectileObj.GetComponent<ProjectileScript>().FireProjectile(stats, isParticle);
        projectileObj.GetComponent<ParticleSystem>().Play();
        //TestForScriptableObject(projectileObj.GetComponent<ProjParticles>().bulletStats);

        muzzleVFX.rotation = Quaternion.Euler(_direction);
        muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>().Play();

        //GameObject muzzleObj = Instantiate(muzzleVFX, _pos, Quaternion.Euler(_direction));
        //muzzleObj.transform.SetParent(transform);
        //Destroy(muzzleObj, muzzleObj.transform.GetChild(0).GetComponent<ParticleSystem>().main.duration);
    }

    public void SetHead()
    {
        foreach (Transform child in heads.transform)
        {
            child.gameObject.SetActive(false);
        }

        heads.transform.GetChild(PlayerPrefs.GetInt("PlayerSkin")).gameObject.SetActive(true);
    }

    public UpgradableStats GetUpgradableStats()
    {
        return upgradableStats;
    }

    void UpdateMaxHealth()
    {
        if (health >= upgradableStats.maxHealth)
        {
            health = upgradableStats.maxHealth;
        }
    }

    void AddSpinningSaw()
    {
        GameObject sawObj = Instantiate(spinningSawObject, new Vector3(transform.position.x, 0.5f, transform.position.z), Quaternion.identity);
        sawObj.transform.parent = gameObject.transform;
        spinningSaws.Add(sawObj);

        float angle = 360 / spinningSaws.Count;

        //reset all spinning saws 
        for (int i = 0; i < spinningSaws.Count; i++)
        {
            spinningSaws[i].transform.localEulerAngles = new Vector3(0, angle * i, 0);
        }
    }
    void AddSentry()
    {
        GameObject sentryObj = Instantiate(sentryObject, new Vector3(transform.position.x, 0.5f, transform.position.z), Quaternion.identity);
        sentryObj.transform.parent = gameObject.transform;
        sentries.Add(sentryObj);

        //Get the sentry bullet
        ProjParticles bullet = sentryObj.transform.Find("Bullet").GetComponent<ProjParticles>();
        bullet.InitValues();
        bullet.ValueCheck();

        float angle = 360 / sentries.Count;

        //reset all spinning saws 
        for (int i = 0; i < sentries.Count; i++)
        {
            sentries[i].transform.localEulerAngles = new Vector3(0, angle * i, 0);
        }
    }

    //attk name and stat are for scriptable object attacks
    public void Upgrade(UpgradeStats.upgradeTiers _upgradeStats)
    {
        SetPaused(false);
        Time.timeScale = 1;

        bool isScriptableObject = false;
        switch (_upgradeStats.upgrade)
        {
            //If there is no upgrade enum selected, use _attkName and _stat to upgrade the scriptable object
            case UPGRADES.none:
                if (_upgradeStats.attackObj == null)
                {
                    Debug.LogError("Upgrade Enum is set to none yet there is no name for the attack object to retrieve");
                    return;
                }

                isScriptableObject = true;
                break;
            case UPGRADES.playerSpeed:
                upgradableStats.playerSpeed += _upgradeStats.positiveUpgrade;
                break;
            //Matthew: Sergio wants to remove the debuff for now
            case UPGRADES.maxHealth:
                upgradableStats.maxHealth += (int)_upgradeStats.positiveUpgrade;
                //upgradableStats.playerSpeed /= _upgradeStats.negativeUpgrade;
                break;
            case UPGRADES.projectileSpeed:
                upgradableStats.projectileSpeed += (int)_upgradeStats.positiveUpgrade;
                break;
            //TODO: Possibly need to get rid of this upgrade as it no longer works with bullet particles
            //case UPGRADES.fireRate:
            //    upgradableStats.fireRate += _upgradeStats.positiveUpgrade;
            //    break;
            //TODO: Change this to work with a new attack
            case UPGRADES.spread:
                upgradableStats.accuracy += _upgradeStats.positiveUpgrade;
                upgradableStats.bulletDamage *= _upgradeStats.negativeUpgrade;
                break;
            case UPGRADES.magnet:
                upgradableStats.magnetStrength += _upgradeStats.positiveUpgrade;
                break;
            case UPGRADES.knockback:
                upgradableStats.bulletKnockback += _upgradeStats.positiveUpgrade;
                break;
            case UPGRADES.glassCannon:
                upgradableStats.baseDMGMultiplier += _upgradeStats.positiveUpgrade;
                upgradableStats.maxHealth /= _upgradeStats.negativeUpgrade;
                break;
            case UPGRADES.homing:
                upgradableStats.homingStrength += _upgradeStats.positiveUpgrade;
                onValueChanged.Invoke(upgradableStats.homingStrength, "Homing");
                break;
            case UPGRADES.critical:
                upgradableStats.criticalChance += (int)_upgradeStats.positiveUpgrade;
                break;
            //Matthew: Removing the debuff for now due to sergio's request + change to new attack
            case UPGRADES.sniper:
                _upgradeStats.attackObj.Range += _upgradeStats.positiveUpgrade;
                //attack.FireRate += _upgradeStats.negativeUpgrade;
                break;
            //Matthew: removing the debuff here due to Sergio's request for now
            //case UPGRADES.extraProjectile:
            //    attack = GetAttackByName(_attkName);
            //    attack.Amount += (int)_upgradeStats.positiveUpgrade;
            //    //upgradableStats.projectiles += (int)_upgradeStats.positiveUpgrade;
            //    //attack.currentDMG += _upgradeStats.negativeUpgrade;
            //    break;
            //Matthew: Change this to make it work with a new attack
            case UPGRADES.submachineGun:
                _upgradeStats.attackObj.FireRate /= _upgradeStats.positiveUpgrade;
                _upgradeStats.attackObj.Range *= _upgradeStats.negativeUpgrade;
                break;
            case UPGRADES.regeneration:
                upgradableStats.regeneration += _upgradeStats.positiveUpgrade;
                break;
            case UPGRADES.explosion:
                upgradableStats.explosionSize += 1 + (_upgradeStats.positiveUpgrade * 0.5f);
                break;
            //Fix the misaligned spawning (Contact blake maybe)
            case UPGRADES.spinningSaw:
                upgradableStats.sawSpinSpeed += _upgradeStats.positiveUpgrade;
                AddSpinningSaw();
                break;
            case UPGRADES.sentry:
                upgradableStats.sentrySpinSpeed += _upgradeStats.positiveUpgrade;
                AddSentry();
                break;
            case UPGRADES.jackOfAllTrades:
                upgradableStats.maxHealth *= _upgradeStats.positiveUpgrade;
                upgradableStats.playerSpeed *= _upgradeStats.positiveUpgrade;

                //Adding the .1 in the value as to not make it too overpowered (may need a tweak idk)
                upgradableStats.baseDMGMultiplier += (_upgradeStats.positiveUpgrade % 1);
                break;
            case UPGRADES.distanceDamage:
                upgradableStats.damageDistance += _upgradeStats.positiveUpgrade;
                break;
            //Will need to be put in scriptable object (also target enemies, contact blake)
            case UPGRADES.lightningStrike:
                upgradableStats.lightningRate += _upgradeStats.positiveUpgrade;
                break;
            //Modify fire rate/amount
            case UPGRADES.spike:
                upgradableStats.spikeSpawnRate += _upgradeStats.positiveUpgrade;
                break;
            case UPGRADES.attributeDMG:
                SetAttributeDMG(_upgradeStats.attributeType, _upgradeStats.positiveUpgrade);
                break;
            default:
                Debug.LogWarning("Upgrade enum: " + _upgradeStats.upgrade + "Does nothing");
                return;
        }

        //Setting scriptable object values here (some upgrades may modify them outside of this bool check so check above this if statement)
        if (isScriptableObject)
        {
            switch (_upgradeStats.attkStat)
            {
                case UpgradeStats.ATTACKSTAT.FIRERATE:
                    if (!_upgradeStats.attackObj.enableSpawn)
                    {
                        //Sergio Made a rough patch here. Bullet Firerate was not upgrading reltive to the level. Ask for more details.
                        _upgradeStats.attackObj.enableSpawn = true;
                        //Upping the fire rate for bullet immediately
                        if (_upgradeStats.attackObj.name == "Bullet") _upgradeStats.attackObj.FireRate += _upgradeStats.positiveUpgrade;
                        

                        //Enabling the attack immediately if required
                        if (!_upgradeStats.attackObj.immediateSpawn) break;

                        GameObject playerAttack = GetPlayerAttackObj(_upgradeStats.attackObj.name);
                        playerAttack.SetActive(true);
                    }
                    else
                    {
                        _upgradeStats.attackObj.FireRate += _upgradeStats.positiveUpgrade;
                    }
                    break;
                case UpgradeStats.ATTACKSTAT.DAMAGE:
                    _upgradeStats.attackObj.currentDMG += _upgradeStats.positiveUpgrade;
                    break;
                case UpgradeStats.ATTACKSTAT.DURATION:
                    _upgradeStats.attackObj.Duration += _upgradeStats.positiveUpgrade;
                    break;
                case UpgradeStats.ATTACKSTAT.RANGE:
                    _upgradeStats.attackObj.Range += _upgradeStats.positiveUpgrade;
                    break;
                case UpgradeStats.ATTACKSTAT.SPEED:
                    _upgradeStats.attackObj.Speed += _upgradeStats.positiveUpgrade;
                    break;
                case UpgradeStats.ATTACKSTAT.AMOUNT:
                    _upgradeStats.attackObj.Amount += (int)_upgradeStats.positiveUpgrade;
                    break;
            }
        }

        UpdateStats();

        uiManager.ShowUpgradeUI(upgradeManager.CheckQueue());
    }

    //Sets the dmg value for damage multipliers
    private void SetAttributeDMG(BaseAttack.ATTRIBUTE _attribute, float _addedValue)
    {

        //Add modifiers in the order of Attack Stat enum (dmg, speed, firerate, duration, range, amount
        switch (_attribute)
        {
            case BaseAttack.ATTRIBUTE.ELECTRICITY:
                upgradableStats.electricityDMGMultiplier += _addedValue;
                break;
            case BaseAttack.ATTRIBUTE.ORBITAL_STRIKE:
                upgradableStats.orbitalDMGMultiplier += _addedValue;
                break;
            case BaseAttack.ATTRIBUTE.PROJECTILE:
                upgradableStats.projectileDMGMultiplier += _addedValue;
                break;
            case BaseAttack.ATTRIBUTE.LAZER:
                upgradableStats.lazerDMGMultiplier += _addedValue;
                break;
            case BaseAttack.ATTRIBUTE.EXPLOSIVE:
                upgradableStats.explosiveDMGMultiplier += _addedValue;
                break;
            case BaseAttack.ATTRIBUTE.NONE:
            default:
                Debug.LogError("Attribute not selected. Upgrade not applied");
                break;
        }
    }

    public void SetWalkAnimation()
    {
        Vector2 movement = playerMovement.GetMovementJoyStickDirection();

        Vector2 differenceDirection = movement - aimingJoystick.Direction;

        modelAnimator.SetFloat("Horizontal", differenceDirection.x);
        modelAnimator.SetFloat("Vertical", differenceDirection.y);
    }

    public void PlayAnimation(ANIMATIONS _animation)
    {
        switch (_animation)
        {
            case ANIMATIONS.Idle:
                modelAnimator.SetBool("IsWalking", false);
                break;

            case ANIMATIONS.Walk:
                modelAnimator.SetBool("IsWalking", true);
                break;

            case ANIMATIONS.Die:
                modelAnimator.SetBool("IsDead", true);
                break;
        }
    }

    public PoolingManager GetPoolingManager()
    {
        return poolingManager;
    }

    public BaseAttack GetAttackByName(string _name)
    {
        foreach (BaseAttack attack in allAttacks)
        {
            if (attack.name == _name) return attack;
        }

        Debug.LogError("Attack name: " + _name + " not found, this may cause certain attacks to not function");
        return null;
    }

    void UpdateStats()
    {
        UpdateMaxHealth();
        inGameUI.UpdateHealthBar(health, upgradableStats.maxHealth);
        playerMovement.UpdateMovemnentSpeed(upgradableStats.playerSpeed);
    }

    //Retrieve a player attack child object (Check the VFX child object under the player prefab)
    private GameObject GetPlayerAttackObj(string _name)
    {
        switch (_name)
        {
            case "Electric Pulse":
                return electricPulse;
                break;
            case "Grenade Throw":
                return grenadeThrow;
                break;
            case "Electric Field":
                return electricField;
                break;
            case "Ring Shot":
                return ringShot;
                break;
            default:
                Debug.LogError(_name + " does not spawn any attacks on the player. " +
                    "Please double check the name (Matthew)");
                return null;
        }
    }

    void TestForScriptableObject(BaseAttack _bullet)
    {
        if (_bullet.FireRate != 1.0f)
        {
            _bullet.FireRate = 1.0f;

        }
    }
}
