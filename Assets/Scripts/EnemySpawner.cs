using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float radius;

    float spawnTimer = 0;
    public int wave;
    public bool dontSpawn;

    public PoolingManager poolingManager;

    bool started = false;
    bool gameOver = false;

    //List<GameObject> enemiesToSpawn = new List<GameObject>();

    GameObject player;

    [System.Serializable]
    public struct EnemySpawnWaves
    {
        public float duration;
        public float spawnRate;
        public int amountToSpawn;
        public PoolingManager.PoolingEnum[] enemiesToSpawn;
    }

    public EnemySpawnWaves[] enemySpawnWaves;
    List<GameObject> activeEnemies = new List<GameObject>();

    public void Awake()
    {
        gameOver = false;
        spawnTimer = enemySpawnWaves[wave].duration;
    }

    public void StartPressed()
    {
        //Matthew: returning the function if the enemy is not supposed to spawn
        if (dontSpawn) return;

        StartCoroutine(WaitToSpawn());
        player = GameObject.FindGameObjectWithTag("Player");
        started = true;
    }

    private void Update()
    {
        //Matthew: returning the function if the started bool has been disabled
        if (!started) return;

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0)
        {
            NextWave();
        }

    }

    private IEnumerator WaitToSpawn()
    {
        yield return new WaitForSeconds(enemySpawnWaves[wave].spawnRate);
        SpawnEnemy(enemySpawnWaves[wave].amountToSpawn);

        StartCoroutine(WaitToSpawn());
    }

    public void GameOver()
    {
        gameOver = true;
        StopAllCoroutines();
    }

    void SpawnEnemy(int _amount)
    {
        if (!gameOver)
        {
            for (int i = 0; i < _amount; i++)
            {
                float angle = Random.Range(0, 2f * Mathf.PI);
                Vector3 pos = player.transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
                pos = new Vector3(pos.x, 0.5f, pos.z);

                var enemyToSpawn = enemySpawnWaves[wave].enemiesToSpawn[Random.Range(0, enemySpawnWaves[wave].enemiesToSpawn.Length)];

                if (poolingManager.CheckIfPoolFree(enemyToSpawn))
                {
                    GameObject enemyObj = poolingManager.SpawnObject(enemyToSpawn, pos, Quaternion.identity);
                    enemyObj.GetComponent<EnemyScript>().Init();
                    activeEnemies.Add(enemyObj);
                }
            }
        }
    }

    public void RemoveEnemy(GameObject _enemy)
    {
        activeEnemies.Remove(_enemy);
    }

    public List<GameObject> GetActiveEnemies()
    {
        return activeEnemies;
    }

    void NextWave()
    {
        if (wave + 1 <= enemySpawnWaves.Length)
            wave++;
        spawnTimer = enemySpawnWaves[wave].duration;
    }
}
