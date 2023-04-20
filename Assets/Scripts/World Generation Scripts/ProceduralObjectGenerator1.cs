using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralObjectGenerator1 : MonoBehaviour
{
    public GameObject ObjecttoSpawn;
    public PoolingManager poolingManager;

    public int num_points;
    public float radius;
    public float DistancetoSpawn;

    [Header("Debugging Purpose Only")]
    public int Seed;

    Vector3 pointzero = new Vector3();
    List<Vector3> playerspawn = new List<Vector3>();

    private void Update()
    {
        //Is the player x distance away from old position
        if (Vector3.Distance(pointzero, this.transform.position) > DistancetoSpawn)
        {
            //Current point
            pointzero = this.transform.position;

            //To find point closet to player
            float leastdistancetoplayer = float.MaxValue;
            Vector3 closestpointtoplayer = new Vector3();
            for (int i = 0; i < playerspawn.Count; i++)
            {
                if (Vector3.Distance(pointzero, playerspawn[i]) < leastdistancetoplayer)
                {
                    leastdistancetoplayer = Vector3.Distance(pointzero, playerspawn[i]);
                    closestpointtoplayer = playerspawn[i];
                }
            }

            //Now that we have the point closest to the player
            //Is that point greater than x
            //If it is we are going to spawn new objects
            //Otherwise we will spawn objects at old locations
            if (leastdistancetoplayer > DistancetoSpawn)
            {
                playerspawn.Add(pointzero);

                float Xmin = (pointzero.x - radius);
                float Xmax = (pointzero.x + radius);
                                                      
                float Ymin = (pointzero.z - radius);
                float Ymax = (pointzero.z + radius);

                //Just a linear Function
                int customseed = (int)((Seed * (Xmin + Xmax)) + (Seed * (Ymin + Ymax)) + Mathf.Pow(Seed, 2.0f));
                System.Random _rnd = new System.Random(customseed);

                for (int i = 0; i < num_points; i++)
                {
                    Vector3 loc = new Vector3(_rnd.Next((int)Xmin, (int)Xmax), this.transform.position.y, _rnd.Next((int)Ymin, (int)Ymax));

                    SpawnObj(loc);
                }
            }
            else
            {
                float Xmin = (closestpointtoplayer.x - radius);
                float Xmax = (closestpointtoplayer.x + radius);
                                                                 
                float Ymin = (closestpointtoplayer.z - radius);
                float Ymax = (closestpointtoplayer.z + radius);

                //Just a linear Function
                int customseed = (int)((Seed * (Xmin + Xmax)) + (Seed * (Ymin + Ymax)) + Mathf.Pow(Seed, 2.0f));
                System.Random _rnd = new System.Random(customseed);

                for (int i = 0; i < num_points; i++)
                {
                    Vector3 loc = new Vector3(_rnd.Next((int)Xmin, (int)Xmax), this.transform.position.y, _rnd.Next((int)Ymin, (int)Ymax));

                    SpawnObj(loc);
                }
            }
        }
    }

    void SpawnObj(Vector3 _pos)
    {
        if (poolingManager.CheckIfPoolFree(PoolingManager.PoolingEnum.GenericCube))
        {
            poolingManager.SpawnObject(PoolingManager.PoolingEnum.GenericCube, _pos, Quaternion.Euler(0, 45, 0));
        }
    }
}