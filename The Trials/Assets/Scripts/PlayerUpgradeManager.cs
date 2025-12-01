using TMPro;
using UnityEngine;
using System.Collections;

public class PlayerUpgradeManager : MonoBehaviour
{
    [SerializeField] WaveManager waveManager;

    // Base Player Stats
    [SerializeField] private float playerBaseHP = 50;
    [SerializeField] private float playerBaseDamage = 2;
    [SerializeField] private float playerBaseMoveSpeed = 10;

    // Player Stat Multipliers
    [SerializeField] private float playerHPMultiplier = 1.08f;
    [SerializeField] private float playerDamageMultiplier = 1.08f;
    [SerializeField] private float playerMoveSpeedMultiplier = 1.01f;

    // Current Player Stats
    [SerializeField] private int currentPlayerHP;
    [SerializeField] private int currentPlayerMaxHP;
    [SerializeField] private float currentPlayerDamage;
    [SerializeField] private float currentPlayerMoveSpeed;

    // Player Health Stuff
    [SerializeField] private TMP_Text playerHealthText;
    [SerializeField] private Color highHealthColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private float lowHealthThreshold = 0.25f;
    [SerializeField] private float pulseSpeed = 6f; // speed of text fade
    private bool isDead = false;

    private Coroutine pulseRoutine;
    private bool isPulsing = false;

    // On initial game load this will default to base values
    private void Awake()
    {
        SetPlayerMaxHP();
        SetPlayerDamage();
        SetPlayerMoveSpeed();

        currentPlayerHP = currentPlayerMaxHP;
        SetPlayerCurrentHP(0);
    }

    public float GetCurrentMoveSpeed() => currentPlayerMoveSpeed; // Return current player move speed
    public int GetCurrentHP() => currentPlayerHP; // Return current player HP
    public float GetCurrentDamage() => currentPlayerDamage; // Return current player damage

    public void SetPlayerMaxHP()
    {
        currentPlayerMaxHP = Mathf.RoundToInt(playerBaseHP * Mathf.Pow(playerHPMultiplier, waveManager.GetCurrentWaveID() - 1));
    }

    public void SetPlayerCurrentHP(int modifier)
    {
        // only take damage if health > 0
        if (currentPlayerHP <= 0) return;

        int newHP = currentPlayerHP + modifier; // modifier can be negative or positive
        newHP = Mathf.Clamp(newHP, 0, currentPlayerMaxHP);
        currentPlayerHP = newHP;

        // Also update the player health HUD text
        SetPlayerHealthText();

        // After taking damage, if health <= 0 we want to set death condition
        if (currentPlayerHP <= 0)
        {
            SetIsDead(true);
        }
    }

    public void SetPlayerDamage()
    {
        currentPlayerDamage = playerBaseDamage * Mathf.Pow(playerDamageMultiplier, waveManager.GetCurrentWaveID() - 1);
    }

    public void SetPlayerMoveSpeed()
    {
        currentPlayerMoveSpeed = playerBaseMoveSpeed * Mathf.Pow(playerMoveSpeedMultiplier, waveManager.GetCurrentWaveID() - 1);
    }

    public void SetPlayerHealthText()
    {
        // Set the text
        playerHealthText.text = currentPlayerHP.ToString() + " / " + currentPlayerMaxHP.ToString();

        // Calculate health %
        float healthPercent;
        if (currentPlayerMaxHP > 0)
        {
            healthPercent = (float)currentPlayerHP / currentPlayerMaxHP;
        }
        else
        {
            healthPercent = 0f;
        }

        healthPercent = Mathf.Clamp01(healthPercent);

        // If we are not in pulse mode, use the normal green-to-red gradient
        if (!isPulsing)
        {
            playerHealthText.color = Color.Lerp(lowHealthColor, highHealthColor, healthPercent);
        }

        // Pulse (color fade) when below threshold
        if (healthPercent <= lowHealthThreshold)
        {
            if (!isPulsing)
            {
                // start pulsing
                if (pulseRoutine != null)
                    StopCoroutine(pulseRoutine);

                pulseRoutine = StartCoroutine(PulseHealthText());
                isPulsing = true;
            }
        }
        else
        {
            // stop pulsing and reset to normal gradient color
            if (isPulsing)
            {
                if (pulseRoutine != null)
                    StopCoroutine(pulseRoutine);

                playerHealthText.transform.localScale = Vector3.one; // just in case
                isPulsing = false;

                // Restore non-pulsing color based on current health
                playerHealthText.color = Color.Lerp(lowHealthColor, highHealthColor, healthPercent);
            }
        }
    }

    private IEnumerator PulseHealthText()
    {
        float t = 0f;

        while (true)
        {
            t += Time.deltaTime * pulseSpeed;

            // s goes 0 to 1 to 0 with a sine wave; we use it to lerp between white and red
            float s = Mathf.Sin(t) * 0.5f + 0.5f; // remap from [-1, 1] to [0, 1]

            // Fade between white and lowHealthColor (usually red)
            Color pulseColor = Color.Lerp(Color.white, lowHealthColor, s);
            playerHealthText.color = pulseColor;

            yield return null;
        }
    }

    public void SetIsDead(bool _isDead)
    {
        isDead = _isDead;
    }
    public bool GetIsDead()
    {
        return isDead;
    }
}
