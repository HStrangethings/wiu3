using UnityEngine;

public class EnergySlash : BossMove
{
    private PhoenixBoss boss;
    private int slashOrCross; // kept for compatibility

    private bool moveActive;
    private float t;

    private Vector3 startPos;
    private Vector3 dashDir;   // LOCKED at startMove
    private float baseY;

    public EnergySlash(PhoenixBoss boss, float unused, int slashOrCross) : base(boss)
    {
        this.boss = boss;
        this.slashOrCross = slashOrCross;
    }

    public override void Start()
    {
        isFinished = false;

        moveActive = false;
        t = 0f;

        SetLegHitboxes(false);

        // Play melee animation (Animator STATE name must be exactly "Melee")
        boss.animator.CrossFadeInFixedTime("Melee", 0.05f);

        // Movement will start ONLY when the animation event calls AE_MeleeStartMove
    }

    public override void Execute()
    {
        // Optional: rotate during attack (remove if you want it locked)
        boss.transform.rotation = boss.RotateToPlayer();

        // Drive movement while active
        if (moveActive)
        {
            t += Time.deltaTime;

            Vector3 nextPos = ComputeMeleePos(t);

            //  Since rb is kinematic and you want reliable motion each frame:
            // Move transform directly (no physics-step delay, no teleport-at-end).
            boss.transform.position = nextPos;

            // Stop moving after duration (animation can continue)
            if (t >= Mathf.Max(0.05f, boss.meleeMoveDuration))
                moveActive = false;
        }

        // End move when the animation ends (safety, so you don't get stuck)
        AnimatorStateInfo st = boss.animator.GetCurrentAnimatorStateInfo(0);
        if (st.IsName("Melee") && st.normalizedTime >= 0.99f)
        {
            SetLegHitboxes(false);
            moveActive = false;
            isFinished = true;
        }
    }

    public override void End()
    {
        SetLegHitboxes(false);
        moveActive = false;
        base.End();
    }

    private Vector3 ComputeMeleePos(float elapsed)
    {
        float duration = Mathf.Max(0.05f, boss.meleeMoveDuration);
        float liftPortion = Mathf.Clamp01(boss.meleeLiftPortion);

        float norm = Mathf.Clamp01(elapsed / duration);

        AnimationCurve curve = boss.meleeMoveCurve != null
            ? boss.meleeMoveCurve
            : AnimationCurve.Linear(0, 0, 1, 1);

        if (norm < liftPortion)
        {
            // ----- LIFT UP -----
            float phaseT = Mathf.Clamp01(norm / Mathf.Max(0.0001f, liftPortion));
            float eased = curve.Evaluate(phaseT);

            float y = baseY + boss.meleeLiftHeight * eased;
            return new Vector3(startPos.x, y, startPos.z);
        }
        else
        {
            // ----- DASH FORWARD + DOWN (diagonal) -----
            float dashPortion = Mathf.Max(0.0001f, 1f - liftPortion);
            float phaseT = Mathf.Clamp01((norm - liftPortion) / dashPortion);
            float eased = curve.Evaluate(phaseT);

            // XZ moves forward
            Vector3 dashOffset = dashDir * (boss.meleeDashDistance * eased);

            // Y comes DOWN from peakY -> baseY
            float peakY = baseY + boss.meleeLiftHeight;
            float y = Mathf.Lerp(peakY, baseY, eased);

            Vector3 peakPos = new Vector3(startPos.x, peakY, startPos.z);
            Vector3 pos = peakPos + dashOffset;
            pos.y = y;

            return pos;
        }
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

        foreach (var c in boss.legHitboxes)
            if (c != null) c.enabled = on;
    }

    public override void AnimEvent(string evt)
    {
        switch (evt)
        {
            case "startMove":
                {
                    // LOCK direction at the moment movement begins
                    startPos = boss.transform.position;
                    baseY = startPos.y;

                    dashDir = GetFlatDirToPlayer();
                    if (dashDir.sqrMagnitude < 0.0001f)
                        dashDir = boss.transform.forward;

                    t = 0f;
                    moveActive = true;
                    break;
                }

            case "hitOn":
                SetLegHitboxes(true);
                break;

            case "hitOff":
                SetLegHitboxes(false);
                break;

            case "end":
                // Optional explicit end (if you add an event at end of clip)
                SetLegHitboxes(false);
                moveActive = false;
                isFinished = true;
                break;
        }
    }

    public override BossMove Clone()
    {
        return new EnergySlash(this.boss, 0f, this.slashOrCross);
    }
}