using UnityEngine;

public struct DamageInfo
{
    public GameObject attacker;
    public float damageAmt;
    public Vector3 hitPoint;
    public Vector3 hitNormal;
    public bool isCrit;
    public DamageInfo(GameObject attacker, float damageAmount, Vector3 hitPoint, Vector3 hitNormal, bool isCritical)
    {
        this.attacker = attacker;
        this.damageAmt = damageAmount;
        this.hitPoint = hitPoint;
        this.hitNormal = hitNormal;
        this.isCrit = isCritical;
    }
}
