using System;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Systems : MonoBehaviour
{
    [Header("Loadout & Switching")]
    [Tooltip("Drag in your WeaponConfig assets here, or fill at runtime.")]
    public List<WeaponConfig> loadout = new List<WeaponConfig>();
    [Tooltip("Index into the loadout for the current weapon.")]
    public int currentWeaponIndex = 0;
    [Tooltip("Allow number keys 1..9 to switch weapons?")]
    public bool enableNumberKeySwitching = true;

    [Header("Context (optional but useful)")]
    public Transform firePoint;          // Where projectiles originate
    public Camera playerCamera;          // For raycasts / aim
    public Animator animator;            // For ability anim triggers
    public Rigidbody characterBody;      // For recoil/impulses
    public GameObject owner;             // Typically this.gameObject

    // Fired when an ability is successfully started (after cooldown & validations).
    public event Action<AbilityBase> OnAbilityStarted;

    // Simple input map for classic Input Manager (no new Input System required).
    // Change or extend as you like.
    [Serializable]
    public struct InputBinding
    {
        public string name;          // For inspector readability
        public KeyCode key;          // Keyboard key
        public int mouseButton;      // -1 = ignore; 0/1/2 = mouse buttons
        public bool onHold;          // true = GetKey / GetMouseButton, false = Down
    }

    private WeaponContext _ctx;

    void Awake()
    {
        if (owner == null) owner = gameObject;
        if (playerCamera == null) playerCamera = Camera.main;

        _ctx = new WeaponContext
        {
            owner = owner,
            firePoint = firePoint,
            camera = playerCamera,
            animator = animator,
            body = characterBody != null ? characterBody : GetComponent<Rigidbody>(),
            transform = transform
        };

        ClampWeaponIndex();
    }

    void Update()
    {
        HandleWeaponSwitchKeys();

        WeaponConfig active = GetActiveWeapon();
        if (active == null) return;

        // Poll all bindings for the active weapon.
        foreach (var map in active.bindings)
        {
            if (BindingPressed(map.binding))
            {
                TryUseAbility(map.ability);
            }
        }
    }

    public WeaponConfig GetActiveWeapon()
    {
        if (loadout == null || loadout.Count == 0) return null;
        ClampWeaponIndex();
        return loadout[currentWeaponIndex];
    }

    public void SetActiveWeapon(int index)
    {
        currentWeaponIndex = Mathf.Clamp(index, 0, loadout.Count - 1);
        // You can play a switch sound/animation here.
    }

    public void NextWeapon()
    {
        if (loadout == null || loadout.Count == 0) return;
        currentWeaponIndex = (currentWeaponIndex + 1) % loadout.Count;
    }

    public void PreviousWeapon()
    {
        if (loadout == null || loadout.Count == 0) return;
        currentWeaponIndex = (currentWeaponIndex - 1 + loadout.Count) % loadout.Count;
    }

    /// <summary>Attempts to use an ability (checks null, cooldown, costs).</summary>
    public bool TryUseAbility(AbilityBase ability)
    {
        if (ability == null) return false;

        // Cooldown, ammo, energy, etc., can be validated here:
        if (!ability.CanUse(_ctx))
            return false;

        bool started = ability.Use(_ctx);
        if (started)
            OnAbilityStarted?.Invoke(ability);

        return started;
    }

    private void HandleWeaponSwitchKeys()
    {
        if (!enableNumberKeySwitching || loadout == null) return;
        int max = Mathf.Min(9, loadout.Count);
        for (int i = 0; i < max; i++)
        {
            // Alpha1 = '1' selects index 0, etc.
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SetActiveWeapon(i);
                break;
            }
        }
    }

    private bool BindingPressed(InputBinding b)
    {
        bool keyboardPressed = false;
        bool mousePressed = false;

        if (b.key != KeyCode.None)
            keyboardPressed = b.onHold ? Input.GetKey(b.key) : Input.GetKeyDown(b.key);

        if (b.mouseButton >= 0)
            mousePressed = b.onHold ? Input.GetMouseButton(b.mouseButton) : Input.GetMouseButtonDown(b.mouseButton);

        return keyboardPressed || mousePressed;
    }

    private void ClampWeaponIndex()
    {
        if (loadout == null || loadout.Count == 0)
            currentWeaponIndex = 0;
        else
            currentWeaponIndex = Mathf.Clamp(currentWeaponIndex, 0, loadout.Count - 1);
    }

    // ======= Helpers to assemble bindings in code (optional) =======

    /// <summary>Convenience: create a binding for a mouse button (0=LMB, 1=RMB, 2=MMB).</summary>
    public static InputBinding MouseBinding(string name, int mouseButton, bool hold = false)
    {
        return new InputBinding { name = name, key = KeyCode.None, mouseButton = mouseButton, onHold = hold };
    }

    /// <summary>Convenience: create a binding for a key.</summary>
    public static InputBinding KeyBinding(string name, KeyCode key, bool hold = false)
    {
        return new InputBinding { name = name, key = key, mouseButton = -1, onHold = hold };
    }
}

/// <summary>Context passed to abilities so they can act without tight coupling.</summary>
public struct WeaponContext
{
    public GameObject owner;
    public Transform firePoint;
    public Camera camera;
    public Animator animator;
    public Rigidbody body;
    public Transform transform;
}