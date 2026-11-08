using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Mid_GemMesh : MonoBehaviour
{
    public enum Shape
    {
        Diamond,
        Pyramid,
        ShearedDiamond,
        CutCrystal
    }

    public Shape shape;
    Mesh generated;

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
        if (generated == null)
        {
            generated = new Mesh
            {
                name = "Mid_" + shape,
                hideFlags = HideFlags.DontSave
            };
        }

        var v = new List<Vector3>();
        var uv = new List<Vector2>();
        var indices = new List<int>();
        Vector3[] points;
        int[] faces;
        if (shape == Shape.CutCrystal)
        {
            BuildCutCrystal();
            return;
        }

        if (shape == Shape.Pyramid)
        {
            points = new[]
            {
                new Vector3(-.5f, 0, -.5f),
                new Vector3(.5f, 0, -.5f),
                new Vector3(.5f, 0, .5f),
                new Vector3(-.5f, 0, .5f),
                new Vector3(0, 1.25f, 0)
            };
            faces = new[]
            {
                0, 1, 2,
                0, 2, 3,
                0, 4, 1,
                1, 4, 2,
                2, 4, 3,
                3, 4, 0
            };
        }
        else
        {
            points = new[]
            {
                new Vector3(0, 0, 0),
                new Vector3(-.5f, .65f, -.5f),
                new Vector3(.5f, .65f, -.5f),
                new Vector3(.5f, .65f, .5f),
                new Vector3(-.5f, .65f, .5f),
                new Vector3(0, 1.35f, 0)
            };
            faces = new[]
            {
                0, 1, 2,
                0, 2, 3,
                0, 3, 4,
                0, 4, 1,
                5, 2, 1,
                5, 3, 2,
                5, 4, 3,
                5, 1, 4
            };
            if (shape == Shape.ShearedDiamond)
            {
                Matrix4x4 shear = Matrix4x4.identity;
                shear.m01 = .42f;
                for (int i = 0; i < points.Length; i++)
                {
                    points[i] = shear.MultiplyPoint3x4(points[i]);
                }
            }
        }

        for (int i = 0; i < faces.Length; i++)
        {
            v.Add(points[faces[i]]);
            indices.Add(i);
            uv.Add(i % 3 == 0 ? new Vector2(.5f, 1) : i % 3 == 1 ? Vector2.zero : Vector2.right);
        }

        generated.Clear();
        generated.SetVertices(v);
        generated.SetTriangles(indices, 0);
        generated.SetUVs(0, uv);
        generated.RecalculateNormals();
        generated.RecalculateTangents();
        generated.RecalculateBounds();
        GetComponent<MeshFilter>().sharedMesh = generated;
    }

    void BuildCutCrystal()
    {
        var vertices = new List<Vector3>();
        var uv = new List<Vector2>();
        var triangles = new List<int>();
        float[] radius = { 0, .48f, .34f, .20f };
        float[] height = { 0, .52f, .94f, 1.12f };
        const int sides = 8;
        for (int ring = 0; ring < 3; ring++)
        {
            for (int i = 0; i < sides; i++)
            {
                int j = (i + 1) % sides;
                Vector3 a = CrystalPoint(radius[ring], height[ring], i, sides);
                Vector3 b = CrystalPoint(radius[ring + 1], height[ring + 1], i, sides);
                Vector3 c = CrystalPoint(radius[ring + 1], height[ring + 1], j, sides);
                Vector3 d = CrystalPoint(radius[ring], height[ring], j, sides);
                CrystalTriangle(a, b, c, vertices, uv, triangles);
                if (ring > 0)
                {
                    CrystalTriangle(a, c, d, vertices, uv, triangles);
                }
            }
        }

        for (int i = 0; i < sides; i++)
        {
            CrystalTriangle(
                new Vector3(0, height[3], 0),
                CrystalPoint(radius[3], height[3], (i + 1) % sides, sides),
                CrystalPoint(radius[3], height[3], i, sides),
                vertices,
                uv,
                triangles
            );
        }

        generated.Clear();
        generated.SetVertices(vertices);
        generated.SetTriangles(triangles, 0);
        generated.SetUVs(0, uv);
        generated.RecalculateNormals();
        generated.RecalculateTangents();
        generated.RecalculateBounds();
        GetComponent<MeshFilter>().sharedMesh = generated;
    }

    static Vector3 CrystalPoint(float r, float y, int i, int sides)
    {
        float angle = i * 2 * Mathf.PI / sides;
        return new Vector3(r * Mathf.Cos(angle), y, r * Mathf.Sin(angle));
    }

    static void CrystalTriangle(Vector3 a, Vector3 b, Vector3 c, List<Vector3> v, List<Vector2> uv, List<int> t)
    {
        int n = v.Count;
        v.Add(a);
        v.Add(b);
        v.Add(c);
        t.Add(n);
        t.Add(n + 1);
        t.Add(n + 2);
        uv.Add(new Vector2(0, 0));
        uv.Add(new Vector2(.5f, 1));
        uv.Add(new Vector2(1, 0));
    }

    void OnDestroy()
    {
        if (generated != null)
        {
            if (Application.isPlaying)
            {
                Destroy(generated);
            }
            else
            {
                DestroyImmediate(generated);
            }
        }
    }
}
