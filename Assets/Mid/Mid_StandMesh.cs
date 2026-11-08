using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Mid_StandMesh : MonoBehaviour
{
    [Range(5, 32)]
    public int sides = 8;
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
                name = "Mid_StandMesh",
                hideFlags = HideFlags.DontSave
            };
        }

        var v = new List<Vector3>();
        var uv = new List<Vector2>();
        var t = new List<int>();
        float[] radius = { .16f, .18f, .18f, .16f };
        float[] height = { 0, .012f, .045f, .06f };
        Vector3[,] rings = new Vector3[4, sides];
        for (int r = 0; r < 4; r++)
        {
            for (int i = 0; i < sides; i++)
            {
                float angle = 2 * Mathf.PI * i / sides;
                rings[r, i] = new Vector3(radius[r] * Mathf.Cos(angle), height[r], radius[r] * Mathf.Sin(angle));
            }
        }

        for (int r = 0; r < 3; r++)
        {
            for (int i = 0; i < sides; i++)
            {
                int j = (i + 1) % sides;
                Triangle(rings[r, i], rings[r + 1, i], rings[r + 1, j], v, uv, t);
                Triangle(rings[r, i], rings[r + 1, j], rings[r, j], v, uv, t);
            }
        }

        for (int i = 0; i < sides; i++)
        {
            int j = (i + 1) % sides;
            Triangle(new Vector3(0, height[3], 0), rings[3, j], rings[3, i], v, uv, t);
            Triangle(Vector3.zero, rings[0, i], rings[0, j], v, uv, t);
        }

        generated.Clear();
        generated.SetVertices(v);
        generated.SetUVs(0, uv);
        generated.SetTriangles(t, 0);
        generated.RecalculateNormals();
        generated.RecalculateTangents();
        generated.RecalculateBounds();
        GetComponent<MeshFilter>().sharedMesh = generated;
    }

    static void Triangle(Vector3 a, Vector3 b, Vector3 c, List<Vector3> v, List<Vector2> uv, List<int> t)
    {
        int start = v.Count;
        v.Add(a);
        v.Add(b);
        v.Add(c);
        t.Add(start);
        t.Add(start + 1);
        t.Add(start + 2);
        uv.Add(new Vector2(a.x, a.z));
        uv.Add(new Vector2(b.x, b.z));
        uv.Add(new Vector2(c.x, c.z));
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
