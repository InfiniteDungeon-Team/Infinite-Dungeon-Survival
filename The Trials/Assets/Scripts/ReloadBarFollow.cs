using UnityEngine;

public class ReloadBarFollow : MonoBehaviour
{
    // follow the player and stay fixed in place

    [SerializeField] private Transform player;
    private Vector3 offset = new Vector3(0f, -1.75f, 0f);

    private void LateUpdate()
    {
        if (player == null) return;

        transform.position = player.position + offset;
        transform.rotation = Quaternion.identity;
    }
}