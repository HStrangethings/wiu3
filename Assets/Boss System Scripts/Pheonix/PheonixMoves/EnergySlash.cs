using UnityEngine;

public class EnergySlash : BossMove
{
    private PhoenixBoss boss;
    private int slashOrCross;

    private bool moveActive;
    private float t;

    private Vector3 startPos;
    private Vector3 dashDir;   // locked at startMove
    private float baseY;

    // landing push window
    private bool landingPushActive;
    private float landingPushTimer;

    // tune these in PhoenixBoss if you want; keeping defaults here
    private const float LANDING_PUSH_DURATION = 0.15f;   // how long to keep pushing after landing event
    private const float LANDING_PUSH_RADIUS = 2.5f;      // match your big feet area radius feel
    private const float LANDING_PUSH_SPEED = 25f;

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

        landingPushActive = false;
        landingPushTimer = 0f;

        // make sure hitboxes are off at start
        // (your legs colliders system OR hitbox manager group, depending on your setup)
        SetLegHitboxes(false);
        SafeSetGroup(boss.shockwave, false);

        boss.animator.CrossFadeInFixedTime("Melee", 0.05f);
    }

    public override void Execute()
    {
        boss.rb.linearVelocity = Vector3.zero;
        boss.transform.rotation = boss.RotateToPlayer();

        // movement
        if (moveActive)
        {
            t += Time.deltaTime;

            Vector3 nextPos = ComputeMeleePos(t);
            boss.transform.position = nextPos;

            if (t >= Mathf.Max(0.05f, boss.meleeMoveDuration))
                moveActive = false;
        }

        // landing push (short window)
        if (landingPushActive)
        {
            landingPushTimer += Time.deltaTime;

            var cc = boss.currPlayer != null ? boss.currPlayer.GetComponent<CharacterController>() : null;
            if (cc != null)
            {
                PushCCSideways(cc, boss.transform.position, LANDING_PUSH_RADIUS, LANDING_PUSH_SPEED);
            }

            if (landingPushTimer >= LANDING_PUSH_DURATION)
                landingPushActive = false;
        }

        // end when animation ends (failsafe)
        AnimatorStateInfo st = boss.animator.GetCurrentAnimatorStateInfo(0);
        if (st.IsName("Melee") && st.normalizedTime >= 0.99f)
        {
            CleanupAndFinish();
        }
    }

    public override void End()
    {
        CleanupAndFinish();
        base.End();
    }

    private void CleanupAndFinish()
    {
        SetLegHitboxes(false);
        SafeSetGroup(boss.shockwave, false);
        landingPushActive = false;
        moveActive = false;
        isFinished = true;
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
            // lift
            float phaseT = Mathf.Clamp01(norm / Mathf.Max(0.0001f, liftPortion));
            float eased = curve.Evaluate(phaseT);

            float y = baseY + boss.meleeLiftHeight * eased;
            return new Vector3(startPos.x, y, startPos.z);
        }
        else
        {
            // dash down diagonally
            float dashPortion = Mathf.Max(0.0001f, 1f - liftPortion);
            float phaseT = Mathf.Clamp01((norm - liftPortion) / dashPortion);
            float eased = curve.Evaluate(phaseT);

            Vector3 dashOffset = dashDir * (boss.meleeDashDistance * eased);

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

    private void SafeSetGroup(HitboxID id, bool on)
    {
        if (boss.hitboxManager == null) return;
        boss.hitboxManager.SetGroup(id, on);
    }

    public override void AnimEvent(string evt)
    {
        switch (evt)
        {
            case "startMove":
                {
                    // lock direction at the moment you start moving
                    startPos = boss.transform.position;
                    baseY = startPos.y;

                    dashDir = GetFlatDirToPlayer();
                    if (dashDir.sqrMagnitude < 0.0001f)
                        dashDir = boss.transform.forward;

                    t = 0f;
                    moveActive = true;
                    break;
                }

            // --- your requested hitbox group logic ---
            case "hitCheck":
                {
                    if (boss.hitboxManager != null)
                    {
                        boss.hitboxManager.SetGroupHitReports(boss.shockwave, GetType());
                        boss.hitboxManager.SetGroup(boss.shockwave, true);
                    }
                    break;
                }

            case "endHitCheck":
                {
                    SafeSetGroup(boss.shockwave, false);
                    break;
                }

            // --- optional: keep your leg collider toggles too if you still use them ---
            case "hitOn":
                SetLegHitboxes(true);
                break;

            case "hitOff":
                SetLegHitboxes(false);
                break;

            // call this at the landing frame so player doesn't clip into the bird
            case "land":
                {
                    landingPushActive = true;
                    landingPushTimer = 0f;
                    break;
                }

            case "end":
                CleanupAndFinish();
                break;
        }
    }

    // Your provided helper (kept identical)
    public static void PushCCSideways(CharacterController cc, Vector3 bossCenter, float bossRadius, float pushSpeed)
    {
        Vector3 p = cc.transform.position;

        Vector3 d = p - bossCenter;
        d.y = 0f;

        float dist = d.magnitude;
        if (dist < 0.0001f) d = Vector3.forward;

        if (dist < bossRadius)
        {
            Vector3 dir = d.normalized;
            float penetration = bossRadius - dist;

            Vector3 push = dir * (penetration * pushSpeed) * Time.deltaTime;
            push.y = -0.2f * Time.deltaTime;

            cc.Move(push);
        }
    }

    public override BossMove Clone()
    {
        return new EnergySlash(this.boss, 0f, this.slashOrCross);
    }
}