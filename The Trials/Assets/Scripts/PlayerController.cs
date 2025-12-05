using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Player Components
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] PlayerUpgradeManager playerUpgradeManager;
    [SerializeField] EnemyManager enemyManager;
    [SerializeField] WaveManager waveManager;

    // Shooting stuff
    [SerializeField] private GameObject arrowPoolGO;
    [SerializeField] private float arrowFiringForce = 15f;
    [SerializeField] private Transform gun1_transform;
    [SerializeField] private Transform gun2_transform;
    private Camera mainCam;
    private Vector3 mousePos;
    private bool canFire = true;
    public bool canFireSpecial { get; private set; } = true;
    private int currentArrowNum = 0;
    private GameObject currentArrowGO;
    private int totalNumArrows;
    private int shotsRemaining;
    private bool isReloading = false;
    [SerializeField] GameObject reloadBar;
    [SerializeField] Transform reloadBarTransform;
    [SerializeField] private TMP_Text playerAmmoText;

    // Special Attack Stuff
    [SerializeField] private TMP_Text playerSpecialText;
    [SerializeField] private Color highLerpColor = Color.green;
    [SerializeField] private Color lowLerpColor = Color.red;

    // Crosshair stuff
    [SerializeField] private Transform crosshairTransform;
    [SerializeField] private float rotationOffset = 90f;

    // Movement variables
    private Vector2 input;
    [SerializeField] private bool canMove = true;

    // Take Damage
    private bool inIFrames = false;

    [SerializeField] AudioManager audioManager;
    private void Awake()
    {
        mainCam = Camera.main;

        // If crosshair not assigned, try to find it as child
        if (crosshairTransform == null)
        {
            crosshairTransform = transform.GetChild(0);
        }

        shotsRemaining = playerUpgradeManager.GetCurrentPlayerMagazineSize();
        reloadBar.SetActive(false);
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
        HandleSpecialAttack();
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
        // Don't shoot if player is reloading
        if (isReloading)
            return;

        // Fire arrows on mouse click
        if (Input.GetMouseButtonDown(0) && canFire)
        {
            audioManager.PlayShootSFX();

            // Play firing animation
            animator.SetTrigger("Shoot");

            // Get total arrows from pool
            totalNumArrows = arrowPoolGO.transform.childCount;

            // Fire from gun1
            FireArrowFromTransform(gun1_transform, rotation);

            // Fire from gun2
            FireArrowFromTransform(gun2_transform, rotation);

            // decrement bullets left in magazine, and begin reload sequence if <= 0
            shotsRemaining--;
            playerAmmoText.text = shotsRemaining.ToString();
            playerAmmoText.color = Color.Lerp(lowLerpColor, highLerpColor, shotsRemaining / (float)playerUpgradeManager.GetCurrentPlayerMagazineSize());


            if (shotsRemaining <= 0)
            {
                isReloading = true;
                StartCoroutine(ReloadSequence());
            }
        }
    }

    private void HandleSpecialAttack()
    {
        // Fire special on spacebar
        if (Input.GetKeyDown(KeyCode.Space) && canFireSpecial)
        {
            // Disable special attack and start its cooldown
            SetCanFireSpecial(false);
            StartCoroutine(SpecialAttackSequence(playerUpgradeManager.GetCurrentPlayerSpecialAttacks()));


            // Play firing animation
            animator.SetTrigger("Shoot");

            // Get total arrows from pool
            totalNumArrows = arrowPoolGO.transform.childCount;
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

        // get next arrow from pool
        currentArrowGO = arrowPoolGO.transform.GetChild(currentArrowNum).gameObject;
        currentArrowGO.transform.position = gunTransform.position;

        // Add velocity to arrow
        Rigidbody2D rb2d = currentArrowGO.GetComponent<Rigidbody2D>();
        Vector3 direction = mousePos - transform.position;
        rb2d.linearVelocity = new Vector2(direction.x, direction.y).normalized * arrowFiringForce;

        // rotate arrow to face firing direction
        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        currentArrowGO.transform.rotation = Quaternion.Euler(0, 0, rot);

        // cycle to next arrow in pool
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

        // for healthpack pickup
        if (collision.gameObject.CompareTag("Healthpack"))
        {
            HealthPack healthPack = collision.GetComponent<HealthPack>();

            if (!healthPack.healthpackPickedUp)
            {
                Debug.Log($"Player healed for {playerUpgradeManager.GetCurrentPlayerHealAmount()} HP!");
                collision.GetComponent<HealthPack>().Reset();
                playerUpgradeManager.SetPlayerCurrentHP(playerUpgradeManager.GetCurrentPlayerHealAmount());
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
        // do not take damage if the player is dead
        if (playerUpgradeManager.GetIsDead())
            return;

        playerUpgradeManager.SetPlayerCurrentHP(damageTaken);

        // trigger player death if health is <= 0 after taking damage
        if(playerUpgradeManager.GetIsDead() == true)
            PlayerDeath();
    }

    private void PlayerDeath()
    {
        audioManager.StopMusic();

        animator.SetBool("Dead", true);
        SetCanMove(false);
        SetCanFire(false);

        // reset the player's rotation to face down
        transform.rotation = Quaternion.identity;

        waveManager.EndWaveGameOver();
    }

    IEnumerator ReloadSequence()
    {
        // reset the bar to the beginning
        reloadBar.SetActive(true);
        reloadBarTransform.localScale = new Vector2(1, 10);

        // play reloading animation
        float elapsed = 0f;
        Vector3 startScale = reloadBarTransform.localScale;
        float reloadTime = playerUpgradeManager.GetCurrentPlayerReloadSpeed();
        float targetScaleX = 60f;

        // scale the reload bar over the duration of player's reload time
        while (elapsed < reloadTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / reloadTime);

            float newX = Mathf.Lerp(startScale.x, targetScaleX, t);
            reloadBarTransform.localScale = new Vector3(newX, startScale.y, startScale.z);

            yield return null;
        }
        // snap exactly to the target scale (60 on the x axis) at the end of the reload animation
        reloadBarTransform.localScale = new Vector3(targetScaleX, startScale.y, startScale.z);

        reloadBar.SetActive(false);


        // give the player a full magazine and allow them to shoot again
        shotsRemaining = playerUpgradeManager.GetCurrentPlayerMagazineSize();
        playerAmmoText.text = shotsRemaining.ToString();
        playerAmmoText.color = highLerpColor;
        isReloading = false;
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

    public void SetCanFireSpecial(bool _canFireSpecial)
    {
        canFireSpecial = _canFireSpecial;
    }
    public void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;
        rb.freezeRotation = true;
    }

    public void WaveStartBehaviors()
    {
        shotsRemaining = playerUpgradeManager.GetCurrentPlayerMagazineSize();
        playerAmmoText.text = shotsRemaining.ToString();
        playerAmmoText.color = highLerpColor;
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

    public void SpecialAttack(float startingAngle)
    {
        // to rotate each shot 45 degrees from the previous
        float angleOffset = startingAngle;

        for (int i = 0; i < 8; i++)
        {
            // Get arrow from pool
            GameObject bullet = arrowPoolGO.transform.GetChild(currentArrowNum).gameObject;

            // Set arrow position to player
            bullet.transform.position = transform.position;
            bullet.SetActive(true);

            // Calculate direction from angle
            float angle = i * angleOffset;
            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            ).normalized;

            // Set rotation so arrow faces the direction
            bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            // Apply velocity
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = dir * arrowFiringForce;

            // Cycle to next arrow in pool
            currentArrowNum = (currentArrowNum + 1) % totalNumArrows;
        }

    }

    // amount of time that needs to pass before the special attack can be used again
    IEnumerator SpecialCooldown()
    {
        playerSpecialText.text = "";
        playerSpecialText.color = lowLerpColor;

        for (int i = 0; i< 8; i++)
        {
            yield return new WaitForSeconds(1.50f);
            playerSpecialText.text += "|";

            playerSpecialText.color = Color.Lerp(lowLerpColor, highLerpColor, i/8f);
        }

        SetCanFireSpecial(true);
    }

    private void FireEightWayBurst(float startingAngle)
    {
        for (int i = 0; i < 8; i++)
        {
            // for each bullet spawn, rotate its angle by 45deg from the starting angle
            float angle = startingAngle + (45f * i);

            // get the next bullet from the pool, move it to the player, set it as active
            GameObject bullet = arrowPoolGO.transform.GetChild(currentArrowNum).gameObject;
            bullet.transform.position = transform.position;
            bullet.SetActive(true);

            // get a vector2 direction to fire the arrow from the calculated angle
            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            ).normalized;

            // rotate the bullet game object to face the direction it is moving
            bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

            // get the bullet's rb and apply velocity in the calculated direction
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = dir * arrowFiringForce;

            // get the next arrow from the pool
            currentArrowNum = (currentArrowNum + 1) % totalNumArrows;
        }
    }
    private IEnumerator SpecialAttackSequence(int playerSpecialAttacks)
    {
        // start cooldown immediately to account for longer, upgraded, special attacks
        StartCoroutine(SpecialCooldown());

        totalNumArrows = arrowPoolGO.transform.childCount;
        float angle1 = 45f;
        float angle2 = 67.5f;
        float angleToShoot;

        for (int i = 0; i < playerSpecialAttacks; i++)
        {
            if (i % 2 != 0)
                angleToShoot = angle1;
            else
                angleToShoot = angle2;
            FireEightWayBurst(angleToShoot);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void ResetPlayer()
    {
        // Move player to center of room
        transform.position = Vector2.zero;


        playerAmmoText.text = shotsRemaining.ToString();
        playerAmmoText.color = Color.Lerp(lowLerpColor, highLerpColor, shotsRemaining / (float)playerUpgradeManager.GetCurrentPlayerMagazineSize());

    }
}