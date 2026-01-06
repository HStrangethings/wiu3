using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [Header("Weapon Selection")]
    public WeaponDatabase database;
    public string weaponPresetId;
    public WeaponPreset currentPreset;

    [Header("Animator")]
    public Animator animator;

    [Tooltip("Small blend time when switching to ability states.")]
    public float crossFadeTime = 0.05f;

    [Header("Ranged")]
    public Transform projectileSpawnPoint;

    [Header("Behaviour")]
    public bool lockoutDuringAbility = true;

    float busyUntil = 0f;

    void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        EquipById(weaponPresetId);
    }

    void Update()
    {
        if (!currentPreset || currentPreset.abilities == null) return;

        if (lockoutDuringAbility && Time.time < busyUntil) return;

        for (int i = 0; i < currentPreset.abilities.Length; i++)
        {
            var a = currentPreset.abilities[i];
            if (a.activationKey == KeyCode.None) continue;

            if (Input.GetKeyDown(a.activationKey))
            {
                UseAbility(i);
                break;
            }
        }
    }

    public void EquipById(string id)
    {
        weaponPresetId = id;

        if (!database)
        {
            Debug.LogError("PlayerWeaponController: WeaponDatabase not assigned.");
            currentPreset = null;
            return;
        }

        currentPreset = database.GetById(id);

        if (!currentPreset)
            Debug.LogError($"PlayerWeaponController: No WeaponPreset found for id '{id}' (case-sensitive).");
        else
            Debug.Log($"Equipped: {currentPreset.displayName} (id={currentPreset.id})");
    }

    void UseAbility(int index)
    {
        if (!currentPreset) return;
        if (index < 0 || index >= currentPreset.abilities.Length) return;

        var a = currentPreset.abilities[index];

        // Determine which animator state to play
        // Priority: animatorStateName -> clip.name (fallback)
        string stateName = null;

        // If you added animatorStateName to AbilityEntry:
        var stateField = a.GetType().GetField("animatorStateName");
        if (stateField != null)
            stateName = stateField.GetValue(a) as string;

        if (string.IsNullOrWhiteSpace(stateName))
            stateName = a.clip ? a.clip.name : null;

        if (string.IsNullOrWhiteSpace(stateName))
        {
            Debug.LogWarning($"Ability '{a.name}' has no animatorStateName and no clip assigned.");
            return;
        }

        // Play animation state by name (no transitions needed)
        if (animator)
        {
            // Works even without transitions; it jumps into that state directly.
            animator.CrossFade(stateName, crossFadeTime, 0, 0f);
        }
        else
        {
            Debug.LogWarning("PlayerWeaponController: No Animator assigned/found.");
        }

        // Spawn projectile if ranged
        if (a.mode == AbilityMode.Ranged)
            SpawnProjectile(a);

        // Optional lockout using your duration override or clip length
        if (lockoutDuringAbility)
        {
            float dur = a.GetDuration(); // uses override if enabled, else clip length
            busyUntil = Time.time + Mathf.Max(0f, dur);
        }

        Debug.Log($"Ability '{a.name}' -> Played Animator state '{stateName}' (mode={a.mode})");
    }

    void SpawnProjectile(AbilityEntry a)
    {
        if (!a.projectilePrefab)
        {
            Debug.LogWarning($"Ability '{a.name}' is Ranged but projectilePrefab is empty.");
            return;
        }
        if (!projectileSpawnPoint)
        {
            Debug.LogWarning("PlayerWeaponController: projectileSpawnPoint is not assigned.");
            return;
        }

        var go = Instantiate(a.projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);

        // Optional payload (if your projectile uses it)
        //var receiver = go.GetComponent<ProjectilePayloadReceiver>();
        //if (receiver)
        //    receiver.SetPayload(a.baseDamage, a.critChance, a.element, a.statusEffects);
    }
}