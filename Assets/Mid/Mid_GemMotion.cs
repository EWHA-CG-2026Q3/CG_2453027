using UnityEngine;

[RequireComponent(typeof(Mid_GemMesh))]
public class Mid_GemMotion : MonoBehaviour
{
    public float degreesPerSecond = 24f;
    public float floatHeight = .045f;
    public float cycleSeconds = 4f;
    public float phaseSeconds;
    Mesh mesh;
    Vector3[] rest;
    Vector3[] moved;

    void Start()
    {
        Cache();
    }

    void Update()
    {
        ApplyAtTime(Time.timeSinceLevelLoad);
    }

    void OnDisable()
    {
        if (mesh && rest != null)
        {
            mesh.vertices = rest;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }

        mesh = null;
        rest = null;
        moved = null;
    }

    void Cache()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        if (mesh == null)
        {
            GetComponent<Mid_GemMesh>().Rebuild();
            mesh = GetComponent<MeshFilter>().sharedMesh;
        }

        rest = mesh.vertices;
        moved = new Vector3[rest.Length];
    }

    public void ApplyAtTime(float seconds)
    {
        if (mesh == null)
        {
            Cache();
        }

        seconds += phaseSeconds;
        float angle = seconds * degreesPerSecond * Mathf.Deg2Rad;
        float c = Mathf.Cos(angle);
        float s = Mathf.Sin(angle);
        Matrix4x4 rotation = Matrix4x4.identity;
        rotation.m00 = c;
        rotation.m02 = s;
        rotation.m20 = -s;
        rotation.m22 = c;
        Matrix4x4 translation = Matrix4x4.identity;
        translation.m13 = (floatHeight / transform.localScale.y) * (.5f + .5f * Mathf.Sin(seconds * 2 * Mathf.PI / cycleSeconds));
        Matrix4x4 model = translation * rotation;
        for (int i = 0; i < rest.Length; i++)
        {
            moved[i] = model.MultiplyPoint3x4(rest[i]);
        }

        mesh.vertices = moved;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
