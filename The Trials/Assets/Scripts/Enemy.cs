using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rb;
    Transform target;
    Vector2 moveDirection;
    [SerializeField] Animator animator;

    private bool stoppedOnHit = false;

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

    [SerializeField] WaveManager waveManager;

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
        // only move to play if they're alive
        if (playerUpgradeManager.GetIsDead() == true)
        {
            SetIsSpawnedAndActive(false);
            return;
        }
        
        
        if (target && spawnedAndActive)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            moveDirection = direction;

            // make the enemy's "up" direction point at the player
            transform.up = -moveDirection;
        }
    }

    private void FixedUpdate()
    {
        // If the enemy is not active in the arena, stop their movement
        if (!spawnedAndActive)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // if it has a target to move towards, apply the movement
        if (target && !stoppedOnHit)
        {
            rb.linearVelocity = new Vector2(moveDirection.x, moveDirection.y) * enemyManager.GetCurrentMoveSpeed();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
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
                if (!stoppedOnHit && this.isActiveAndEnabled)
                {
                    stoppedOnHit = true;
                    StartCoroutine(StopOnHit());
                }
            }
        }
    }

    public void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;
    }

    // When an enemy is set active and ready to attack player, update its stats to match the current wave
    public void ResetEnemyStats()
    {
        currentEnemyHP = enemyManager.GetCurrentMaxHP();
        currentEnemyDamage = enemyManager.GetCurrentDamage();
        currentEnemyMoveSpeed = enemyManager.GetCurrentMoveSpeed();
    }

    private void TakeDamage()
    {
        currentEnemyHP -= playerUpgradeManager.GetCurrentDamage();
        //Debug.Log($"{this.gameObject.name}: I took {playerUpgradeManager.GetCurrentDamage()} damage. I have {this.currentEnemyHP} / {enemyManager.GetCurrentMaxHP()} HP.");
        if (currentEnemyHP <= 0)
        {
            EnemyDeath();
        }

        UpdateHealthBar(currentEnemyHP, enemyManager.GetCurrentMaxHP());
    }

    public void EnemyDeath()
    {
        StopMovement();
        SetIsSpawnedAndActive(false);
        animator.SetTrigger("Death");
    }

    public void OnDeathAnimationComplete()
    {
        transform.position = new Vector2(0, -9999); // when killed, move the enemy out of sight of player
        this.gameObject.SetActive(false); // disable the gameobject
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (enemyHealthBarSlider != null && maxHealth > 0)
        {
            enemyHealthBarSlider.value = currentHealth / maxHealth;
        }
    }
    private IEnumerator StopOnHit()
    {
        StopMovement();
        yield return new WaitForSeconds(0.10f);
        stoppedOnHit = false;
    }

    public bool GetSpawnedAndActiveState()
    {
        return spawnedAndActive;
    }
    public void SetIsSpawnedAndActive(bool _bool)
    {
        spawnedAndActive = _bool;
    }
}
