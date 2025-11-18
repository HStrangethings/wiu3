using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAbility", menuName = "Weapons/Abilities/Melee")]
public class MeleeAbility : AbilityBase
{
    public float range = 2f;
    public float radius = 0.4f;
    public int damage = 25;
    public LayerMask hitMask = ~0;

    protected override bool OnUse(WeaponContext ctx)
    {
        Vector3 origin = ctx.firePoint ? ctx.firePoint.position : ctx.transform.position + ctx.transform.forward * 0.5f;
        Vector3 dir = ctx.firePoint ? ctx.firePoint.forward : ctx.transform.forward;

        // Simple sphere cast "hit-scan" melee
        if (Physics.SphereCast(origin, radius, dir, out var hit, range, hitMask, QueryTriggerInteraction.Ignore))
        {
            // Example damage hook
            var dmg = hit.collider.GetComponent<IDamageable>();
            if (dmg != null) dmg.TakeDamage(damage);

            if (ctx.animator) ctx.animator.SetTrigger("Melee");
            return true;
        }

        return false;
    }
}

// Minimal damage interface (optional)
public interface IDamageable
{
    void TakeDamage(int amount);
}
