using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileAbility", menuName = "Weapons/Abilities/Projectile")]
public class ProjectileAbility : AbilityBase
{
    public GameObject projectilePrefab;
    public float muzzleVelocity = 40f;
    public float spreadDegrees = 0f;

    protected override bool OnUse(WeaponContext ctx)
    {
        if (projectilePrefab == null || ctx.firePoint == null) return false;

        // Aim direction (firePoint forward preferred fall back to camera forward)
        Vector3 dir = (ctx.firePoint != null ? ctx.firePoint.forward :
                      (ctx.camera != null ? ctx.camera.transform.forward : ctx.transform.forward));

        // Apply simple spread
        if (spreadDegrees > 0f)
        {
            dir = Quaternion.Euler(
                Random.Range(-spreadDegrees, spreadDegrees),
                Random.Range(-spreadDegrees, spreadDegrees),
                0f) * dir;
        }

        GameObject proj = Instantiate(projectilePrefab, ctx.firePoint.position, Quaternion.LookRotation(dir));

        if (proj.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = dir.normalized * muzzleVelocity;
        }

        // Optional  play animation
        if (ctx.animator) ctx.animator.SetTrigger("Fire");

        return true;
    }
}