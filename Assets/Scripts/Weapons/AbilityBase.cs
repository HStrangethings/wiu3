using UnityEngine;

public abstract class AbilityBase : ScriptableObject
{
    [Header("Meta")]
    public string displayName = "Ability";
    [TextArea] public string description;

    [Header("Timing")]
    public float cooldown = 0.25f;
    private float _lastUseTime = -999f;

    [Header("Costs (optional)")]
    public int ammoCost = 0;
    public float energyCost = 0f;

    public virtual bool CanUse(WeaponContext ctx)
    {
        // Cooldown gate
        if (Time.time < _lastUseTime + cooldown) return false;
        // Add your own ammo energy checks here, reading from ctx.owner, etc.
        return true;
    }

    public bool Use(WeaponContext ctx)
    {
        if (!CanUse(ctx)) return false;
        bool ok = OnUse(ctx);
        if (ok) _lastUseTime = Time.time;
        return ok;
    }

    /// <summary> Do the actual ability logic here.</summary>
    protected abstract bool OnUse(WeaponContext ctx);
}
