using UnityEngine;

public enum ElementType { Physical, Magic }
public enum StatusEffectType { Stun, Burn, Freeze }
public enum AbilityMode { Melee, Ranged }

[System.Serializable]
public struct StatusEffectSpec
{
    public StatusEffectType type;
    [Range(0f, 1f)] public float applyChance;
    [Min(0f)] public float duration;
    // Burn: DPS, Freeze: slow% (0..1), Stun: ignore
    public float potency;
}

// Reusable animation slot with optional duration override
[System.Serializable]
public struct AnimSlot
{
    public AnimationClip clip;

    [Header("Optional Runtime Override")]
    public bool useOverrideDuration;
    [Min(0f)] public float overrideDuration;

    public float GetDuration()
        => useOverrideDuration ? overrideDuration : (clip ? clip.length : 0f);
}

[System.Serializable]
public struct AbilityEntry
{
    [Header("Identity & Trigger")]
    public string name;                  // e.g., "Whirlwind" / "Magic Bolt"
    public AnimationClip clip;           // animation for the ability
    public KeyCode activationKey;        // e.g., Q/E/Mouse1

    [Header("Mode & Combat")]
    public ElementType element;          // Physical / Magic
    [Min(0f)] public float baseDamage;
    [Range(0f, 1f)] public float critChance;

    [Header("Optional Status Effects")]
    public StatusEffectSpec[] statusEffects;

    [Header("Optional Runtime Override")]
    public bool useOverrideDuration;
    [Min(0f)] public float overrideDuration;

    public float GetDuration()
        => useOverrideDuration ? overrideDuration : (clip ? clip.length : 0f);
}

[CreateAssetMenu(menuName = "Game/Weapons/Weapon Preset", fileName = "WPN_NewPreset")]
public class WeaponPreset : ScriptableObject
{
    [Header("Identity")]
    public string id;
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("General Animations")]
    public AnimSlot idle;
    public AnimSlot equip;
    public AnimSlot unequip;

    [Header("Abilities")]
    public AbilityEntry[] abilities;

    // Convenience helpers
    public float GetIdleDuration() => idle.GetDuration();
    public float GetEquipDuration() => equip.GetDuration();
    public float GetUnequipDuration() => unequip.GetDuration();
}