using UnityEngine;

public class MoonRotation : MonoBehaviour
{
    [SerializeField] float duration = 2f;

    void Update()
    {
        float angle = Time.time * 360f / duration;

        transform.localRotation = Quaternion.Euler(0, angle, 0);
    }
}