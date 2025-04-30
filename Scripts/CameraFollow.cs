using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (target == null)
        {
            PlayerMovement player = FindObjectOfType<PlayerMovement>();
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            PlayerMovement player = FindObjectOfType<PlayerMovement>();
            if (player != null)
            {
                target = player.transform;
            }
        }

        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            desiredPosition.z = transform.position.z;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}