using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    [System.Serializable]
    public struct PoolingSystems
    {
        public PoolingEnum name;
        public GameObject gameObjectToSpawn;
        public int IntialToSpawn;
    }

    public PoolingSystems[] poolingSystems;
    List<GameObject> parentPools = new List<GameObject>();

    public enum PoolingEnum
    {
        Coins,
        XP,
        Bullet,
        Enemy1,
        Enemy2,
        Enemy3,
        Enemy4,
        Obstacles,
        LazerStrike,
        ChainLightning,
        GenericCube,
        DMGNum,
        LightningStrike
    }

    public void SpawnIntialObjects()
    {
        for (int i = 0; i < poolingSystems.Length; i++)
        {
            GameObject parent = new GameObject(poolingSystems[i].name.ToString() + "PoolSystem");
            parentPools.Add(parent);
            parentPools[i].transform.parent = transform;

            for (int j = 0; j < poolingSystems[i].IntialToSpawn; j++)
            {
                GameObject obj = Instantiate(poolingSystems[i].gameObjectToSpawn, new Vector3(0, 0, 0), Quaternion.identity);
                obj.transform.parent = parent.transform;
                obj.SetActive(false);
            }
        }

        //Setting the projectile object in the player script
        //PlayerScript player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        //if (player.projectile != null) return;

        //Debug.Log("Name: " + parentPools[(int)PoolingEnum.Bullet].transform.name);

        //player.projectile = FindFreeObject(parentPools[(int)PoolingEnum.Bullet].transform).gameObject;
    }

    public bool CheckIfPoolFree(PoolingEnum _poolingEnum)
    {
        return (FindFreeObject(parentPools[(int)_poolingEnum].transform) != null);
    }

    public GameObject SpawnObject(PoolingEnum _poolingEnum, Vector3 _position, Quaternion _rotation)
    {
        GameObject spawnObj = FindFreeObject(parentPools[(int)_poolingEnum].transform);

        if (spawnObj != null)
        {
            spawnObj.SetActive(true);
            //Sergio has had issues with how his particles are spawning through the ground so im doing this to lift them 
            Vector3 pos = _position;
            pos.y = 0.5f;
            spawnObj.transform.position = _position;
            spawnObj.transform.rotation = _rotation;
        }

        return spawnObj;
    }

    public void DespawnObject(GameObject _object)
    {
        _object.SetActive(false);
        _object.transform.position = new Vector3(0, 0, 0);
    }

    public IEnumerator DespawnObjectTimer(GameObject _object, float _time)
    {
        yield return new WaitForSeconds(_time);

        _object.SetActive(false);
        _object.transform.position = new Vector3(0, 0, 0);
    }

    GameObject FindFreeObject(Transform _parent)
    {
        foreach (Transform child in _parent)
        {
            if (!child.gameObject.activeSelf)
            {
                return child.gameObject;
            }
        }

        Debug.LogWarning($"Couldn't find a free object in {_parent.name} pool (try increasing the size)");
        return null;
    }

    //Check how many items in the pool is currently active
    public int GetPoolAmount(PoolingEnum _poolEnum)
    {
        return parentPools[(int)_poolEnum].transform.childCount;
    }

    /*PoolingSystems FindPoolingSystem(PoolingEnum _poolingEnum)
    {
        for(int i = 0; i < poolingSystems.Length; i++)
        {
            if (poolingSystems[i].name == _poolingEnum)
                return poolingSystems[i];
        }

        Debug.LogError("Couldn't find enum");
        return poolingSystems[0];
    }*/
}