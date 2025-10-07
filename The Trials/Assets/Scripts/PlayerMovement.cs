using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float speed;

    [SerializeField] private Rigidbody2D rb;

    private Vector2 input;

    [SerializeField] SpriteRenderer spriteRenderer;

    private void Start()
    {
        speed = 10f;
    }

    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        // Allow for diagonal movement at the same rate as other movement
        input.Normalize();

        // if we're moving to the left, flip the sprite
        if (rb.linearVelocity.x < 0.01f)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = input * speed;
    }
}
