using UnityEngine;

public struct DamageInfo
{
    public GameObject attacker;
    public string damageName;
    public float damageAmt;
    public Vector3 hitPoint;
    public Vector3 hitNormal;
    public bool isCrit;
    public DamageInfo(GameObject attacker, string dmgName, float damageAmount, Vector3 hitPoint, Vector3 hitNormal, bool isCritical)
    {
        this.attacker = attacker;
        this.damageName = dmgName;
        this.damageAmt = damageAmount;
        this.hitPoint = hitPoint;
        this.hitNormal = hitNormal;
        this.isCrit = isCritical;
    }
}
