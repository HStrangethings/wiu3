using UnityEngine;

public class ProjectileLifetime : MonoBehaviour
{
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Destroy on hit (you can add damage logic here later)
        Destroy(gameObject);
    }
}