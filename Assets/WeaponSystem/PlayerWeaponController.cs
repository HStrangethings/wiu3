using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
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
    public float crossFadeTime = 5f;

    [Header("Ranged")]
    public Transform projectileSpawnPoint;

    [Header("Behaviour")]
    public bool lockoutDuringAbility = true;

    float busyUntil = 0f;



    public List<ComboAction> combo = new List<ComboAction>();
    int playIndex = 0;
    bool comboPlaying = false;
    public event Action onAttackPlay;

    void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        controller = GetComponentInParent<PlayerController>();
    }

    void Update()
    {
        if (!currentPreset || currentPreset.abilities == null) return;
        
        //if (lockoutDuringAbility && Time.time < busyUntil) return;

        if (controller.GetCanMove() == false && Time.time >= busyUntil)
        {
            controller.ToggleCanMove(true);
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryQueueAttack();
        }

        for (int i = 0; i < currentPreset.abilities.Length; i++)
        {
            var a = currentPreset.abilities[i];
            if (a.activationKey == KeyCode.None) continue;

            if (Input.GetKeyDown(a.activationKey))
            {
                TryQueueAbility(i);
                break;
            }
        }
    }

    public void TryQueueAttack()
    {
        if (!canAttack) return;
        if (combo.Count == 0)
        {
            combo.Add(new ComboAction
            {
                type = ComboType.attack,
                attackSlot = currentPreset.attackSlot[GetAttackInComboCount()]
            });
            Debug.Log("added step");
            StartCoroutine(PlayCombo());
            return;
        }

        if (combo.Count > playIndex + 1)
            return;

        if (IsCurrentAnimationReadyForNextStep())
        {
            if (GetAttackInComboCount() >= 3) return;
            combo.Add(new ComboAction
            {
                type = ComboType.attack,
                attackSlot = currentPreset.attackSlot[GetAttackInComboCount()]
            });
            Debug.Log("added step");
        }
    }

    public void TryQueueAbility(int i)
    {
        if (!canAttack) return;

        combo.Add(new ComboAction
        {
            type = ComboType.ability,
            abilityEntry = currentPreset.abilities[i]
        });

        // 1 because we just added 1, this is to check if combo just started
        if (combo.Count == 1)
        {
            StartCoroutine(PlayCombo());
        }
    }

    public IEnumerator PlayCombo()
    {
        if (comboPlaying) yield break;
        comboPlaying = true;

        // Safety: nothing to play
        if (combo.Count == 0)
        {
            comboPlaying = false;
            yield break;
        }

        playIndex = 0;

        // ---- Start by playing the first action if it's not already playing ----
        {
            var first = combo[playIndex];
            string firstState = GetStateName(first);

            if (!string.IsNullOrWhiteSpace(firstState) && animator)
            {
                animator.CrossFadeInFixedTime(firstState, crossFadeTime, 0, 0f);
                yield return StartCoroutine(WaitForStateToStart(firstState));
                onAttackPlay?.Invoke();
            }

            // Optional movement lockout per action
            if (lockoutDuringAbility)
            {
                float dur = GetDuration(first);
                busyUntil = Time.time + Mathf.Max(0f, dur);
                if (controller) controller.ToggleCanMove(false);
            }
        }

        // ---- Main loop: while there are queued actions left ----
        while (playIndex < combo.Count)
        {
            // 1) Is there an animation playing right now? If there is, continue
            //    (In practice: we just wait until the CURRENT action's state reaches 90%.)
            var currentAction = combo[playIndex];
            string currentState = GetStateName(currentAction);

            // If the animator isn't in the expected state yet (crossfade / transition),
            // don't advance; just keep waiting.
            if (!IsStatePast(currentState))
            {
                yield return null;
                continue;
            }

            // 2) Is current animation 90% done? If it is, crossfade to next
            playIndex++;

            // If no next action, end combo
            if (playIndex >= combo.Count)
                break;

            if (currentAction.type == ComboType.ability)
                break;

            var nextAction = combo[playIndex];
            string nextState = GetStateName(nextAction);

            if (!string.IsNullOrWhiteSpace(nextState) && animator)
            {
                animator.CrossFadeInFixedTime(nextState, crossFadeTime, 0, 0f);
                yield return StartCoroutine(WaitForStateToStart(nextState));
                onAttackPlay?.Invoke();
            }

            // Optional movement lockout per action
            if (lockoutDuringAbility)
            {
                float dur = GetDuration(nextAction);
                busyUntil = Time.time + Mathf.Max(0f, dur);
                if (controller) controller.ToggleCanMove(false);
            }

            yield return null;
        }

        // ---- Combo finished: reset ----
        combo.Clear();
        playIndex = 0;
        comboPlaying = false;

        // Optional: re-enable movement at the end (if you want)
        if (controller) controller.ToggleCanMove(true);
    }

    bool IsCurrentAnimationReadyForNextStep()
    {
        if (!animator) return false;
        if (combo.Count == 0) return false;
        if (playIndex < 0 || playIndex >= combo.Count) return false;

        int expectedHash = GetStateHash(combo[playIndex]);
        if (expectedHash == 0) return false;

        var cur = animator.GetCurrentAnimatorStateInfo(0);
        if (cur.shortNameHash == expectedHash && cur.normalizedTime >= 0.7f)
            return true;

        if (animator.IsInTransition(0))
        {
            var nxt = animator.GetNextAnimatorStateInfo(0);
            if (nxt.shortNameHash == expectedHash && nxt.normalizedTime >= 0.7f)
                return true;
        }

        return false;
    }

    int GetStateHash(ComboAction a)
    {
        string name = GetStateName(a);
        if (string.IsNullOrWhiteSpace(name)) return 0;
        return Animator.StringToHash(name);
    }

    bool IsStatePast(string stateName)
    {
        if (string.IsNullOrWhiteSpace(stateName) || !animator) return false;

        var cur = animator.GetCurrentAnimatorStateInfo(0);
        if (cur.IsName(stateName) && cur.normalizedTime >= 0.9f) return true;

        if (animator.IsInTransition(0))
        {
            var nxt = animator.GetNextAnimatorStateInfo(0);
            if (nxt.IsName(stateName) && nxt.normalizedTime >= 0.9f) return true;
        }
        return false;
    }

    string GetStateName(ComboAction a)
    {
        return a.type == ComboType.ability
            ? a.abilityEntry.stateName
            : a.attackSlot.animatorStateName;
    }

    float GetDuration(ComboAction a)
    {
        return a.type == ComboType.ability
            ? a.abilityEntry.GetDuration()
            : a.attackSlot.GetDuration();
    }

    public int GetAttackInComboCount()
    {
        int i = 0;
        foreach(ComboAction action in combo)
        {
            if (action.type == ComboType.attack)
            { i++; }
        }
        return i;
    }

    IEnumerator WaitForStateToStart(string stateName, int layer = 0)
    {
        if (!animator || string.IsNullOrWhiteSpace(stateName))
            yield break;

        // Wait until transition finishes and this becomes the CURRENT state
        while (animator.IsInTransition(layer))
            yield return null;

        // Wait until current state matches
        while (!animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName))
            yield return null;

        // Optional: ensure it actually started playing (sometimes it's 0 on the first frame)
        while (animator.GetCurrentAnimatorStateInfo(layer).normalizedTime <= 0f)
            yield return null;
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