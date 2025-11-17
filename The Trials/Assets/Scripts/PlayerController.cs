using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerMoveSpeed = 10f;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private Transform gun1_transform;
    [SerializeField] private Transform gun2_transform;

    [SerializeField] private GameObject arrowPoolGO;
    [SerializeField] private float timeBetweenFiring = 0.15f;
    [SerializeField] private float arrowFiringForce = 15f;

    [SerializeField] private Transform crosshairTransform;
    [SerializeField] private float rotationOffset = 90f;

    // Movement variables
    private Vector2 input;

    // Shooting variables
    private Camera mainCam;
    private Vector3 mousePos;
    private bool canFire = true;
    private float timer;
    private int currentArrowNum = 0;
    private GameObject currentArrowGO;
    private int totalNumArrows;

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
        rb.linearVelocity = input * playerMoveSpeed;
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

        // Fire arrows on mouse click
        if (Input.GetMouseButtonDown(0) && canFire)
        {
            canFire = false;

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
}