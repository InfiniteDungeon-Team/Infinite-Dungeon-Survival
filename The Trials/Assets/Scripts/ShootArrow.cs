using UnityEngine;
using UnityEngine.Rendering;

public class ShootArrow : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 mousePos;
    private Transform crosshairTransform;
    public bool canFire;
    private float timer;
    public float timeBetweenFiring;

    public GameObject arrowPoolGO;

    private int currentArrowNum = 0; // iterator for the current arrow "index" of the pool
    GameObject currentArrowGO; // the currently fired arrow game object
    private int totalNumArrows; // total number of arrows in the pool
    private float force; // force on a fired arrow 

    void Awake()
    {
        force = 15f;
        timeBetweenFiring = 0.15f;
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        crosshairTransform = transform.GetChild(0);
    }

    void Update()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 rotation = mousePos - transform.position;

        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        // Only rotate to position the crosshair around the player
        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        // Counter-rotate the sprite to keep it upright
        crosshairTransform.rotation = Quaternion.identity;

        // establish a firing delay timer
        if (!canFire)
        {
            timer += Time.deltaTime;
            if(timer > timeBetweenFiring)
            {
                canFire = true;
                timer = 0;
            }
        }

        // Get mouse input for firing arrows
        if(Input.GetMouseButtonDown(0) && canFire)
        {
            canFire = false;
            // get the current total number of arrows from the pool
            totalNumArrows = arrowPoolGO.transform.childCount;

            // move the next arrow from the pool to the player's position
            currentArrowGO = arrowPoolGO.transform.GetChild(currentArrowNum).gameObject;
            currentArrowGO.transform.position = transform.position;            

            // add velocity to the arrow
            Rigidbody2D rb2d = currentArrowGO.GetComponent<Rigidbody2D>();
            Vector3 direction = mousePos - transform.position;

            // add velocity to the fired arrow
            rb2d.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;

            // rotate the arrow to face the direction it is fired in
            float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            currentArrowGO.transform.rotation = Quaternion.Euler(0, 0, rot);

            currentArrowNum += 1;
            // reset to start with arrowPool arrow #0 if we've reached the end of the pool
            if (currentArrowNum + 1 >= totalNumArrows)
                currentArrowNum = 0;
        }
    }
}