using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Chain lightning is an attack that zips to a target, damages it multiple times and searches for nearby enemies to
//zap again
public class LightningChain : MonoBehaviour
{
    public enum chainLightningState
    {
        SEARCH,
        CHASE,
        ATTACK,
    }

    [Header("Current state values")]
    public float timeTillCheck = 2.0f;
    public float searchRadius = 5.0f;
    [SerializeField] private chainLightningState currentState = chainLightningState.SEARCH;
    private float timer = 0.0f;
    private PlayerScript player;

    [Header("Movement")]
    public float moveSpeed = 20.0f;
    public EnemyScript target;
    private bool targetFound = false;

    [Header("Attack values")]
    public int enemyAmount = 5;
    public int hitChange = 3;
    private int hitNum = 0;
    public float attackRate = 0.6f;
    private float attkTimer = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
    }

    //This update function has a timer. As the timer is going, it will search, chase and attack a target.
    //Once the timer goes past a limit, it will reset the timer and search for a new target and if no target 
    //has been found then it despawns
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timeTillCheck)
        {
            switch (currentState)
            {
                //Despawn the object if it hasn't been able to search for any new enemy for awhile
                case chainLightningState.SEARCH:
                    player.poolingManager.DespawnObject(gameObject);
                    break;
                //Find a new target
                case chainLightningState.ATTACK:
                    currentState = chainLightningState.SEARCH;
                    targetFound = false;
                    break;
            }

            timer = 0;
        }
        else
        {
            switch (currentState)
            {
                case chainLightningState.SEARCH:
                    SearchForEnemies();
                    break;
                case chainLightningState.ATTACK:
                    //Search for a new target if current target is dead due to other causes
                    if (!target.gameObject.activeSelf)
                    {
                        currentState = chainLightningState.SEARCH;

                        //Reset the timer so that it is given enough time for a search
                        timer = 0;
                        targetFound = false;
                    }


                    //Stay with the target
                    transform.position = target.transform.position;

                    attkTimer += Time.deltaTime;
                    if (attkTimer >= attackRate)
                    {
                        //Debug.Log(attkTimer);
                        target.DamageEnemy(player.GetUpgradableStats().chainLightningDMG, true);
                        
                        hitNum++;
                        attkTimer = 0;

                        if (hitNum % hitChange == 0)
                        {
                            currentState = chainLightningState.SEARCH;

                            //Reset the timer so that it is given enough time for a search
                            timer = 0;
                            targetFound = false;

                        }


                        if (hitNum >= (hitChange * enemyAmount))
                        {
                            player.poolingManager.DespawnObject(gameObject);
                        }
                    }

                    
                    break;
                case chainLightningState.CHASE:
                    //There's a weird error that seems to occur when there is no target so this is a workaround 
                    //solution
                    if (target == null || !targetFound)
                    {
                        currentState = chainLightningState.SEARCH;
                        return;
                    }
                    //Creating another if statement in case the nearest target was already within the target's 
                    //collider (It causes the object to stay in the chase state)
                    else if (Vector3.Distance(target.transform.position, transform.position) <= 0.5f)
                    {
                        currentState = chainLightningState.ATTACK;
                        timer = 0;
                        return;
                    }
                    break;
            }
        }
    }

    //Handling the movement of the lightning chain if it is chasing
    private void FixedUpdate()
    {
        if (currentState == chainLightningState.CHASE && (target != null && targetFound))
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            direction.y = 0.5f;

            transform.position += (direction * moveSpeed) * Time.fixedDeltaTime;
        }


    }

    public void SearchForEnemies()
    {
        float closestDistance = searchRadius;
        LayerMask enemy = LayerMask.GetMask("Enemy");
        Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius, enemy);

        //Don't bother continuing this function if there aren't any enemies nearby
        if (colliders.Length == 0) return;

        foreach (Collider nearbyEnemy in colliders)
        {
            EnemyScript currentEnemy = nearbyEnemy.GetComponent<EnemyScript>();
            //Trying to find the nearest enemy
            float currentDistance = Vector3.Distance(transform.position, nearbyEnemy.transform.position);
            if (currentDistance < closestDistance && (nearbyEnemy != target && nearbyEnemy.gameObject.activeSelf))
            {
                targetFound = true;
                closestDistance = currentDistance;
                target = currentEnemy;
            }
        }

        CurrentState = chainLightningState.CHASE;
        timer = 0;
    }

    //Set/get the current lightning state
    public chainLightningState CurrentState
    {
        set
        {
            currentState = value;

            //Resetting the timer so it doesnt go on search mode immediately
            if (currentState == chainLightningState.ATTACK)
            {
                timer = 0;
            }
        }
        get { return currentState; }
    }

    private void OnEnable()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        if (currentState != chainLightningState.SEARCH) currentState = chainLightningState.SEARCH;
        hitNum = 0;
        timer = 0.0f;
        attkTimer = 0.0f;
    }
}
