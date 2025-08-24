using System.Collections.Generic;
using UnityEngine;

public static class PolygonUtils
{
    public static Vector2 GetRandomPointInPolygon(List<Vector2> polygon)
    {
        if (polygon == null || polygon.Count < 3)
        {
            Debug.LogError("El polígono debe tener al menos 3 puntos.");
            return Vector2.zero;
        }

        // Triangular el polígono
        List<Triangle> triangles = TriangulatePolygon(polygon);

        if (triangles.Count == 0)
        {
            Debug.LogError("No se pudo triangular el polígono.");
            return Vector2.zero;
        }

        // Calcular áreas acumuladas para ponderar la selección
        float totalArea = 0f;
        foreach (var t in triangles) totalArea += t.Area;

        float randomValue = Random.value * totalArea;
        float cumulative = 0f;

        Triangle selected = triangles[0];
        foreach (var t in triangles)
        {
            cumulative += t.Area;
            if (randomValue <= cumulative)
            {
                selected = t;
                break;
            }
        }

        // Devolver un punto aleatorio dentro del triángulo elegido
        return RandomPointInTriangle(selected.A, selected.B, selected.C);
    }

    private static Vector2 RandomPointInTriangle(Vector2 a, Vector2 b, Vector2 c)
    {
        float r1 = Mathf.Sqrt(Random.value);
        float r2 = Random.value;

        return (1 - r1) * a +
               (r1 * (1 - r2)) * b +
               (r1 * r2) * c;
    }

    private struct Triangle
    {
        public Vector2 A, B, C;
        public float Area => Mathf.Abs((A.x * (B.y - C.y) + B.x * (C.y - A.y) + C.x * (A.y - B.y)) / 2f);
    }

    // Triangulación simple asumiendo polígono convexo
    private static List<Triangle> TriangulatePolygon(List<Vector2> polygon)
    {
        List<Triangle> triangles = new List<Triangle>();
        for (int i = 1; i < polygon.Count - 1; i++)
        {
            triangles.Add(new Triangle
            {
                A = polygon[0],
                B = polygon[i],
                C = polygon[i + 1]
            });
        }
        return triangles;
    }
}