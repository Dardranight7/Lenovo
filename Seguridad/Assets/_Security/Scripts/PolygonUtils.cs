using System.Collections.Generic;
using UnityEngine;

public static class PolygonUtils
{
    public static Vector3 GetRandomPointInPolygon(List<Vector3> polygon)
    {
        if (polygon == null || polygon.Count < 3)
        {
            Debug.LogError("El polígono debe tener al menos 3 puntos.");
            return Vector3.zero;
        }

        // Obtener la normal del polígono para proyectarlo en 2D
        Vector3 normal = Vector3.Cross(polygon[1] - polygon[0], polygon[2] - polygon[0]).normalized;

        // Elegir el eje más adecuado para proyectar a 2D
        int axis = GetProjectionAxis(normal);

        // Proyectar a 2D para triangular
        List<Vector2> polygon2D = ProjectTo2D(polygon, axis);

        // Triangular en 2D
        List<Triangle2D> triangles2D = TriangulatePolygon(polygon2D);

        if (triangles2D.Count == 0)
        {
            Debug.LogError("No se pudo triangular el polígono.");
            return Vector3.zero;
        }

        // Calcular áreas acumuladas
        float totalArea = 0f;
        foreach (var t in triangles2D) totalArea += t.Area;

        float randomValue = Random.value * totalArea;
        float cumulative = 0f;

        Triangle2D selected = triangles2D[0];
        foreach (var t in triangles2D)
        {
            cumulative += t.Area;
            if (randomValue <= cumulative)
            {
                selected = t;
                break;
            }
        }

        // Generar un punto aleatorio dentro del triángulo en 2D
        Vector2 point2D = RandomPointInTriangle(selected.A, selected.B, selected.C);

        // Convertir de vuelta a 3D
        return UnprojectTo3D(point2D, axis, polygon, polygon2D);
    }

    private static Vector2 RandomPointInTriangle(Vector2 a, Vector2 b, Vector2 c)
    {
        float r1 = Mathf.Sqrt(Random.value);
        float r2 = Random.value;

        return (1 - r1) * a +
               (r1 * (1 - r2)) * b +
               (r1 * r2) * c;
    }

    private struct Triangle2D
    {
        public Vector2 A, B, C;
        public float Area => Mathf.Abs((A.x * (B.y - C.y) + B.x * (C.y - A.y) + C.x * (A.y - B.y)) / 2f);
    }

    // Triangulación simple asumiendo polígono convexo
    private static List<Triangle2D> TriangulatePolygon(List<Vector2> polygon)
    {
        List<Triangle2D> triangles = new List<Triangle2D>();
        for (int i = 1; i < polygon.Count - 1; i++)
        {
            triangles.Add(new Triangle2D
            {
                A = polygon[0],
                B = polygon[i],
                C = polygon[i + 1]
            });
        }
        return triangles;
    }

    // Escoge en qué plano proyectar (XY, YZ, XZ) según la normal
    private static int GetProjectionAxis(Vector3 normal)
    {
        normal = new Vector3(Mathf.Abs(normal.x), Mathf.Abs(normal.y), Mathf.Abs(normal.z));
        if (normal.z >= normal.x && normal.z >= normal.y)
            return 2; // proyectar en XY
        else if (normal.x >= normal.y)
            return 0; // proyectar en YZ
        else
            return 1; // proyectar en XZ
    }

    private static List<Vector2> ProjectTo2D(List<Vector3> polygon, int axis)
    {
        List<Vector2> result = new List<Vector2>(polygon.Count);
        foreach (var v in polygon)
        {
            switch (axis)
            {
                case 0: result.Add(new Vector2(v.y, v.z)); break; // Proyección YZ
                case 1: result.Add(new Vector2(v.x, v.z)); break; // Proyección XZ
                case 2: result.Add(new Vector2(v.x, v.y)); break; // Proyección XY
            }
        }
        return result;
    }

    private static Vector3 UnprojectTo3D(Vector2 point, int axis, List<Vector3> polygon3D, List<Vector2> polygon2D)
    {
        // Usar correspondencia entre el primer punto 2D y 3D para reconstruir
        Vector3 base3D = polygon3D[0];
        Vector2 base2D = polygon2D[0];

        switch (axis)
        {
            case 0: return new Vector3(base3D.x, point.x, point.y);
            case 1: return new Vector3(point.x, base3D.y, point.y);
            case 2: return new Vector3(point.x, point.y, base3D.z);
            default: return Vector3.zero;
        }
    }
}
