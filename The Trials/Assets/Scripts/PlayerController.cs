using UnityEngine;

public class PlayerController : MonoBehaviour
{
<<<<<<< Updated upstream

    [SerializeField] private float speed = 10f;
    [SerializeField] private Rigidbody2D rb;

=======
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] PlayerUpgradeManager playerUpgradeManager;
>>>>>>> Stashed changes

    // Shooting stuff
    [SerializeField] private GameObject arrowPoolGO;
    [SerializeField] private float timeBetweenFiring = 0.15f;
<<<<<<< Updated upstream
    [SerializeField] private float force = 15f;


    [SerializeField] private Transform crosshairTransform;
    [SerializeField] private float rotationOffset = -90f; // Adjust this based on your sprite's default facing direction

    // Movement variables
    private Vector2 input;

    // Shooting variables
=======
    [SerializeField] private float arrowFiringForce = 15f;
    [SerializeField] private Transform gun1_transform;
    [SerializeField] private Transform gun2_transform;
>>>>>>> Stashed changes
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
        // Handle movement input
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input.Normalize();

        // Handle rotation based on mouse position
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotation = mousePos - transform.position;
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        // Rotate the entire player object (including sprite and crosshair)
        // Subtract 90 degrees if your sprite is facing up in its default state
        // You may need to adjust this offset based on your sprite's default orientation
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
<<<<<<< Updated upstream
        rb.linearVelocity = input * speed;
=======
        //rb.linearVelocity = input * 10;
        rb.linearVelocity = input * playerUpgradeManager.GetCurrentMoveSpeed();
>>>>>>> Stashed changes
    }

    private void HandleShooting(Vector3 rotation)
    {
        // Update firing timer
        if (!canFire)
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenFiring)
            {
                canFire = true;
                timer = 0;
            }
        }

        // Fire arrow on mouse click
        if (Input.GetMouseButtonDown(0) && canFire)
        {
            canFire = false;

            // Play firing animation
            animator.SetTrigger("Shoot");

            // Get total arrows from pool
            totalNumArrows = arrowPoolGO.transform.childCount;

            // Get next arrow from pool
            currentArrowGO = arrowPoolGO.transform.GetChild(currentArrowNum).gameObject;
            currentArrowGO.transform.position = transform.position;

            // Add velocity to arrow
            Rigidbody2D rb2d = currentArrowGO.GetComponent<Rigidbody2D>();
            Vector3 direction = mousePos - transform.position;
            rb2d.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;

            // Rotate arrow to face firing direction
            float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            currentArrowGO.transform.rotation = Quaternion.Euler(0, 0, rot);

            // Cycle to next arrow in pool
            currentArrowNum = (currentArrowNum + 1) % totalNumArrows;
        }
    }
}