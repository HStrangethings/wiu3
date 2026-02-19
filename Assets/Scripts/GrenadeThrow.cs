using UnityEngine;

public class GrenadeThrow : MonoBehaviour
{
    [Header("References")]
    public GameObject grenadePrefab;
    public Transform throwPoint;

    [Header("Throw Settings")]
    public float throwForce = 12f;
    public float upwardForce = 5f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            ThrowGrenade();
        }
    }

    void ThrowGrenade()
    {
        GameObject grenade = Instantiate(grenadePrefab, throwPoint.position, throwPoint.rotation);

        Rigidbody rb = grenade.GetComponent<Rigidbody>();

        // arc
        Vector3 throwDirection = throwPoint.forward * throwForce + Vector3.up * upwardForce;

        rb.AddForce(throwDirection, ForceMode.Impulse);

        
        Collider grenadeCol = grenade.GetComponent<Collider>();
        Collider playerCol = GetComponent<Collider>();

        if (grenadeCol != null && playerCol != null)
        {
            Physics.IgnoreCollision(grenadeCol, playerCol);
        }
    }
}
