using UnityEngine;

// This script is used to move the enemy from the spawn locations into the arena,
// which requires movement logic different from the main enemy controller.

// It is disabled after the enemy enters the arena and is ready to attack player.

public class EnemyMoveToArena : MonoBehaviour
{
    [SerializeField] Collider2D mainCollider2D; // the game action collider
    [SerializeField] Collider2D tempCollider2D; // temporary collider needed only for moving into arena
    [SerializeField] Transform arenaAreaTransform;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Enemy enemyScript;

    private Vector2 moveDirection;

    private void OnEnable()
    {
        mainCollider2D.enabled = false;
        tempCollider2D.enabled = true;
        enemyScript.enabled = false;
    }

    private void Update()
    {
        Vector3 direction = (arenaAreaTransform.position - transform.position).normalized;
        moveDirection = direction;
        transform.up = -moveDirection;
        rb.linearVelocity = new Vector2(moveDirection.x, moveDirection.y) * 2f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == arenaAreaTransform.gameObject.name)
        {
            enemyScript.enabled = true;
            mainCollider2D.enabled = true;
            tempCollider2D.enabled = false;
            this.enabled = false;
        }
    }


}
