using UnityEngine;

public class WaterBlastProj : MonoBehaviour
{
    public GameObject impactVFX;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Damageable>(out Damageable dmg))
        {
            Vector3 p = other.ClosestPoint(transform.position);

            // direction from surface point back toward you
            Vector3 dir = (transform.position - p);
            if (dir.sqrMagnitude < 1e-6f) dir = -transform.forward; // fallback
            dir.Normalize();

            // start slightly off the surface, raycast into it
            Vector3 origin = p + dir * 0.05f;

            if (Physics.Raycast(origin, -dir, out RaycastHit hit, 0.2f))
            {
                // use hit data
                Vector3 hitPoint = hit.point;
                Vector3 normal = hit.normal;

                var dmgInfo = new DamageInfo(this.gameObject, 10, hitPoint, normal, false);
                dmg.TryTakeDamage(dmgInfo);

                //Add an effect here, or add an effect on player hit, depends
                Quaternion rot = Quaternion.LookRotation(normal, Vector3.up);
                Instantiate(impactVFX, hitPoint + normal * 0.02f, rot);
            }
        }
        Destroy(gameObject);
    }
}
