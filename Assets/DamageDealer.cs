using UnityEngine;

public class DamageDealer : MonoBehaviour, ITriggerReceiver
{
    public float damage;
    public string damageName;
    public bool isActive = true;
    public bool hasVFX = false;
    public GameObject impactVFX;

    public void OnTrigger(Collider other)
    {
        if (CheckHit())
        {
            if (other.TryGetComponent<Damageable>(out Damageable dmg))
            {
                ComputeRay(other, out Vector3 origin, out Vector3 dir);

                if (Physics.Raycast(origin, -dir, out RaycastHit hit, 0.2f))
                {
                    // use hit data
                    Vector3 hitPoint = hit.point;
                    Vector3 normal = hit.normal;

                    OnHit(hitPoint, normal, dmg);
                }
            } 
        }
    }

    public virtual bool CheckHit()
    {
        if (isActive)
        {
            return true;
        }
        else { return false; }
    }

    public virtual void ComputeRay(Collider other, out Vector3 origin, out Vector3 dir)
    {
        Vector3 p = other.ClosestPoint(transform.position);

        // direction from surface point back toward you
        dir = (transform.position - p);
        if (dir.sqrMagnitude < 1e-6f) dir = -transform.forward; // fallback
        dir.Normalize();

        // start slightly off the surface, raycast into it
        origin = p + dir * 0.05f;
    }

    public virtual void OnHit(Vector3 hitPoint, Vector3 normal, Damageable dmg)
    {
        var dmgInfo = new DamageInfo(this.gameObject, damageName, damage, hitPoint, normal, false);
        dmg.TryTakeDamage(dmgInfo);


        //Add an effect here, or add an effect on player hit, depends
        Quaternion rot = Quaternion.LookRotation(normal, Vector3.up);
        Instantiate(impactVFX, hitPoint + normal * 0.02f, rot);
    }
}
