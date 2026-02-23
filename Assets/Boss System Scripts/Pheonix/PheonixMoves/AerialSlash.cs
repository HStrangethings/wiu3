using System.Collections;
using UnityEngine;

public class AerialSlash : BossMove
{
    private PhoenixBoss boss;

    private GameObject proj;
    private bool spawned;

    private const float LIFE_TIME = 5f;
    private const float DESTROY_GRACE_TIME = 0.15f;

    public AerialSlash(PhoenixBoss boss) : base(boss)
    {
        this.boss = boss;
    }

    public override void Start()
    {
        isFinished = false;
        spawned = false;
        proj = null;

        // Play your aerial slash animation state name here
        // (Must match the Animator STATE node name)
        boss.animator.CrossFadeInFixedTime("AerialSlash", 0.05f, 0);
    }

    public override void Execute()
    {
        // Move ends when animation ends (failsafe)
        AnimatorStateInfo st = boss.animator.GetCurrentAnimatorStateInfo(0);
        if (st.IsName("AerialSlash") && st.normalizedTime >= 0.99f)
            isFinished = true;
    }

    public override void AnimEvent(string evt)
    {
        switch (evt)
        {
            case "spawn":
                SpawnProjectile();
                break;

            case "end":
                isFinished = true;
                break;
        }
    }

    private void SpawnProjectile()
    {
        if (spawned) return;
        spawned = true;

        if (boss.aerialSlashPrefab == null)
        {
            Debug.LogError("[AerialSlash] aerialSlashPrefab not assigned on PhoenixBoss.");
            isFinished = true;
            return;
        }

        Transform sp = boss.aerialSlashSpawnPoint != null ? boss.aerialSlashSpawnPoint : boss.transform;

        // Spawn forward so it doesn't overlap the boss collider
        Vector3 spawnPos = sp.position + sp.forward * 1.2f;
        Quaternion spawnRot = Quaternion.LookRotation(sp.forward, Vector3.up);

        proj = Object.Instantiate(boss.aerialSlashPrefab, spawnPos, spawnRot);

        // Ensure trigger/collision events work reliably
        EnsureRigidbodyExists(proj);

        // Report hits to move machine via BossDamageDealer
        SetupBossHitReporting(proj);

        // Prevent instant self-destroy at spawn
        IgnoreBossCollisions(proj);

        // Temporarily disable DestroyOnTrigger so it can leave spawn overlap
        var dot = proj.GetComponent<DestroyOnTrigger>();
        if (dot != null)
        {
            dot.enabled = false;
            boss.StartCoroutine(ReEnableDestroyOnTrigger(dot, DESTROY_GRACE_TIME));
        }

        // Attach controller that moves + destroys independently of the boss move lifecycle
        var ctrl = proj.GetComponent<AerialSlashProjectileController>();
        if (ctrl == null) ctrl = proj.AddComponent<AerialSlashProjectileController>();
        ctrl.Init(sp.forward, boss.aerialSlashSpeed, LIFE_TIME);

        Debug.Log($"[AerialSlash] Spawned {proj.name} at {spawnPos}");
    }

    private IEnumerator ReEnableDestroyOnTrigger(DestroyOnTrigger dot, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (dot != null) dot.enabled = true;
    }

    private void EnsureRigidbodyExists(GameObject go)
    {
        // For trigger callbacks to be reliable, at least one collider in the interaction should have a Rigidbody.
        var rb = go.GetComponent<Rigidbody>();
        if (rb == null) rb = go.AddComponent<Rigidbody>();

        // We move by script, not physics
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void IgnoreBossCollisions(GameObject projectile)
    {
        var projCols = projectile.GetComponentsInChildren<Collider>(true);
        var bossCols = boss.GetComponentsInChildren<Collider>(true);

        foreach (var pc in projCols)
            foreach (var bc in bossCols)
                if (pc && bc)
                    Physics.IgnoreCollision(pc, bc, true);
    }

    public override BossMove Clone()
    {
        return new AerialSlash(this.boss);
    }
}