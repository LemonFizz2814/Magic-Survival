using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float radius;

    public int wave;
    public bool dontSpawn;

    int killedAmount;

    public PoolingManager poolingManager;

    bool started = false;
    bool gameOver = false;

    //List<GameObject> enemiesToSpawn = new List<GameObject>();

    GameObject player;

    [System.Serializable]
    public struct EnemySpawnWaves
    {
        public float killAmount;
        public int totalActivePerWave;
        public int groupSpawnSize;
        public PoolingManager.PoolingEnum[] enemiesToSpawn;
    }

    public EnemySpawnWaves[] enemySpawnWaves;
    List<GameObject> activeEnemies = new List<GameObject>();

    public void Awake()
    {
        gameOver = false;
    }

    public void StartPressed()
    {
        //Matthew: returning the function if the enemy is not supposed to spawn
        if (dontSpawn) return;

        player = GameObject.FindGameObjectWithTag("Player");
        started = true;
        SpawnEnemy(enemySpawnWaves[wave].groupSpawnSize);
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
        CheckKillAmount();
        activeEnemies.Remove(_enemy);
    }

    void CheckKillAmount()
    {
        killedAmount++;

        // spawn enemies in groups after a certain amount have been killed already
        if(killedAmount % enemySpawnWaves[wave].groupSpawnSize == 0)
        {
            Debug.Log($"SPAWNING, killedAmount {killedAmount}, groupSpawnSize {enemySpawnWaves[wave].groupSpawnSize}");
            do
            {
                SpawnEnemy(enemySpawnWaves[wave].groupSpawnSize);
            } while (activeEnemies.Count < enemySpawnWaves[wave].totalActivePerWave); // loop until total active per wave is reached
        }

        // once kill amount is reached then move to next wave
        if (killedAmount >= enemySpawnWaves[wave].killAmount)
        {
            Debug.Log($"NextWave");
            NextWave();
        }
    }

    public List<GameObject> GetActiveEnemies()
    {
        return activeEnemies;
    }

    void NextWave()
    {
        killedAmount = 0;
        if (wave + 1 < enemySpawnWaves.Length)
            wave++;

        SpawnEnemy(enemySpawnWaves[wave].groupSpawnSize);
    }
}
