using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSouls : MonoBehaviour
{
   
    public GameObject souls = null;
    public List<Transform> spawnPoints;

    void Start()
    {   
        if (spawnPoints.Count != 0) 
        {
            Spawn(); 
        }
    }

    void Spawn()
    {
         for ( int i = 0; i < 8; i++) 
        { 
            var spawn = Random.Range(0, spawnPoints.Count);
            if (spawnPoints.Count == 0) 
            {
                break;
            }
            Instantiate(souls, spawnPoints[spawn].transform.position, Quaternion.identity);
            spawnPoints.RemoveAt(spawn);
            
        }
    }
}
