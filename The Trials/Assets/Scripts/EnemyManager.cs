using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] WaveManager waveManager;

    // Enemy Pool
    [SerializeField] GameObject enemyPool;

    // Enemy Spawn Locations
    [SerializeField] GameObject enemySpawn_N;
    [SerializeField] GameObject enemySpawn_E;
    [SerializeField] GameObject enemySpawn_S;
    [SerializeField] GameObject enemySpawn_W;

    // Base Enemy Stats
    private float baseEnemyHP = 10;
    private float baseEnemyDamage = 1;
    private float baseEnemyMoveSpeed = 1;


    // Enemy Stat Multipliers
    [SerializeField] private float enemyHPMultiplier = 1.08f;
    [SerializeField] private float enemyDamageMultiplier = 1.08f;
    [SerializeField] private float enemyMoveSpeedMultiplier = 1.01f;

    public float GetCurrentMaxHP()
    {
        return baseEnemyHP * Mathf.Pow(enemyHPMultiplier, waveManager.GetCurrentWaveID() - 1);
    }

    public float GetCurrentDamage()
    {
        return baseEnemyDamage * Mathf.Pow(enemyDamageMultiplier, waveManager.GetCurrentWaveID() - 1);
    }

    public float GetCurrentMoveSpeed()
    {
        return baseEnemyMoveSpeed * Mathf.Pow(enemyMoveSpeedMultiplier, waveManager.GetCurrentWaveID() - 1);
    }
}
