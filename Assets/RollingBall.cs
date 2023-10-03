using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBall : MonoBehaviour
{
    public Transform ground;
    Vector3 gravityForce = Physics.gravity;
    private Vector3 velocity = Vector3.zero;
    private float radius = 0.05f;
    private float mass = 1f;
    private float bounciness = 0f;
    public TriangleSurface surfaceScript;


    // Start is called before the first frame update
    void Start()
    {
        radius = transform.localScale.x * 0.5f;
    }

    void FixedUpdate()
    {
        var collInfo = surfaceScript.CheckCollision(transform.position);

        Vector3 normalForce = new();
        Vector3 acceleration = new();

        float distanceToSurface = Vector3.Distance(transform.position, collInfo.position);

        // If colliding with surface
        if (distanceToSurface <= radius && collInfo.didCollide)
        {
            normalForce = Vector3.Dot(velocity, collInfo.normal) * collInfo.normal;
            velocity = velocity - normalForce - bounciness * normalForce;

            // Move up
            var newPos = transform.position;
            newPos.y = collInfo.position.y + radius;
            transform.position = newPos;
            
        }

        acceleration = ((mass * gravityForce) + normalForce) / mass;
        velocity += acceleration * Time.fixedDeltaTime;
        transform.position += velocity * Time.fixedDeltaTime;
        print("Acceleration: " + acceleration);
        print("Velocity: " + velocity);
        print("Position: " + transform.position);
        //print("Normal vector: " + collInfo.position);
    }
}