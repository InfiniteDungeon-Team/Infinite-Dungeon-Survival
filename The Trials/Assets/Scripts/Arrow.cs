using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // When the bullet hits a wall, stop it's velocity and move it out of sight of the player.
        if (collision.gameObject.CompareTag("Wall"))
        {
            gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

            animator.SetTrigger("HitWall");

            if (collision.gameObject.name.Contains("North"))
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 180f);
            }
            else if (collision.gameObject.name.Contains("East"))
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            }
            else if (collision.gameObject.name.Contains("West"))
            {
                transform.rotation = Quaternion.Euler(0f, 0f, -90f);
            }
            else if (collision.gameObject.name.Contains("South"))
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            StartCoroutine(WaitBeforeRemove(1f));            
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            animator.SetTrigger("HitWall");
            StartCoroutine(WaitBeforeRemove(1f));
        }
    }

    private IEnumerator WaitBeforeRemove(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.transform.position = new Vector2(0, -999); // move the bullet out of player's sight
    }
}
