using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    public WeaponPreset currentPreset;
    public HitboxManager hitboxManager;
    public bool isAttacking = false;
    private bool canAttack = true;
    private PlayerController controller;

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
        controller = GetComponentInParent<PlayerController>();
    }

    void Update()
    {
        if (!currentPreset || currentPreset.abilities == null) return;
        
        if (lockoutDuringAbility && Time.time < busyUntil) return;

        if (controller.GetCanMove() == false)
        {
            controller.ToggleCanMove(true);
        }

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

    void UseAbility(int index)
    {
        if (!canAttack) return;
        if (!currentPreset) return;
        if (index < 0 || index >= currentPreset.abilities.Length) return;

        var a = currentPreset.abilities[index];

        // Determine which animator state to play
        // Priority: animatorStateName -> clip.name (fallback)
        string stateName = null;

        // If you added animatorStateName to AbilityEntry:
        //?
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

        AbilityFunction();
        isAttacking = true;

        if (lockoutDuringAbility)
        {
            float dur = a.GetDuration(); // uses override if enabled, else clip length
            busyUntil = Time.time + Mathf.Max(0f, dur);
            controller.ToggleCanMove(false);
        }
    }

    //call this in animations, and also when stunned or whenever attack animation gets cut
    public void StopAttacking()
    {
        isAttacking = false;
    }

    public void StunnedAttack()
    {
        isAttacking = false;
        canAttack = false;
        busyUntil = 0;
    }

    public void RecoverAttack()
    {
        canAttack = true;
    }

    //wtv monobehaviour function u might need per ability, eg.spawning projectile
    public void AbilityFunction()
    {
        switch (currentPreset.name)
        {
            case "sword":
                break;
        }
    }

    public void EnableHitbox(HitboxID id)
    {
        if (hitboxManager && id)
            hitboxManager.SetGroup(id, true);
    }

    // Called by Animation Event
    public void DisableHitbox(HitboxID id)
    {
        if (hitboxManager && id)
            hitboxManager.SetGroup(id, false);
    }
}