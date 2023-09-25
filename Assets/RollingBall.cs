using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float gravity = 9.81f;
    public Transform ground;
    private Vector3 velocity = Vector3.zero;
    private float radius;
    public TriangleSurface surfaceScript;
    private Vector3[] cachedVertices;


    // Start is called before the first frame update
    void Start()
    {
        surfaceScript = GameObject.FindObjectOfType<TriangleSurface>();
        if (surfaceScript == null)
        {
            Debug.LogError("TriangleSurface script not found.");
        }

        radius = transform.localScale.x * 0.5f;
        cachedVertices = surfaceScript.GetCachedVertices();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 gravityForce = Vector3.down * gravity;
        float ballRadius = 0.05f;

        Vector3 groundNormal = surfaceScript.CalculateGroundNormal();

        if (BallCollisionCheck(transform.position, ballRadius, groundNormal))
        {
            Debug.LogError("collides");

            Vector3[] vertices = cachedVertices;

            Vector3 triangleNormal = CalculateTriangleNormal(vertices[0], vertices[1], vertices[2]);

            Vector3 normalForce = CalculateNormalForce(gravityForce, triangleNormal);
            Vector3 totalForce = gravityForce + normalForce;

            Vector3 acceleration = totalForce;

            Vector3 horizontalVelocity = Vector3.ProjectOnPlane(velocity, groundNormal);
            velocity = horizontalVelocity + acceleration * Time.deltaTime;
        }
        else
        {
            velocity += gravityForce * Time.deltaTime;
        }

        Vector3 movement = velocity * Time.deltaTime;

        transform.Translate(movement);

    }


    bool BallCollisionCheck(Vector3 center, float radius, Vector3 groundNormal)
    {
        float distanceToSurface = center.y - surfaceScript.transform.position.y;

        if (distanceToSurface <= radius)
        {
            if (CheckCollision(center, groundNormal))
            {
                return true;
            }
        }

        return false;
    }
    bool CheckCollision(Vector3 point, Vector3 groundNormal)
    {
        Vector3 localPoint = surfaceScript.transform.InverseTransformPoint(point);

        Vector3[] vertices = new Vector3[6];
        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(0, 0.11f, 0.5191f);
        vertices[2] = new Vector3(0.5191f, 0, 0.5191f);
        vertices[3] = new Vector3(0.5191f, 0.21f, 0);
        vertices[4] = new Vector3(0, 0, 1.0382f);
        vertices[5] = new Vector3(0.5447f, 0.13f, 1.0688f);

        bool insideSurface = IsPointInsideTriangle(localPoint, vertices[0], vertices[1], vertices[2]) ||
                             IsPointInsideTriangle(localPoint, vertices[0], vertices[2], vertices[3]) ||
                             IsPointInsideTriangle(localPoint, vertices[4], vertices[2], vertices[1]) ||
                             IsPointInsideTriangle(localPoint, vertices[4], vertices[5], vertices[2]);

        Debug.DrawLine(surfaceScript.transform.TransformPoint(vertices[0]), surfaceScript.transform.TransformPoint(vertices[1]), Color.green);
        Debug.DrawLine(surfaceScript.transform.TransformPoint(vertices[1]), surfaceScript.transform.TransformPoint(vertices[2]), Color.green);
        Debug.DrawLine(surfaceScript.transform.TransformPoint(vertices[2]), surfaceScript.transform.TransformPoint(vertices[0]), Color.green);

        Debug.DrawLine(surfaceScript.transform.TransformPoint(vertices[0]), surfaceScript.transform.TransformPoint(vertices[2]), Color.green);
        Debug.DrawLine(surfaceScript.transform.TransformPoint(vertices[2]), surfaceScript.transform.TransformPoint(vertices[3]), Color.green);
        Debug.DrawLine(surfaceScript.transform.TransformPoint(vertices[3]), surfaceScript.transform.TransformPoint(vertices[0]), Color.green);

        Debug.DrawLine(surfaceScript.transform.TransformPoint(vertices[4]), surfaceScript.transform.TransformPoint(vertices[1]), Color.green);
        Debug.DrawLine(surfaceScript.transform.TransformPoint(vertices[1]), surfaceScript.transform.TransformPoint(vertices[2]), Color.green);
        Debug.DrawLine(surfaceScript.transform.TransformPoint(vertices[2]), surfaceScript.transform.TransformPoint(vertices[4]), Color.green);

        Debug.DrawLine(surfaceScript.transform.TransformPoint(vertices[4]), surfaceScript.transform.TransformPoint(vertices[5]), Color.green);
        Debug.DrawLine(surfaceScript.transform.TransformPoint(vertices[5]), surfaceScript.transform.TransformPoint(vertices[2]), Color.green);
        Debug.DrawLine(surfaceScript.transform.TransformPoint(vertices[2]), surfaceScript.transform.TransformPoint(vertices[4]), Color.green);

        DrawNormal(surfaceScript.transform, vertices[0], vertices[1], vertices[2]);
        DrawNormal(surfaceScript.transform, vertices[0], vertices[2], vertices[3]);
        DrawNormal(surfaceScript.transform, vertices[4], vertices[2], vertices[1]);
        DrawNormal(surfaceScript.transform, vertices[4], vertices[5], vertices[2]);

        Debug.DrawLine(surfaceScript.transform.TransformPoint(localPoint), surfaceScript.transform.TransformPoint(localPoint) + Vector3.up, Color.red);

        return insideSurface;
    }

    Vector3 CalculateTriangleNormal(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
    {
        Vector3 edge1 = vertex2 - vertex1;
        Vector3 edge2 = vertex3 - vertex1;
        Vector3 normal = Vector3.Cross(edge1, edge2).normalized;
        return normal;
    }

    void DrawNormal(Transform groundTransform, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
    {
        Vector3 center = (vertex1 + vertex2 + vertex3) / 3.0f;
        Vector3 normal = Vector3.Cross(vertex2 - vertex1, vertex3 - vertex1).normalized;
        Debug.DrawLine(groundTransform.TransformPoint(center), groundTransform.TransformPoint(center) + normal, Color.blue);
    }
    bool IsPointInsideTriangle(Vector3 point, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
    {
        Vector3 v0 = vertex2 - vertex1;
        Vector3 v1 = vertex3 - vertex1;
        Vector3 v2 = point - vertex1;

        float dot00 = Vector3.Dot(v0, v0);
        float dot01 = Vector3.Dot(v0, v1);
        float dot02 = Vector3.Dot(v0, v2);
        float dot11 = Vector3.Dot(v1, v1);
        float dot12 = Vector3.Dot(v1, v2);

        float invDenominator = 1.0f / (dot00 * dot11 - dot01 * dot01);
        float u = (dot11 * dot02 - dot01 * dot12) * invDenominator;
        float v = (dot00 * dot12 - dot01 * dot02) * invDenominator;

        return (u >= 0) && (v >= 0) && (u + v <= 1);
    }


    private Vector3 CalculateNormalForce(Vector3 gravityForce, Vector3 groundNormal)
    {
        Vector3 normalForce = -Vector3.Project(gravityForce, groundNormal);

        return normalForce;
    }

}
