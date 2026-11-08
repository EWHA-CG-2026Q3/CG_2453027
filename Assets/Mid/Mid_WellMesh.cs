using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Mid_WellMesh : MonoBehaviour
{
    public MeshFilter waterSurface;
    [Range(8, 32)]
    public int segments = 16;
    Mesh wall;
    Mesh water;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uv = new List<Vector2>();

    void OnEnable()
    {
        Rebuild();
    }

    void OnValidate()
    {
        Rebuild();
    }

    public void Rebuild()
    {
        segments = Mathf.Clamp(segments, 8, 32);
        if (wall == null)
        {
            wall = new Mesh
            {
                name = "Mid_WellMesh",
                hideFlags = HideFlags.DontSave
            };
        }

        vertices.Clear();
        triangles.Clear();
        uv.Clear();
        AddRing(0.70f, 0.49f, 0.00f, 0.64f);
        AddRing(0.76f, 0.47f, 0.64f, 0.76f);
        wall.Clear();
        wall.SetVertices(vertices);
        wall.SetTriangles(triangles, 0);
        wall.SetUVs(0, uv);
        wall.RecalculateNormals();
        wall.RecalculateTangents();
        wall.RecalculateBounds();
        GetComponent<MeshFilter>().sharedMesh = wall;
        MeshCollider collider = GetComponent<MeshCollider>();
        if (collider != null)
        {
            collider.sharedMesh = wall;
        }

        BuildWater();
    }

    void AddRing(float outerRadius, float innerRadius, float bottom, float top)
    {
        for (int i = 0; i < segments; i++)
        {
            float angleA = 2 * Mathf.PI * i / segments;
            float angleB = 2 * Mathf.PI * (i + 1) / segments;
            Vector3 outerBottomA = CirclePoint(outerRadius, bottom, angleA);
            Vector3 outerBottomB = CirclePoint(outerRadius, bottom, angleB);
            Vector3 outerTopA = CirclePoint(outerRadius, top, angleA);
            Vector3 outerTopB = CirclePoint(outerRadius, top, angleB);
            Vector3 innerBottomA = CirclePoint(innerRadius, bottom, angleA);
            Vector3 innerBottomB = CirclePoint(innerRadius, bottom, angleB);
            Vector3 innerTopA = CirclePoint(innerRadius, top, angleA);
            Vector3 innerTopB = CirclePoint(innerRadius, top, angleB);
            AddQuad(outerBottomA, outerTopA, outerTopB, outerBottomB);
            AddQuad(innerBottomB, innerTopB, innerTopA, innerBottomA);
            AddQuad(outerTopA, innerTopA, innerTopB, outerTopB);
            AddQuad(outerBottomB, innerBottomB, innerBottomA, outerBottomA);
        }
    }

    Vector3 CirclePoint(float radius, float height, float angle)
    {
        return new Vector3(radius * Mathf.Cos(angle), height, radius * Mathf.Sin(angle));
    }

    void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        int first = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        vertices.Add(d);
        triangles.Add(first);
        triangles.Add(first + 1);
        triangles.Add(first + 2);
        triangles.Add(first);
        triangles.Add(first + 2);
        triangles.Add(first + 3);
        float width = Vector3.Distance(a, d) * 2;
        float height = Vector3.Distance(a, b) * 2;
        uv.Add(new Vector2(0, 0));
        uv.Add(new Vector2(0, height));
        uv.Add(new Vector2(width, height));
        uv.Add(new Vector2(width, 0));
    }

    void BuildWater()
    {
        if (waterSurface == null)
        {
            return;
        }

        if (water == null)
        {
            water = new Mesh
            {
                name = "Mid_Water",
                hideFlags = HideFlags.DontSave
            };
        }

        const int count = 32;
        Vector3[] points = new Vector3[count + 1];
        Vector2[] waterUV = new Vector2[count + 1];
        int[] faces = new int[count * 3];
        points[0] = Vector3.zero;
        waterUV[0] = new Vector2(0.5f, 0.5f);
        for (int i = 0; i < count; i++)
        {
            float angle = 2 * Mathf.PI * i / count;
            points[i + 1] = CirclePoint(0.485f, 0, angle);
            waterUV[i + 1] = new Vector2(0.5f + 0.5f * Mathf.Cos(angle), 0.5f + 0.5f * Mathf.Sin(angle));
            faces[i * 3] = 0;
            faces[i * 3 + 1] = (i + 1) % count + 1;
            faces[i * 3 + 2] = i + 1;
        }

        water.Clear();
        water.vertices = points;
        water.triangles = faces;
        water.uv = waterUV;
        water.RecalculateNormals();
        water.RecalculateBounds();
        waterSurface.sharedMesh = water;
    }

    void OnDestroy()
    {
        Release(wall);
        Release(water);
    }

    void Release(Mesh mesh)
    {
        if (mesh == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            Destroy(mesh);
        }
        else
        {
            DestroyImmediate(mesh);
        }
    }
}
