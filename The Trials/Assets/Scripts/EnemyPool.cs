using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{

    public List<GameObject> enemyPoolList;

    private int enemiesToSpawn = 50;

    void Start()
    {
        //for (int i = 0; i < enemiesToSpawn; i++)
        //{
        //    GameObject newObject = Instantiate(enemyPrefab, transform); // instatiate arrow prefabs at start as children of the ArrowPool
        //    enemyPoolList.Add(newObject);
        //}
    }
}