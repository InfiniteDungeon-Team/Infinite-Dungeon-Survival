using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PlayerUpgradeManager : MonoBehaviour
{
    [SerializeField] WaveManager waveManager;

    // Base Player Stats
    [SerializeField] private float playerBaseHP = 10;
    [SerializeField] private float playerBaseDamage = 2;
    [SerializeField] private float playerBaseMoveSpeed = 10;

    // Player Stat Multipliers
    [SerializeField] private float playerHPMultiplier = 1.08f;
    [SerializeField] private float playerDamageMultiplier = 1.08f;
    [SerializeField] private float playerMoveSpeedMultiplier = 1.01f;

    // Current Player Stats
    [SerializeField] private float currentPlayerHP;
    [SerializeField] private float currentPlayerDamage;
    [SerializeField] private float currentPlayerMoveSpeed;

    // On initial game load this will default to base values
    private void Awake()
    {
        SetPlayerMaxHP();
        SetPlayerDamage();
        SetPlayerMoveSpeed();
    }

    public float GetCurrentMoveSpeed() => currentPlayerMoveSpeed; // Return current player move speed
    public float GetCurrentHP() => currentPlayerHP; // Return current player HP
    public float GetCurrentDamage() => currentPlayerDamage; // Return current player damage

    public void SetPlayerMaxHP()
    {
        currentPlayerHP = playerBaseHP * Mathf.Pow(playerHPMultiplier, waveManager.GetCurrentWaveID() - 1);
    }

    public void SetPlayerDamage()
    {
        currentPlayerDamage = playerBaseDamage * Mathf.Pow(playerDamageMultiplier, waveManager.GetCurrentWaveID() - 1);
    }

    public void SetPlayerMoveSpeed()
    {
        currentPlayerMoveSpeed = playerBaseMoveSpeed * Mathf.Pow(playerMoveSpeedMultiplier, waveManager.GetCurrentWaveID() - 1);
        //Debug.Log($"Wave: {waveManager.GetCurrentWaveID()}, Base Speed: {playerBaseMoveSpeed}, Current Speed: {currentPlayerMoveSpeed}");
    }
}
