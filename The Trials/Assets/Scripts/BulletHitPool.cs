using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEngine;

public class BulletHitPool : MonoBehaviour
{
    [SerializeField] GameObject bulletHitWallFX_prefab;

    public List<GameObject> bulletHitPrefabList;

    private int maxFXs = 50;

    void Start()
    {
        for (int i = 0; i < maxFXs; i++)
        {
            GameObject newObject = Instantiate(bulletHitWallFX_prefab, transform); // instatiate arrow prefabs at start as children of the ArrowPool
            bulletHitPrefabList.Add(newObject);
        }
    }

    // Have an arrow call
}
