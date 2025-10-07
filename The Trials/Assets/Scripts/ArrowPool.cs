using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPool : MonoBehaviour
{
    [SerializeField] GameObject arrowPrefab;
    
    public List<GameObject> arrowPool;

    private int maxArrows = 50;

    void Start()
    {
        for (int i = 0; i < maxArrows; i++)
        {
            GameObject newObject = Instantiate(arrowPrefab, transform); // instatiate arrow prefabs at start as children of the ArrowPool
            arrowPool.Add(newObject);
        }
    }
}
