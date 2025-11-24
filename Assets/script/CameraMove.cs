using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float topLimit = 4f;
    public float rightLimit = 15f;
    public float leftLimit = 0f;
    public float bottomLimit = 0f;

    public float FollowSpeed = 2f;
    public float JumpFollowSpeed = 8f;
    public float YOffset = 0f;
    public Transform target;

    private Rigidbody2D rb;


    void LateUpdate()
    {
        if (target == null) return;

        float targetY = target.position.y + YOffset;
        float targetX = target.position.x;
        float targetZ = -10f;

        targetY = Mathf.Clamp(targetY, bottomLimit, topLimit);
        targetX = Mathf.Clamp(targetX, leftLimit, rightLimit);

        Vector3 targetPos = new Vector3(targetX, targetY, targetZ);

        transform.position = Vector3.Lerp(transform.position, targetPos, FollowSpeed * Time.deltaTime);

    }
}

