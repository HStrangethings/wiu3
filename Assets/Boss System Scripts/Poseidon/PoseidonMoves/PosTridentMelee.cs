using UnityEngine;

public class PosTridentMelee : DamageDealer
{
    public HitboxID hitbox;
    public GameObject impactVFX;

    private void Start()
    {
        isActive = false;
    }

    public override void OnHit(Vector3 hitPoint, Vector3 normal, Damageable dmg)
    {
        var dmgInfo = new DamageInfo(this.gameObject, damageName, damage, hitPoint, normal, false);
        dmg.TryTakeDamage(dmgInfo);

        //Add an effect here, or add an effect on player hit, depends
        Quaternion rot = Quaternion.LookRotation(normal, Vector3.up);
        Instantiate(impactVFX, hitPoint + normal * 0.02f, rot);
        Debug.Log("HIT");
    }
}
