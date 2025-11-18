using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset; // offset from player (default = 0)
    [SerializeField] private float damping; // how fast try to catch up to player

    public Transform target; // target of the camera (player). Set to public in case we need to change the target in the future

    private Vector3 vel = Vector3.zero;

    private void LateUpdate()
    {
        Vector3 targetPosition = target.position + offset;
        targetPosition.z = transform.position.z;

        // Smoothly interpolate cam position toward target, where 'vel' is automatically updated each frame to maintain smooth velocity
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref vel, damping);
    }

}
