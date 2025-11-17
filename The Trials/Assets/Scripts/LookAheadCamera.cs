using UnityEngine;

public class LookAheadCamera : MonoBehaviour
{
    [Header("Basic Follow")]
    [SerializeField] private Vector3 offset;   // offset from player (default = 0)
    [SerializeField] private float damping = 0.15f; // how fast to catch up to player

    public Transform target;                  // target of the camera (player)

    [Header("Look Ahead")]
    [SerializeField] private float lookAheadDistance = 2f;   // how far in front of movement
    [SerializeField] private float moveThreshold = 0.05f;    // min movement before look-ahead changes
    [SerializeField] private float lookAheadReturnSpeed = 2f; // how fast camera recenters when player slows

    private Vector3 vel = Vector3.zero;
    private Vector3 lastTargetPosition;
    private Vector3 lookAheadOffset = Vector3.zero;

    private void Start()
    {
        if (target == null) return;

        lastTargetPosition = target.position;

        // Snap camera to starting position
        Vector3 startPos = target.position + offset;
        startPos.z = transform.position.z;
        transform.position = startPos;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position;
        Vector3 moveDelta = targetPosition - lastTargetPosition;

        // --- UPDATE LOOK-AHEAD BASED ON MOVEMENT ---
        if (moveDelta.sqrMagnitude > moveThreshold * moveThreshold)
        {
            // Player moved enough: push camera ahead in that direction
            Vector3 dir = moveDelta.normalized;
            lookAheadOffset = dir * lookAheadDistance;
        }
        else
        {
            // Player slowed / stopped: gently pull camera back toward center
            lookAheadOffset = Vector3.MoveTowards(
                lookAheadOffset,
                Vector3.zero,
                lookAheadReturnSpeed * Time.deltaTime
            );
        }

        // Desired camera position = player + offset + look-ahead
        Vector3 desiredPosition = targetPosition + offset + lookAheadOffset;
        desiredPosition.z = transform.position.z; // keep current camera Z

        // Smoothly interpolate cam position toward desired position
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref vel,
            damping
        );

        lastTargetPosition = targetPosition;
    }
}
