using UnityEngine;

public class AerialSlashProjectile : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5f;

    private Vector3 direction;
    private float speed;
    private float timer;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Init(Vector3 dir, float spd)
    {
        direction = dir.normalized;
        speed = spd;

        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity = direction * speed;
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
            return;
        }

        // If no Rigidbody, move manually
        if (rb == null)
        {
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}