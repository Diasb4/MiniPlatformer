using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    private Vector3 target;
    private Vector3 lastPos;
    
    public Vector3 Velocity { get; private set; }

    void Start()
    {
        target = pointB.position;
        lastPos = transform.position;
    }

    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);
        if (Vector3.Distance(transform.position, target) < 0.01f)
            target = (target == pointA.position) ? pointB.position : pointA.position;
        
        Velocity = (transform.position - lastPos) / Time.fixedDeltaTime;
        lastPos = transform.position;
    }
    
}
