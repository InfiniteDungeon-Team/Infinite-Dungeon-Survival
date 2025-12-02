using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] WaveManager waveManager;

    // Enemy Pool
    [SerializeField] GameObject enemyPool;

    // Base Enemy Stats
    private float baseEnemyHP = 10;
    private int baseEnemyDamage = 1;
    private float baseEnemyMoveSpeed = 2;


    // Enemy Stat Multipliers
    [SerializeField] private float enemyHPMultiplier = 1.02f;
    [SerializeField] private float enemyDamageMultiplier = 1.03f;
    [SerializeField] private float enemyMoveSpeedMultiplier = 1.01f;

    public float GetCurrentMaxHP()
    {
        return baseEnemyHP * Mathf.Pow(enemyHPMultiplier, waveManager.GetCurrentWaveID() - 1);
    }

    public int GetCurrentDamage()
    {
        // Scale base damage and round the final result to int
        float scaledDamage = baseEnemyDamage * Mathf.Pow(enemyDamageMultiplier, waveManager.GetCurrentWaveID() - 1);
        return Mathf.RoundToInt(scaledDamage);
    }

    public float GetCurrentMoveSpeed()
    {
        return baseEnemyMoveSpeed * Mathf.Pow(enemyMoveSpeedMultiplier, waveManager.GetCurrentWaveID() - 1);
    }
}
