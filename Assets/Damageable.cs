using UnityEngine;

public abstract class Damageable : MonoBehaviour
{
    public float health { get; set; }

    public abstract void TryTakeDamage(DamageInfo info);

    public abstract void Death();
}
