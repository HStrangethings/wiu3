using System.Collections;
using UnityEngine;

public class PhoenixBoss : BossBehaviour
{
    [Header("Melee Movement (Kinematic)")]
    public AnimationCurve meleeMoveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public float meleeLiftHeight = 2.0f;
    public float meleeDashDistance = 6.0f;
    public float meleeMoveDuration = 0.8f;   // total time for lift+dash
    [Range(0.05f, 0.95f)]
    public float meleeLiftPortion = 0.3f;    // how much of duration is "lift" phase

    //[Header("Melee Landing")]
    //public float meleeLandingOffset = 0f;


    [Header("Melee Hitboxes (Drag leg colliders here)")]
    public Collider[] legHitboxes;

    [Header("Laser")]
    public GameObject laserBeam;
    public Transform laserSpawnPoint;     // empty GO at beak
    public float laserFreezeDelay = 0.5f; // wait after event before freezing
    public float laserFreezeDuration = 2.0f;

    [Header("Aerial Slash")]
    public GameObject aerialSlashPrefab;
    public Transform aerialSlashSpawnPoint;
    public float aerialSlashSpeed = 25f;

    [Header("Phase 2 Settings")]
    public float phase2Threshold = 0.5f; // 50%
    public float phase2SpeedMult = 1.5f;

    private bool phase2Active = false;
    private bool isFrozen = false;
    private Coroutine laserFreezeRoutine;

    public override void Start()
    {
        base.Start();

        // States
        sm.AddState(new PhoenixIdle(sm, this));
        sm.AddState(new PhoenixAttackMelee(sm, this));
        sm.AddState(new PhoenixAttackLaser(sm, this));
        sm.AddState(new PhoenixAerialSlash(sm, this));

        // Moves
        mm.AddMove("EnergySlash", new EnergySlash(this, 0f, 0)); // uses Melee animation + leg hitboxes
        mm.AddMove("LaserBeam", new LaserBeam(this, 3f));        // plays LaserBeamAnim + freeze-spawn logic
        mm.AddMove("AerialSlash", new AerialSlash(this, aerialSlashSpeed));

        // If you still want your other moves, keep them too:
        // mm.AddMove("TripleSlash", new TripleSlash(this, 50, 0));
        // mm.AddMove("CrossEnergySlash", new EnergySlash(this, 50, 1));
        // mm.AddMove("TripleCrossEnergySlash", new TripleSlash(this, 50, 1));

        sm.Initialize<PhoenixIdle>();

        // Apply initial speed (Phase 1)
        ApplyDesiredAnimatorSpeedIfNotFrozen();
    }

    private void Update()
    {
        TryEnterPhase2();
    }

    private void TryEnterPhase2()
    {
        if (phase2Active) return;
        if (boss == null) return;
        if (boss.baseHealth <= 0f) return;

        // You can keep boss.health01 updated if you want
        boss.RecalcHealth01();

        // Phase 2 when <= 50%
        if ((boss.health / boss.baseHealth) <= phase2Threshold)
        {
            phase2Active = true;
            ApplyDesiredAnimatorSpeedIfNotFrozen();
        }
    }

    private float GetDesiredAnimatorSpeed()
    {
        return phase2Active ? phase2SpeedMult : 1f;
    }

    private void ApplyDesiredAnimatorSpeedIfNotFrozen()
    {
        if (animator == null) return;
        if (isFrozen) return;

        animator.speed = GetDesiredAnimatorSpeed();
    }

    // =========================
    // ANIMATION EVENT BRIDGES
    // =========================

    // ---- Laser freeze event ----
    // Put an Animation Event on LaserBeamAnim clip calling AE_LaserFreeze
    public void AE_LaserFreeze()
    {
        if (laserFreezeRoutine != null)
            StopCoroutine(laserFreezeRoutine);

        laserFreezeRoutine = StartCoroutine(LaserFreezeRoutine(laserFreezeDelay, laserFreezeDuration));
    }

    private IEnumerator LaserFreezeRoutine(float delay, float freezeDuration)
    {
        // Wait before freezing (animation continues during this delay)
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        // Freeze
        isFrozen = true;
        float prevSpeed = animator != null ? animator.speed : 1f;
        if (animator != null) animator.speed = 0f;

        // Spawn laser while frozen (calls LaserBeam.AnimEvent("start"))
        mm.currentMove?.AnimEvent("start");

        // Stay frozen
        if (freezeDuration > 0f)
            yield return new WaitForSeconds(freezeDuration);

        // Despawn laser + finish move (LaserBeam.AnimEvent("end") sets isFinished)
        mm.currentMove?.AnimEvent("end");

        // Resume animation at correct phase speed
        isFrozen = false;
        ApplyDesiredAnimatorSpeedIfNotFrozen();

        laserFreezeRoutine = null;
    }

    // ---- Melee events (put on Melee clip) ----
    public void AE_MeleeStartMove() => mm.currentMove?.AnimEvent("startMove");
    public void AE_MeleeHitOn() => mm.currentMove?.AnimEvent("hitOn");
    public void AE_MeleeHitOff() => mm.currentMove?.AnimEvent("hitOff");
    public void AE_MeleeEnd() => mm.currentMove?.AnimEvent("end");
    public void AE_MeleeComboCheck() => mm.currentMove?.AnimEvent("comboCheck");

    

    // ---- AerialSlash events (put on AerialSlashAnim clip) ----
    public void AE_AerialSlashSpawn() => mm.currentMove?.AnimEvent("spawn");
    public void AE_AerialSlashEnd() => mm.currentMove?.AnimEvent("end");

    private void OnDisable()
    {
        // Safety: if disabled mid-freeze
        if (animator != null) animator.speed = 1f;
        isFrozen = false;
    }
}