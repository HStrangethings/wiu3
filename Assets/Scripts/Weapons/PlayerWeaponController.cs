using UnityEngine;
using System.Collections.Generic;

public class PlayerWeaponController : MonoBehaviour
{
    [Header("Weapon Selection")]
    public WeaponDatabase database;
    public string weaponPresetId;
    public WeaponPreset currentPreset;

    [Header("Animation")]
    public Animator animator;

    [Tooltip("Your base AnimatorController (the one with the placeholder clip in it).")]
    public RuntimeAnimatorController baseController;

    [Tooltip("The clip inside the base controller that will be replaced at runtime.")]
    public AnimationClip abilityPlaceholderClip;

    [Tooltip("Animator state name to play when an ability is used (must exist in the controller).")]
    public string abilityStateName = "Ability";

    [Header("Ranged")]
    public Transform projectileSpawnPoint;

    [Header("Behavior")]
    public bool lockoutDuringAbility = true;

    AnimatorOverrideController overrideController;
    readonly Dictionary<int, float> cooldownUntil = new();
    float busyUntil = 0f;

    void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        SetupAnimatorOverride();
        EquipById(weaponPresetId);
    }

    void Update()
    {
        if (!currentPreset || currentPreset.abilities == null) return;

        // Optional lockout
        if (lockoutDuringAbility && Time.time < busyUntil) return;

        for (int i = 0; i < currentPreset.abilities.Length; i++)
        {
            var ability = currentPreset.abilities[i];
            if (ability.activationKey == KeyCode.None) continue;

            if (Input.GetKeyDown(ability.activationKey))
            {
                TryUseAbility(i);
            }
        }
    }

    public void EquipById(string id)
    {
        weaponPresetId = id;

        if (!database)
        {
            Debug.LogWarning("PlayerWeaponController: No WeaponDatabase assigned.");
            currentPreset = null;
            return;
        }

        currentPreset = database.GetById(id);
        if (!currentPreset)
            Debug.LogWarning($"PlayerWeaponController: Weapon preset not found for id '{id}'.");
    }

    public void TryUseAbility(int abilityIndex)
    {
        if (!currentPreset) return;
        if (abilityIndex < 0 || abilityIndex >= currentPreset.abilities.Length) return;

        // cooldown check
        if (cooldownUntil.TryGetValue(abilityIndex, out var until) && Time.time < until)
            return;

        var a = currentPreset.abilities[abilityIndex];

        // Animation
        PlayAbilityAnimation(a.clip);

        // If ranged, spawn projectile
        if (a.mode == AbilityMode.Ranged)
            SpawnProjectile(a);

        // set cooldown
        float cd = Mathf.Max(0f, a.useOverrideDuration ? 0f : 0f); // not tying to duration; cooldown is separate if you add it later
        // NOTE: your current AbilityEntry doesn't include cooldown; if you want it, add it back and use it here.
        // For now, no cooldown field. If you do have one in your struct, replace this with: float cd = Mathf.Max(0f, a.cooldown);

        // Basic lockout using duration (clip length or override)
        if (lockoutDuringAbility)
        {
            float dur = a.GetDuration();
            busyUntil = Time.time + Mathf.Max(0f, dur);
        }

        // If you later add a cooldown field, set:
        // cooldownUntil[abilityIndex] = Time.time + cd;
    }

    void SpawnProjectile(AbilityEntry a)
    {
        if (!a.projectilePrefab)
        {
            Debug.LogWarning($"Ability '{a.name}' is Ranged but has no projectilePrefab assigned.");
            return;
        }
        if (!projectileSpawnPoint)
        {
            Debug.LogWarning("PlayerWeaponController: No projectileSpawnPoint assigned.");
            return;
        }

        var go = Instantiate(a.projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);

        // Optional: pass damage/crit/element/status to projectile if it has a receiver
        //var receiver = go.GetComponent<ProjectilePayloadReceiver>();
        //if (receiver)
        //    receiver.SetPayload(a.baseDamage, a.critChance, a.element, a.statusEffects);
    }

    void SetupAnimatorOverride()
    {
        if (!animator || !baseController || !abilityPlaceholderClip) return;

        overrideController = new AnimatorOverrideController(baseController);
        animator.runtimeAnimatorController = overrideController;
    }

    void PlayAbilityAnimation(AnimationClip clip)
    {
        if (!animator) return;

        // If you didn’t set up override, just try to play the state anyway
        if (!overrideController || !abilityPlaceholderClip || !clip)
        {
            if (!string.IsNullOrEmpty(abilityStateName))
                animator.Play(abilityStateName, 0, 0f);
            return;
        }

        overrideController[abilityPlaceholderClip] = clip;

        if (!string.IsNullOrEmpty(abilityStateName))
            animator.Play(abilityStateName, 0, 0f);
    }
}