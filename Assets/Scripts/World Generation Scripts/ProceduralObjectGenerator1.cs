using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProceduralObjectGenerator1 : MonoBehaviour
{
    public GameObject ObjecttoSpawn;
    public PoolingManager poolingManager;
    [SerializeField] private List<GameObject> activeObjects;

    public int num_points;
    public float radius;
    public float DistancetoSpawn;

    [Header("Debugging Purpose Only")]
    public int Seed;
    public bool isSpawn = false;

    Vector3 pointzero = new Vector3();
    List<Vector3> playerspawn = new List<Vector3>();

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "Main Game")
        {
            poolingManager.SpawnIntialObjects();
        }

        playerspawn.Add(this.transform.position);
    }

    private void Update()
    {
        float asd = float.MaxValue;
        for (int i = 0; i < playerspawn.Count; i++)
        {
            if (Vector3.Distance(this.transform.position, playerspawn[i]) < asd)
            {
                asd = Vector3.Distance(this.transform.position, playerspawn[i]);
            }
        }

        //Is the player x distance away from old position
        if (asd > DistancetoSpawn)
        {
            //Current point
            pointzero = this.transform.position;

            playerspawn.Add(pointzero);

            float Xmin = (pointzero.x - radius);
            float Xmax = (pointzero.x + radius);

            float Ymin = (pointzero.z - radius);
            float Ymax = (pointzero.z + radius);

            //Just a linear Function
            int customseed = (int)((Seed * (Xmin + Xmax)) + (Seed * (Ymin + Ymax)) + Mathf.Pow(Seed, 2.0f));
            System.Random _rnd = new System.Random(customseed);

            DespawnPrevObj();
            for (int i = 0; i < num_points; i++)
            {
                Vector3 loc = new Vector3(_rnd.Next((int)Xmin, (int)Xmax), this.transform.position.y, _rnd.Next((int)Ymin, (int)Ymax));

                SpawnObj(loc);
            }
        }
        else
        {
            //To find point closet to player
            float leastdistancetoplayer = DistancetoSpawn;
            Vector3 closestpointtoplayer = new Vector3();
            for (int i = 0; i < playerspawn.Count; i++)
            {
                if (Vector3.Distance(pointzero, playerspawn[i]) < leastdistancetoplayer)
                {
                    leastdistancetoplayer = Vector3.Distance(pointzero, playerspawn[i]);
                    closestpointtoplayer = playerspawn[i];
                }
            }

            float Xmin = (closestpointtoplayer.x - radius);
            float Xmax = (closestpointtoplayer.x + radius);

            float Ymin = (closestpointtoplayer.z - radius);
            float Ymax = (closestpointtoplayer.z + radius);

            //Just a linear Function
            int customseed = (int)((Seed * (Xmin + Xmax)) + (Seed * (Ymin + Ymax)) + Mathf.Pow(Seed, 2.0f));
            System.Random _rnd = new System.Random(customseed);

            DespawnPrevObj();
            for (int i = 0; i < num_points; i++)
            {
                Vector3 loc = new Vector3(_rnd.Next((int)Xmin, (int)Xmax), this.transform.position.y, _rnd.Next((int)Ymin, (int)Ymax));

                SpawnObj(loc);
            }
        }
    }

    void SpawnObj(Vector3 _pos)
    {
        if (poolingManager.CheckIfPoolFree(PoolingManager.PoolingEnum.GenericCube))
        {
            GameObject newObject = poolingManager.SpawnObject(PoolingManager.PoolingEnum.GenericCube, _pos, Quaternion.Euler(0, 45, 0));
            activeObjects.Add(newObject);
        }
    }

    //Despawn the objects at the previous point
    void DespawnPrevObj()
    {
        if (activeObjects.Count == 0) return;

        for (int i = 0; i < activeObjects.Count; i++)
        {
            poolingManager.DespawnObject(activeObjects[i]);
        }

        activeObjects.Clear();
    }
}