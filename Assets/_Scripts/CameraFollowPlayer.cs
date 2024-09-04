using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{

    [SerializeField] private Transform target;
    private float smoothSpeed = 0.125f;

    private void OnEnable() {
        GameManager.OnPlayerWin += DisablePlayerFollow;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = new Vector3(target.position.x, 0f, transform.position.z);

            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            transform.position = smoothedPosition;
        }
    }

    private void DisablePlayerFollow()
    {
        enabled = false;
    }

    private void OnDisable() {
        GameManager.OnPlayerWin -= DisablePlayerFollow;
    }
}
