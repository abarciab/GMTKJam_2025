using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class PlaceObjectsOnMesh : MonoBehaviour
{
    [SerializeField] private GameObject _rockPrefab;
    [SerializeField] private int _numRocks = 20;
    [SerializeField] private float _minY = 0;
    [SerializeField] private Transform _terrainTransform;
    [SerializeField] private Mesh _terrainMesh;
    [SerializeField] private bool _generateOnStart;

    private void Start()
    {
        if (_generateOnStart) GenerateRocks();
    }

    [ButtonMethod]
    private void GenerateRocks()
    {
        for (int i = 0; i < _numRocks; i++) {
            var point = GetPoint();
            while (point.y < _minY) point = GetPoint();
            var newRock = Instantiate(_rockPrefab, _terrainTransform);
            newRock.transform.position = point;
            newRock.transform.localScale = Vector3.one * 0.1f;
            newRock.transform.localEulerAngles = Vector3.zero + Vector3.up * Random.Range(0, 360);
        }
    }

    private Vector3 GetPoint()
    {
        var local = GetRandomPointOnMesh(_terrainMesh);
        return _terrainTransform.TransformPoint(local);
    }

    public static List<int[]> GetTriangles(Mesh mesh)
    {
        List<int[]> trianglesList = new List<int[]>();

        if (mesh != null) {
            int[] triangles = mesh.triangles;

            for (int i = 0; i < triangles.Length; i += 3) {
                int[] triangle = new int[3];
                triangle[0] = triangles[i];
                triangle[1] = triangles[i + 1];
                triangle[2] = triangles[i + 2];
                trianglesList.Add(triangle);
            }
        }

        return trianglesList;
    }

    public static Vector3 GetRandomPointOnMesh(Mesh mesh)
    {
        List<int[]> triangles = GetTriangles(mesh);

        // Calculate total area of all triangles
        float totalArea = 0;
        foreach (var triangle in triangles) {
            Vector3 v0 = mesh.vertices[triangle[0]];
            Vector3 v1 = mesh.vertices[triangle[1]];
            Vector3 v2 = mesh.vertices[triangle[2]];
            totalArea += CalculateTriangleArea(v0, v1, v2);
        }

        // Generate a random number within the range [0, total area)
        float randomArea = Random.Range(0f, totalArea);

        // Iterate through triangles and accumulate areas until reaching the random number
        float accumulatedArea = 0;
        foreach (var triangle in triangles) {
            Vector3 v0 = mesh.vertices[triangle[0]];
            Vector3 v1 = mesh.vertices[triangle[1]];
            Vector3 v2 = mesh.vertices[triangle[2]];
            float triangleArea = CalculateTriangleArea(v0, v1, v2);
            accumulatedArea += triangleArea;

            // If the random number falls within this triangle's accumulated area, return a point within this triangle
            if (randomArea <= accumulatedArea) {
                return RandomPointInTriangle(v0, v1, v2);
            }
        }

        // This point should never be reached under normal circumstances
        Debug.LogError("Failed to generate a random point on the mesh.");
        return Vector3.zero;
    }

    private static float CalculateTriangleArea(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        // Using Heron's formula to calculate triangle area
        float a = (v0 - v1).magnitude;
        float b = (v1 - v2).magnitude;
        float c = (v2 - v0).magnitude;
        float s = (a + b + c) * 0.5f;
        return Mathf.Sqrt(s * (s - a) * (s - b) * (s - c));
    }

    private static Vector3 RandomPointInTriangle(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        // Generate random barycentric coordinates
        float rand1 = Random.Range(0f, 1f);
        float rand2 = Random.Range(0f, 1f - rand1);

        // Convert random barycentric coordinates to cartesian coordinates
        return v0 + rand1 * (v1 - v0) + rand2 * (v2 - v0);
    }
}
