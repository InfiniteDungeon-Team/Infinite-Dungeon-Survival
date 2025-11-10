using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;

    public List<GameObject> enemyPool;

    private int maxEnemies = 50;

    void Start()
    {
        for (int i = 0; i < maxEnemies; i++)
        {
            GameObject newObject = Instantiate(enemyPrefab, transform); // Instantiate Enemy Prefabs
            enemyPool.Add(newObject);
        }
    }
}
