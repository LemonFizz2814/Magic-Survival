using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public float spawnRate;
    public float radius;
    public int maxSpawn;

    public GameObject coinObj;
    public PoolingManager poolingManager;

    GameObject player;

    public void StartPressed()
    {
        StartCoroutine(WaitToSpawn());
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private IEnumerator WaitToSpawn()
    {
        yield return new WaitForSeconds(spawnRate);
        if (FindObjectsOfType(typeof(CoinScript)).Length <= maxSpawn)
        {
            SpawnCoin();
        }        

        StartCoroutine(WaitToSpawn());
    }

    void SpawnCoin()
    {
        float angle;
        Vector3 pos;
        bool foundSpot = false;
        int safetyCheck = 0;

        do
        {
            safetyCheck++;
            angle = Random.Range(0, 2f * Mathf.PI);
            pos = player.transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;

            RaycastHit hit;

            if (Physics.Raycast(pos, -transform.up, out hit, 2))
            {
                if(hit.transform.CompareTag("Ground"))
                {
                    foundSpot = true;
                }
            }
        } while (!foundSpot && safetyCheck < 100);

        if (poolingManager.CheckIfPoolFree(PoolingManager.PoolingEnum.Coins))
        {
            GameObject coinObj = poolingManager.SpawnObject(PoolingManager.PoolingEnum.Coins, pos, Quaternion.Euler(0, 45, 0));
            coinObj.transform.position = new Vector3(coinObj.transform.position.x, 1, coinObj.transform.position.z);
            coinObj.GetComponent<CoinScript>().Init();
        }
    }
}
