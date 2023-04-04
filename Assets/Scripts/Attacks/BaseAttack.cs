using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttack : MonoBehaviour
{
    //Maybe create a serializable script that hosts max/min speed, damage, fire rate, size, range, crit chance/dmg
    //Everything else can be a value from here as they're unique values that are likely not going to be used
    //anywhere else

    [Header("Object values")]
    [SerializeField] private float spawnTimer = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public virtual void Spawn
}
