using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rb;
    Transform target;
    Vector2 moveDirection;

    // When true, the enemy is active in the level and moving/attacking the player
    [SerializeField] private bool spawnedAndActive = false;

    // Managers
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private PlayerUpgradeManager playerUpgradeManager;

    // Current Enemy Stats
    [SerializeField] private float currentEnemyHP;
    [SerializeField] private float currentEnemyDamage;
    [SerializeField] private float currentEnemyMoveSpeed;

    // Health bar
    [SerializeField] private Slider enemyHealthBarSlider;

    // Use a different name so we don't hide Component.camera
    [SerializeField] private Camera enemyCamera;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ResetEnemyStats();
    }

    private void Start()
    {
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Enemy: Could not find Player object in the scene.");
        }
    }

    private void Update()
    {
        if (target && spawnedAndActive)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            moveDirection = direction;

            // Make the enemy's "up" direction point at the player
            transform.up = -moveDirection;
        }
    }

    private void FixedUpdate()
    {
        if (target && spawnedAndActive)
        {
            rb.linearVelocity = new Vector2(moveDirection.x, moveDirection.y) * enemyManager.GetCurrentMoveSpeed();
        }
    }

    private void LateUpdate()
    {
        if (target && spawnedAndActive && enemyHealthBarSlider != null)
        {
            // Force the health bar to maintain world rotation of zero
            enemyHealthBarSlider.transform.rotation = Quaternion.identity;

            // Calculate world position offset without parent rotation influence
            Vector3 worldOffset = new Vector3(0, 1, 0); // 1 unit up in world space
            enemyHealthBarSlider.transform.position = transform.position + worldOffset;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.name.Contains("bullet") && spawnedAndActive)
        {
            if (currentEnemyHP > 0)
            {
                TakeDamage();
            }
        }
    }

    public void SetIsSpawnedAndActive(bool _bool)
    {
        spawnedAndActive = _bool;
    }

    public void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;
    }

    // When an enemy is set active and ready to attack player, update its stats to match the current wave
    private void ResetEnemyStats()
    {
        currentEnemyHP = enemyManager.GetCurrentMaxHP();
        currentEnemyDamage = enemyManager.GetCurrentDamage();
        currentEnemyMoveSpeed = enemyManager.GetCurrentMoveSpeed();
        UpdateHealthBar(currentEnemyHP, enemyManager.GetCurrentMaxHP());
    }

    private void TakeDamage()
    {
        currentEnemyHP -= playerUpgradeManager.GetCurrentDamage();
        Debug.Log($"Ow! You shot me. I have {currentEnemyHP} / {enemyManager.GetCurrentMaxHP()} HP remaining!");

        if (currentEnemyHP <= 0)
        {
            EnemyDeath();
        }

        UpdateHealthBar(currentEnemyHP, enemyManager.GetCurrentMaxHP());
    }

    private void EnemyDeath()
    {
        StopMovement();
        SetIsSpawnedAndActive(false);
        transform.position = new Vector2(0, -9999); // when hit, move the enemy out of sight of player
        StartCoroutine(RespawnCoolDown());
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (enemyHealthBarSlider != null && maxHealth > 0)
        {
            enemyHealthBarSlider.value = currentHealth / maxHealth;
        }
    }

    private IEnumerator RespawnCoolDown()
    {
        yield return new WaitForSeconds(Random.Range(1f, 3f));

        Vector2 respawnLocation = new Vector2(Random.Range(-6.5f, 6.5f), Random.Range(-6.5f, 6.5f)); // respawn somewhere in the arena
        transform.position = respawnLocation;
        ResetEnemyStats();
        SetIsSpawnedAndActive(true);
    }
}
