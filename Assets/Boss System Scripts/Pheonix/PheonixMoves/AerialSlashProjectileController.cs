using UnityEngine;

public class AerialSlashProjectileController : MonoBehaviour
{
    private Vector3 dir;
    private float speed;
    private float lifeTime;
    private float t;

    public void Init(Vector3 forwardDir, float projectileSpeed, float projectileLifeTime)
    {
        dir = forwardDir.sqrMagnitude < 0.0001f ? transform.forward : forwardDir.normalized;
        speed = projectileSpeed;
        lifeTime = projectileLifeTime;
        t = 0f;
    }

    private void Update()
    {
        transform.position += dir * speed * Time.deltaTime;

        t += Time.deltaTime;
        if (t >= lifeTime)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        Debug.Log($"[AerialSlashProjectileController] Destroyed t={Time.time:F2} pos={transform.position}");
    }
}
