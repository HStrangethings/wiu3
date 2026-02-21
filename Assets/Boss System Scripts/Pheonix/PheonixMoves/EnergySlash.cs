using UnityEngine;

public class EnergySlash : BossMove
{
    private PhoenixBoss boss;
    private int slashOrCross; // kept for compatibility

    private float timer;
    private bool ended;

    private Vector3 startPos;
    private Vector3 dashDir;

    public EnergySlash(PhoenixBoss boss, float unused, int slashOrCross) : base(boss)
    {
        this.boss = boss;
        this.slashOrCross = slashOrCross;
    }

    public override void Start()
    {
        isFinished = false;
        ended = false;
        timer = 0f;

        startPos = boss.transform.position;

        dashDir = GetFlatDirToPlayer();
        if (dashDir.sqrMagnitude < 0.0001f)
            dashDir = boss.transform.forward;

        SetLegHitboxes(false);

        boss.animator.Play("Melee");
    }

    public override void Execute()
    {
        if (ended) return;

        timer += Time.deltaTime;

        // Optional: rotate while attacking
        boss.transform.rotation = boss.RotateToPlayer();

        ApplyKinematicMeleeMotion(timer);
    }

    public override void End()
    {
        SetLegHitboxes(false);
        base.End();
    }

    private void ApplyKinematicMeleeMotion(float t)
    {
        float duration = Mathf.Max(0.01f, boss.meleeMoveDuration);
        float liftPortion = Mathf.Clamp01(boss.meleeLiftPortion);
        float liftHeight = boss.meleeLiftHeight;
        float dashDistance = boss.meleeDashDistance;

        AnimationCurve curve = boss.meleeMoveCurve != null
            ? boss.meleeMoveCurve
            : AnimationCurve.Linear(0, 0, 1, 1);

        float norm = Mathf.Clamp01(t / duration);

        // Update dash direction during dash phase (optional)
        if (norm >= liftPortion)
        {
            Vector3 newDir = GetFlatDirToPlayer();
            if (newDir.sqrMagnitude > 0.0001f)
                dashDir = newDir;
        }

        Vector3 targetPos;

        if (norm < liftPortion)
        {
            float phaseT = Mathf.Clamp01(norm / Mathf.Max(0.0001f, liftPortion));
            float eased = curve.Evaluate(phaseT);
            targetPos = startPos + Vector3.up * (liftHeight * eased);
        }
        else
        {
            float dashPortion = Mathf.Max(0.0001f, 1f - liftPortion);
            float phaseT = Mathf.Clamp01((norm - liftPortion) / dashPortion);
            float eased = curve.Evaluate(phaseT);

            Vector3 peakPos = startPos + Vector3.up * liftHeight;
            targetPos = peakPos + dashDir * (dashDistance * eased);
        }

        // Kinematic-safe movement:
        // MovePosition is correct for kinematic RBs, but if it still doesn't move in your project,
        // switch to transform.position (commented below).
        if (boss.rb != null)
            boss.rb.MovePosition(targetPos);
        else
            boss.transform.position = targetPos;

        // If MovePosition doesn’t work for your setup, use this instead:
        // boss.transform.position = targetPos;
    }

    private Vector3 GetFlatDirToPlayer()
    {
        if (boss.currPlayer == null) return Vector3.zero;
        Vector3 toPlayer = boss.currPlayer.transform.position - boss.transform.position;
        toPlayer.y = 0f;
        return toPlayer.normalized;
    }

    private void SetLegHitboxes(bool on)
    {
        if (boss.legHitboxes == null) return;
        foreach (var col in boss.legHitboxes)
            if (col != null) col.enabled = on;
    }

    public override void AnimEvent(string evt)
    {
        switch (evt)
        {
            case "hitOn":
                SetLegHitboxes(true);
                break;

            case "hitOff":
                //  ONLY disable hitboxes, DO NOT end the move
                SetLegHitboxes(false);
                break;

            case "end":
                //  End the move here (animation timed)
                SetLegHitboxes(false);
                ended = true;
                isFinished = true;
                break;
        }
    }

    public override BossMove Clone()
    {
        return new EnergySlash(this.boss, 0f, this.slashOrCross);
    }
}