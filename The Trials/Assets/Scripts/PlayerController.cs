using System.Collections;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] PlayerUpgradeManager playerUpgradeManager;
    [SerializeField] EnemyManager enemyManager;
    [SerializeField] WaveManager waveManager;

    // Shooting stuff
    [SerializeField] private GameObject arrowPoolGO;
    [SerializeField] private float timeBetweenFiring = 0.15f;
    [SerializeField] private float arrowFiringForce = 15f;
    [SerializeField] private Transform gun1_transform;
    [SerializeField] private Transform gun2_transform;
    private Camera mainCam;
    private Vector3 mousePos;
    private bool canFire = true;
    private float timer;
    private int currentArrowNum = 0;
    private GameObject currentArrowGO;
    private int totalNumArrows;


    // Crosshair stuff
    [SerializeField] private Transform crosshairTransform;
    [SerializeField] private float rotationOffset = 90f;

    // Movement variables
    private Vector2 input;
    [SerializeField] private bool canMove = true;

    // Take Damage
    private bool inIFrames = false;

    private void Awake()
    {
        mainCam = Camera.main;

        // If crosshair not assigned, try to find it as child
        if (crosshairTransform == null)
        {
            crosshairTransform = transform.GetChild(0);
        }
    }

    void Update()
    {
        if (!canMove) return;

        // Handle movement input
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input.Normalize();

        // Handle rotation based on mouse position
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotation = mousePos - transform.position;
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        // Rotate the entire player object (including sprite and crosshair)
        transform.rotation = Quaternion.Euler(0, 0, rotZ + rotationOffset);

        // Counter-rotate the crosshair to keep it upright
        if (crosshairTransform != null)
        {
            crosshairTransform.rotation = Quaternion.identity;
        }

        // Handle shooting
        HandleShooting(rotation);
    }

    private void FixedUpdate()
    {
        // Apply movement
        if(canMove)
            rb.linearVelocity = input * playerUpgradeManager.GetCurrentMoveSpeed();
        else
            StopMovement();
    }

    private void HandleShooting(Vector3 rotation)
    {
        // Update firing timer
        if (!canFire)
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenFiring)
            {
                SetCanFire(true);
                timer = 0;
            }
        }

        // Fire arrows on mouse click
        if (Input.GetMouseButtonDown(0) && canFire)
        {
            SetCanFire(false);

            // Play firing animation
            animator.SetTrigger("Shoot");

            // Get total arrows from pool
            totalNumArrows = arrowPoolGO.transform.childCount;

            // Fire from gun1
            FireArrowFromTransform(gun1_transform, rotation);

            // Fire from gun2
            FireArrowFromTransform(gun2_transform, rotation);
        }
    }

    private void FireArrowFromTransform(Transform gunTransform, Vector3 rotation)
    {
        // Make sure we have arrows in the pool
        if (currentArrowNum >= totalNumArrows)
        {
            Debug.LogWarning("Not enough arrows in pool!");
            return;
        }

        // Get next arrow from pool
        currentArrowGO = arrowPoolGO.transform.GetChild(currentArrowNum).gameObject;
        currentArrowGO.transform.position = gunTransform.position;

        // Add velocity to arrow
        Rigidbody2D rb2d = currentArrowGO.GetComponent<Rigidbody2D>();
        Vector3 direction = mousePos - transform.position;
        rb2d.linearVelocity = new Vector2(direction.x, direction.y).normalized * arrowFiringForce;

        // Rotate arrow to face firing direction
        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        currentArrowGO.transform.rotation = Quaternion.Euler(0, 0, rot);

        // Cycle to next arrow in pool
        currentArrowNum = (currentArrowNum + 1) % totalNumArrows;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // for inital enemy collision
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // don't take damage if a wave is not currently active
            if (!waveManager.WaveIsActive) return;

            // only process enemy damage if player health > 0
            if (playerUpgradeManager.GetCurrentHP() <= 0) return;

            if (!inIFrames)
            {
                // Camera Shake to show hit
                CameraShake.Instance.Shake(0.05f, 0.10f);

                // Run hit animation & iFrames
                StartCoroutine(PlayerHitIFrames());

                // Take Damage
                TakeDamage(-enemyManager.GetCurrentDamage());
            }
        }
    }

    // for if enemies are still colliding after initial
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // don't take damage if a wave is not currently active
            if (!waveManager.WaveIsActive) return;

            // only process enemy damage if player health > 0
            if (playerUpgradeManager.GetCurrentHP() <= 0) return;

            if (!inIFrames)
            {
                // Camera Shake to show hit
                CameraShake.Instance.Shake(0.05f, 0.10f);

                // Run hit animation & iFrames
                StartCoroutine(PlayerHitIFrames());

                // Take Damage
                TakeDamage(-enemyManager.GetCurrentDamage());
            }
        }
    }

    private void TakeDamage(int damageTaken)
    {
        playerUpgradeManager.SetPlayerCurrentHP(damageTaken);

        // trigger player death if health is <= 0 after taking damage
        if(playerUpgradeManager.GetIsDead() == true)
            PlayerDeath();
    }

    private void PlayerDeath()
    {
        animator.SetBool("Dead", true);
        SetCanMove(false);
        SetCanFire(false);

        // reset the player's rotation to face down
        transform.rotation = Quaternion.identity;
    }

    IEnumerator PlayerHitIFrames()
    {
        inIFrames = true;
        int numFlashes = 8;

        for (int i = 0; i < numFlashes; i++)
        {
            spriteRenderer.color = new Color(1, 0, 0, 1);
            yield return new WaitForSeconds(0.15f);
            spriteRenderer.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(0.15f);
        }

        inIFrames = false;
    }

    public void SetCanMove(bool _canMove)
    {
        canMove = _canMove;
    }

    public void SetCanFire(bool _canFire)
    {
        canFire = _canFire;
    }
    public void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;
        rb.freezeRotation = true;
    }

    public void WaveStartBehaviors()
    {
        SetCanFire(true);
        SetCanMove(true);
        rb.freezeRotation = false;
    }
    public void WaveStopBehaviors()
    {
        SetCanFire(false);
        SetCanMove(false);
        StopMovement();
        rb.freezeRotation = true;
    }
}