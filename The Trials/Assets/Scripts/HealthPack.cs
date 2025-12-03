using System.Collections;
using UnityEngine;

public class HealthPack : MonoBehaviour
{

    // Components
    [SerializeField] Transform shadowTransform;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] BoxCollider2D boxCollider;

    // Drop Zone Stuff
    private float spawnBoundsPosX = 6.5f;
    private float spawnBoundsPosY = 6.5f;
    private float spawnBoundsNegX = -6.5f;
    private float spawnBoundsNegY = -6.5f;
    private Vector2 dropZonePos;

    // Fall Speed of the Healthpack
    private float fallSpeed = 10f;

    // Shadow Stuff
    private float shrinkStartDistance = 10f; // how far away the healthpack should be from the shadow when the shadow starts shrinking
    private float shadowShrinkSpeed = 1f; // how fast to shrink the shadow
    private bool isDropping = false;
    private bool shadowShrinking = false;

    public bool healthpackPickedUp { get; private set; }

    private void Awake()
    {
        Reset();
    }

    // Set at which time during the wave the healthpack will drop
    public void SetDropTime(float waveDuration)
    {
        StartCoroutine(CountdownToDrop(Random.Range(10, waveDuration - 5f)));
    }

    IEnumerator CountdownToDrop(float time)
    {
        Debug.Log($"Healthpack inbound in {time} seconds!");
        yield return new WaitForSeconds(time);
        TweenIn();
    }

    private void LateUpdate()
    {
        if (!isDropping) return;

        float distanceY = Mathf.Abs(transform.position.y - dropZonePos.y);

        // Start shrinking the shadow when the healthpack is within the start shrink distance
        if (!shadowShrinking && distanceY < shrinkStartDistance)
        {
            shadowShrinking = true;
        }

        if (shadowShrinking)
        {
            // shrink the shadow
            shadowTransform.localScale = Vector3.Lerp(shadowTransform.localScale, Vector3.zero, shadowShrinkSpeed * Time.deltaTime
            );
        }

        // stop when healthpack has landed
        if (distanceY < 0.1f)
        {
            shadowTransform.localScale = Vector3.zero; // scale the transform to 0 completely
        }
    }

    private Vector2 GetDropZone()
    {
        float x = Random.Range(spawnBoundsNegX, spawnBoundsPosX);
        float y = Random.Range(spawnBoundsNegY, spawnBoundsPosY);
        return new Vector2(x, y);
    }

    public void MoveShadow(Vector2 dropZonePos)
    {
        shadowTransform.localScale = Vector3.one; // reset the size of the transform
        shadowTransform.position = dropZonePos;   
    }

    public void TweenIn()
    {
        isDropping = true;

        healthpackPickedUp = false;

        // Choose random drop location
        dropZonePos = GetDropZone();

        // Place the shadow on the ground
        MoveShadow(dropZonePos);

        // Get the health pack in position on the x axis
        transform.position = new Vector2(dropZonePos.x, transform.position.y);

        // Drop the healthpack
        LeanTween.moveY(gameObject, dropZonePos.y, 1f).setSpeed(fallSpeed).setEase(LeanTweenType.linear).setOnComplete(() => StartCoroutine(CountdownToDespawn()));
    }

    IEnumerator CountdownToDespawn()
    {
        ToggleCollider(true); // turn on the collider so it can be picked up

        yield return new WaitForSeconds(1);

        for (int i = 0; i < 4; i++)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0);
            yield return new WaitForSeconds(0.35f);
            spriteRenderer.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(0.35f);
        }
        for (int i = 0; i < 6; i++)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0);
            yield return new WaitForSeconds(0.18f);
            spriteRenderer.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(0.18f);
        }
        for (int i = 0; i < 10; i++)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0);
            yield return new WaitForSeconds(0.10f);
            spriteRenderer.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(0.10f);
        }

        Reset();
    }

    public void Reset()
    {
        boxCollider.enabled = false;
        transform.position = new Vector2(0, 20f);
        transform.localScale = Vector2.one;
        spriteRenderer.color = new Color(1, 1, 1, 1);
        healthpackPickedUp = true;
    }
    
    public void ToggleCollider(bool toggle)
    {
        boxCollider.enabled = toggle;
    }
}
