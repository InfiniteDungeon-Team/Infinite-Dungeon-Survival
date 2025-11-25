using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private Vector3 initialLocalPos;
    private Coroutine shakeRoutine;

    private void Awake()
    {
        // singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        initialLocalPos = transform.localPosition;
    }

    public void Shake(float duration, float magnitude)
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < duration)
        {
            Vector2 offset2D = Random.insideUnitCircle * magnitude;
            transform.position = startPos + new Vector3(offset2D.x, offset2D.y, 0f);

            elapsed += Time.deltaTime;
            yield return null;

            // refresh base position so it stays around follow target
            startPos = transform.position;
        }

        shakeRoutine = null;
    }
}
