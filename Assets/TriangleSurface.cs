using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TriangleSurface : MonoBehaviour
{
    private Vector3[] vertices;
    private Vector3[] cachedVertices;
    [SerializeField] private Material color;

    private static Vector3[] _vertices =
{
        new Vector3(0, 0, 0),
        new Vector3(0, 0.11f, 0.5191f),
        new Vector3(0.5191f, 0, 0.5191f),
        new Vector3(0.5191f, 0.21f, 0),
        new Vector3(0, 0, 1.0382f),
        new Vector3(0.5447f, 0.13f, 1.0688f)
    };


    private int[] _indices =
    {
        0, 1, 2,
        0, 2, 3,
        4, 2, 1,
        4, 5, 2
    };

    // Start is called before the first frame update
    void Start()
    {
        GenerateMesh();

    }
    private void GenerateMesh()
    {
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        var triangleMesh = new Mesh
        {
            vertices = _vertices,
            triangles = _indices
        };

        GetComponent<MeshFilter>().mesh = triangleMesh;
        GetComponent<MeshRenderer>().material = color;
    }

    public Vector3 CalculateGroundNormal()
    {
        // Return the normal vector of the triangular surface
        return transform.up;
    }

    public Vector3 CalculateTriangleNormal(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
    {
        Vector3 edge1 = vertex2 - vertex1;
        Vector3 edge2 = vertex3 - vertex1;
        Vector3 normal = Vector3.Cross(edge1, edge2).normalized;
        return normal;
    }

    public Vector3[] GetCachedVertices()
    {
        return cachedVertices;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
