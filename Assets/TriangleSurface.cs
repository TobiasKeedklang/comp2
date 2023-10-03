using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static TriangleSurface;

public class TriangleSurface : MonoBehaviour
{
    Vector3[] vertices = new Vector3[6];
    Vector2[] uv = new Vector2[6];
    int[] triangles = new int[12];

    public struct CollInfo
    {
        public bool didCollide;
        public Vector3 normal;
        public Vector3 position;
    }

    void Start()
    {
        Mesh surface = new Mesh();

        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(0, 0.11f, 0.5191f);
        vertices[2] = new Vector3(0.5191f, 0, 0.5191f);
        vertices[3] = new Vector3(0.5191f, 0.21f, 0);
        vertices[4] = new Vector3(0, 0, 1.0382f);
        vertices[5] = new Vector3(0.5447f, 0.13f, 1.0688f);

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(0, 0);
        uv[2] = new Vector2(0, 0);
        uv[3] = new Vector2(0, 0);
        uv[4] = new Vector2(0, 0);
        uv[5] = new Vector2(0, 0);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;

        triangles[6] = 4;
        triangles[7] = 2;
        triangles[8] = 1;

        triangles[9] = 4;
        triangles[10] = 5;
        triangles[11] = 2;

        surface.vertices = vertices;
        surface.triangles = triangles;

        GetComponent<MeshFilter>().mesh = surface;
        print("normal.normalized: " + CalculateGroundNormal());
    }

    public Vector3 CalculateGroundNormal()
    {
        Vector3 averageNormal = Vector3.zero;

        for (int i = 0; i < vertices.Length; i += 3)
        {
            Vector3 triangleNormal = CalculateTriangleNormal(vertices[i], vertices[i + 1], vertices[i + 2]);
            averageNormal += triangleNormal;
        }

        averageNormal /= (float)vertices.Length / 3;

        
        return averageNormal.normalized;
        
    }

    Vector3 Barycentric(Vector3 point, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
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

        return new Vector3(u, v, invDenominator);
    }

    public CollInfo CheckCollision(Vector3 point)
    {
        CollInfo info = new();
        info.position = point;

        // Check each triangle
        for (var i = 0; i < triangles.Length; i += 3)
        {
            int i1 = triangles[i];
            int i2 = triangles[i + 1];
            int i3 = triangles[i + 2];

            var v1 = vertices[i1];
            var v2 = vertices[i2];
            var v3 = vertices[i3];

            var v1e = new Vector2(v1.x, v1.z);
            var v2e = new Vector2(v2.x, v2.z);
            var v3e = new Vector2(v3.x, v3.z);

            var pt = new Vector2(point.x, point.z);
            float u, v, w;
            Barycentric(v1e, v2e, v3e, pt, out u, out v, out w);

            // Check if point is inside triangle
            if (u is >= 0f and <= 1f && v is >= 0f and <= 1f && w is >= 0f and <= 1f)
            {
                info.didCollide = true;
                info.position.y = vertices[i1].y * u + vertices[i2].y * v + vertices[i3].y * w;
                info.normal = CalculateTriangleNormal(vertices[i1], vertices[i2], vertices[i3]);
                return info;
            }
            
        }

        return info;
    }

    void Barycentric(Vector2 a, Vector2 b, Vector2 c, Vector2 p, out float u, out float v, out float w)
    {
        Vector2 v0 = b - a;
        Vector2 v1 = c - a;
        Vector2 v2 = p - a;

        float d00 = Vector2.Dot(v0, v0);
        float d01 = Vector2.Dot(v0, v1);
        float d11 = Vector2.Dot(v1, v1);
        float d20 = Vector2.Dot(v2, v0);
        float d21 = Vector2.Dot(v2, v1);

        float denom = d00 * d11 - d01 * d01;

        v = (d11 * d20 - d01 * d21) / denom;
        w = (d00 * d21 - d01 * d20) / denom;
        u = 1.0f - v - w;
    }

    public Vector3 CalculateTriangleNormal(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
    {
        //print("vertex1: " + vertex1 + "vertex1: " + vertex2 + "vertex1: " + vertex3);
        Vector3 edge1 = vertex2 - vertex1;
        Vector3 edge2 = vertex3 - vertex1;
        Vector3 normal = Vector3.Cross(edge1, edge2).normalized;
        

        //print("Normal: " + normal);

        return normal;
    }

    private void OnDrawGizmos()
    {
        // Draw normals for each triangle
        for (var i = 0; i < triangles.Length; i += 3)
        {
            int i1 = triangles[i];
            int i2 = triangles[i + 1];
            int i3 = triangles[i + 2];

            var v1 = vertices[i1];
            var v2 = vertices[i2];
            var v3 = vertices[i3];

            // Calculate the normal vector
            Vector3 normal = CalculateTriangleNormal(v1, v2, v3).normalized;

            // Print the normal vector to the console
            //print("Triangle " + (i / 3 + 1) + " Normal: " + normal);

            // Draw the normal vector as a line
            Gizmos.color = UnityEngine.Color.cyan;
            Gizmos.DrawLine(v1, v1 + normal);
            Gizmos.DrawLine(v2, v2 + normal);
            Gizmos.DrawLine(v3, v3 + normal);
        }

        // Draw line around each triangle
        for (var i = 0; i < triangles.Length; i += 3)
        {
            int i1 = triangles[i];
            int i2 = triangles[i + 1];
            int i3 = triangles[i + 2];

            var v1 = vertices[i1];
            var v2 = vertices[i2];
            var v3 = vertices[i3];

            Gizmos.color = UnityEngine.Color.yellow;
            Gizmos.DrawLine(v1, v2);
            Gizmos.DrawLine(v2, v3);
            Gizmos.DrawLine(v3, v1);
        }
    }
}