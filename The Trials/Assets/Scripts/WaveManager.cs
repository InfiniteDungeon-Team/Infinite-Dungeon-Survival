using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] PlayerUpgradeManager playerUpgradeManager;

    [SerializeField] private List<GameObject> activeEnemiesList = new List<GameObject>();

    // Other Wave Related Things
    [SerializeField] WaveCountdownUI waveCountdownUI;
    [SerializeField] private int currentWaveID = 1;
    public float waveDuration { get; private set; } = 20f; // the duration of a single wave
    private float baseMin = 1f; // base minimum spawn time between enemy spawns
    private float baseMax = 2f; // base maximum spawn time between enemy spawns
    private float decay = 0.92f; // 8% faster each wave
    private float min; // current minimum spawn time between enemy spawns
    private float max; // current maximum spawn time between enemy spawns
    public bool WaveIsActive { get; private set; }



    // Player Stuff
    [SerializeField] PlayerController playerController;

    // Enemy Pool Stuff
    [SerializeField] GameObject enemyPoolGO;
    private int currentEnemyInPool = 0;

    // Enemy Spawn Locations
    [SerializeField] Transform enemySpawn_N;
    [SerializeField] Transform enemySpawn_E;
    [SerializeField] Transform enemySpawn_S;
    [SerializeField] Transform enemySpawn_W;
    [SerializeField] GameObject[] enemySpawnLocations;

    // UI Stuff
    [SerializeField] private TMP_Text waveNumTMP;
    [SerializeField] private TMP_Text wavetimerTMP;

    // Wave Timing
    private Coroutine waveRoutine;
    private Coroutine timerRoutine;

    private void Start()
    {
        SetNeutralWaveUI();
        InitiateWaveStart();
    }

    public void InitiateWaveStart()
    {
        // ****** THIS IS WHAT OFFICIALLY STARTS A WAVE *******
        waveCountdownUI.PlayWaveCountdown();
    }

    public void StartWave(float duration)
    {
        SetWaveIsActive(true);

        // Clear the active enemies list completely for next wave
        activeEnemiesList.Clear();

        // Get the player ready to move again
        playerController.WaveStartBehaviors();

        // Stop any previous wave or timer if they’re still running
        if (waveRoutine != null) StopCoroutine(waveRoutine);
        if (timerRoutine != null) StopCoroutine(timerRoutine);

        SetMinAndMax(GetCurrentWaveID());

        waveRoutine = StartCoroutine(Wave(duration));
        timerRoutine = StartCoroutine(RunWaveUIElements(duration));
    }

    IEnumerator Wave(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (currentEnemyInPool >= enemyPoolGO.transform.childCount)
                currentEnemyInPool = 0;

            // Assign the next enemy from the pool 
            GameObject currentEnemyGO = enemyPoolGO.transform.GetChild(currentEnemyInPool).gameObject; // get the next enemy in the pool

            // Set its GameObject to active
            currentEnemyGO.SetActive(true); // set the enemy to active

            // Set it's spawnedAndActiveState to true
            currentEnemyGO.GetComponent<Enemy>().SetIsSpawnedAndActive(true);

            // pick a random spawn location (include all spawn points!)
            currentEnemyGO.transform.position = enemySpawnLocations[Random.Range(0, enemySpawnLocations.Length)].transform.position;

            currentEnemyGO.GetComponent<Enemy>().enabled = false; // disable its main enemy script
            currentEnemyGO.GetComponent<EnemyMoveToArena>().enabled = true; // enable the script to enter the arena

            currentEnemyInPool++;

            // pick a random interval for the next spawn
            float wait = Random.Range(min, max);

            // don’t overshoot the wave duration
            if (elapsed + wait > duration)
                wait = duration - elapsed;

            // wait before next spawn
            yield return new WaitForSeconds(wait);
            elapsed += wait;
        }

        Debug.Log("Wave finished!");

        // "wave complete" logic here
        EndWave();
    }

    public void EndWave()
    {
        IncrementWaveID();

        // Set end of wave behaviors on player
        playerController.WaveStopBehaviors();

        // Find all active enemies in arena and add to a list
        for (int i = 0; i < enemyPoolGO.transform.childCount; i++)
        {
            GameObject currentScannedEnemy = enemyPoolGO.transform.GetChild(i).gameObject;
            if (currentScannedEnemy.GetComponent<Enemy>().GetSpawnedAndActiveState())
            {
                activeEnemiesList.Add(currentScannedEnemy);
            }
        }

        // Stop enemy behaviors on all enemies
        for (int i = 0; i < activeEnemiesList.Count; i++)
        {
            activeEnemiesList[i].GetComponent<Enemy>().SetIsSpawnedAndActive(false);
        }

        // Kill the enemies one at a time with slight pause between each
        for (int i = 0; i < activeEnemiesList.Count; i++)
        {
            StartCoroutine(KillEnemiesSequentially());
        }

        SetWaveIsActive(false);
    }

    private IEnumerator KillEnemiesSequentially()
    {
        // to create a brief pause at the end of the wave before killing off enemies
        yield return new WaitForSeconds(1.50f);

        // copy to array in case the list changes when enemies die
        var enemiesToKill = new List<GameObject>(activeEnemiesList);

        for (int i = 0; i < enemiesToKill.Count; i++)
        {
            Enemy enemy = enemiesToKill[i].GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.EnemyDeath();
            }

            // Wait before killing the next one
            yield return new WaitForSeconds(0.40f);
        }
    }

    IEnumerator RunWaveUIElements(float duration)
    {
            SetWaveNumberUI(GetCurrentWaveID());
            float timeRemaining = duration;

            while (timeRemaining > 0f)
            {
                // Update the timer UI
                wavetimerTMP.text = Mathf.Ceil(timeRemaining).ToString("0");

                timeRemaining -= Time.deltaTime;
                yield return null; // wait for next frame
            }

        // Make sure we show 0 at the end
        wavetimerTMP.text = "0";

        Debug.Log("Wave timer finished!");
    }

    private void SetMinAndMax(int waveNumber)
    {
        min = baseMin * Mathf.Pow(decay, waveNumber - 1);
        max = baseMax * Mathf.Pow(decay, waveNumber - 1);

        min = Mathf.Clamp(min, 0.25f, baseMin);
        max = Mathf.Clamp(max, 0.50f, baseMax);
    }

    public void SetWaveTimerUI(float duration)
    {
        wavetimerTMP.text = "0";
    }

    public void SetWaveNumberUI(int waveNumber)
    {
        if(waveNumber < 10)
            waveNumTMP.text = ($"WAVE: 0{waveNumber.ToString()}");
        else
            waveNumTMP.text = ($"WAVE: {waveNumber.ToString()}");
    }
    public int GetCurrentWaveID()
    {
        return currentWaveID;
    }

    public void IncrementWaveID()
    {
        currentWaveID++;
    }

    private void SetNeutralWaveUI()
    {
        wavetimerTMP.text = "--";
        waveNumTMP.text = ($"WAVE: --");
    }

    private void SetWaveIsActive(bool state)
    {
        WaveIsActive = state;
    }
}
