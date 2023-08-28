using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCheck : MonoBehaviour
{
    public int tileNum = 0;
    public WorldChunker chunkScript;

    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Check for left mouse button click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the hit object is the one you're interested in
                if (hit.collider.gameObject == gameObject)
                {
                    Debug.Log("Object clicked: " + hit.collider.gameObject.name);
                    // Perform actions for when the object is clicked
                }
            }
        }
    }
}
