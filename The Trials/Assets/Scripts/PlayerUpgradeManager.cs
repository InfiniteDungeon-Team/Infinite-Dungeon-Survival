using TMPro;
using UnityEngine;
using System.Collections;

public class PlayerUpgradeManager : MonoBehaviour
{
    [SerializeField] WaveManager waveManager;

    // Base Player Stats
    private int playerBaseHP = 1;
    private float playerBaseDamage = 2;
    private float playerBaseMoveSpeed = 8;
    private int playerBaseSpecialAttacks = 1;
    private int playerBaseMagazineSize = 10;
    private float playerBaseReloadSpeed = 3f;
    private int playerBaseHealAmount = 5;

    // Player Stat Multipliers
    private int playerHPMultiplier = 5;
    private float playerDamageMultiplier = 1.08f;
    private float playerMoveSpeedMultiplier = 1.01f;
    private int playerSpecialAttacksModifer = 1;
    private int playerMagazineSizeModifier = 1;
    private float playerReloadSpeedModifier = 0.04f;
    private int playerHealAmountModifier = 2;

    // Current Player Stats
    private int currentPlayerHP;
    private int currentPlayerMaxHP;
    private float currentPlayerDamage;
    private float currentPlayerMoveSpeed;
    private int currentPlayerSpecialAttacks;
    private int currentPlayerMagainzeSize;
    private float currentPlayerReloadSpeed;
    private int currentPlayerHealAmount;

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
        ResetPlayerStats();
    }

    // getters
    public float GetCurrentMoveSpeed() => currentPlayerMoveSpeed; // return current player move speed
    public float GetNextUpgradeMoveSpeed() => playerBaseMoveSpeed * playerMoveSpeedMultiplier; // return next upgrade move speed value

    public int GetCurrentHP() => currentPlayerMaxHP; // return current player HP
    public int GetNextUpgradeHP() => currentPlayerMaxHP + playerHPMultiplier; // return next upgrade max HP value

    public float GetCurrentDamage() => currentPlayerDamage; // return current player damage
    public float GetNextUpgradeDamage() => currentPlayerDamage * playerDamageMultiplier; // return next upgrade player damage

    public int GetCurrentPlayerSpecialAttacks() => currentPlayerSpecialAttacks; // return current number of special attacks
    public int GetNextUpgradePlayerSpecialAttacks() => currentPlayerSpecialAttacks + playerSpecialAttacksModifer;

    public int GetCurrentPlayerMagazineSize() => currentPlayerMagainzeSize; // return current magazine size
    public int GetNextUpgradePlayerMagazineSize() => currentPlayerMagainzeSize + playerMagazineSizeModifier;

    public float GetCurrentPlayerReloadSpeed() => currentPlayerReloadSpeed; // return current reload speed
    public float GetNextUpgradePlayerReloadSpeed() => currentPlayerReloadSpeed * (1 - playerReloadSpeedModifier);
    public int GetCurrentPlayerHealAmount() => currentPlayerHealAmount;
    public int GetNextPlayerHealAmount() => currentPlayerHealAmount + playerHealAmountModifier;

    // setters
    public void SetPlayerMaxHP()
    {
        currentPlayerMaxHP += playerHPMultiplier;
        SetPlayerHealthText();
    }

    public void SetPlayerCurrentHP(int modifier)
    {
        // only take damage if health > 0
        if (currentPlayerHP <= 0) return;

        int newHP = currentPlayerHP + modifier;

        // don't allow currentHP to be less than 0 or greater than MaxHP
        currentPlayerHP = Mathf.Clamp(newHP, 0, currentPlayerMaxHP);

        SetPlayerHealthText();

        if (currentPlayerHP <= 0)
        {
            SetIsDead(true);
        }
    }

    public void SetPlayerDamage()
    {
        currentPlayerDamage = playerBaseDamage * playerDamageMultiplier;
    }

    public void SetPlayerMoveSpeed()
    {
        currentPlayerMoveSpeed = playerBaseMoveSpeed * playerMoveSpeedMultiplier;
    }

    public void SetPlayerSpecialAttacks()
    {
        currentPlayerSpecialAttacks += playerSpecialAttacksModifer;
    }

    public void SetPlayerMagazineSize()
    {
        currentPlayerMagainzeSize += playerMagazineSizeModifier;
    }

    public void SetPlayerReloadSpeed()
    {
        currentPlayerReloadSpeed *= (1 - playerReloadSpeedModifier);
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

        // pulse when below threshold
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

    public void PrintPlayerUpgrades()
    {
        Debug.Log(
            $"HP: {currentPlayerHP}/{currentPlayerMaxHP} | " +
            $"DMG: {currentPlayerDamage} | " +
            $"SPD: {currentPlayerMoveSpeed} | " +
            $"SPEC: {currentPlayerSpecialAttacks} | " +
            $"MAG: {currentPlayerMagainzeSize} | " +
            $"RELOAD: {currentPlayerReloadSpeed}"
        );
    }

    public void ResetPlayerStats()
    {
        currentPlayerMaxHP = playerBaseHP;
        currentPlayerDamage = playerBaseDamage;
        currentPlayerMoveSpeed = playerBaseMoveSpeed;
        currentPlayerSpecialAttacks = playerBaseSpecialAttacks;
        currentPlayerMagainzeSize = playerBaseMagazineSize;
        currentPlayerReloadSpeed = playerBaseReloadSpeed;
        currentPlayerHP = currentPlayerMaxHP;
        SetPlayerCurrentHP(0);
        currentPlayerHealAmount = playerBaseHealAmount;
    }
}
