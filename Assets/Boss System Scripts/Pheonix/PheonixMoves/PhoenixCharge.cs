using UnityEngine;

public class PhoenixCharge : BossMove
{
    private PhoenixBoss boss;
    private float chargeSpeed;
    private float chargeDuration;
    private float stopDistance;

    private float timer;
    private bool started;
    private bool comboChecked;

    private Vector3 chargeDir;

    public PhoenixCharge(PhoenixBoss boss, float chargeSpeed, float chargeDuration, float stopDistance = 2.5f)
        : base(boss)
    {
        this.boss = boss;
        this.chargeSpeed = chargeSpeed;
        this.chargeDuration = chargeDuration;
        this.stopDistance = stopDistance;
    }

    public override void Start()
    {
        timer = 0f;
        started = true;
        comboChecked = false;

        // Lock direction ONCE (straight charge)
        Vector3 toPlayer = (boss.player.transform.position - boss.transform.position);
        toPlayer.y = 0f;
        chargeDir = toPlayer.sqrMagnitude > 0.001f ? toPlayer.normalized : boss.transform.forward;

        // Face the charge direction
        if (chargeDir.sqrMagnitude > 0.001f)
            boss.transform.rotation = Quaternion.LookRotation(chargeDir);

        // Optional: enable a hitbox group for charge damage (if you have one)
        // boss.hitboxManager.EnableGroup("Charge", true);
        // boss.hitboxManager.SetGroupHitReports("Charge", GetType());
        // boss.hitboxManager.SetGroupMoveMachines("Charge", boss.mm);
    }

    public override void Execute()
    {
        timer += Time.deltaTime;

        // Move forward (physics-friendly)
        boss.rb.linearVelocity = chargeDir * chargeSpeed;

        // Stop early if close enough
        float dist = Vector3.Distance(boss.transform.position, boss.player.transform.position);
        if (dist <= stopDistance)
        {
            FinishAndComboCheck();
            return;
        }

        // If we hit the player during the dash, end quickly (same style as EnergySlash)
        if (!comboChecked && timer >= 0.1f && boss.mm.HitConfirmed(GetType()))
        {
            FinishAndComboCheck();
            return;
        }

        // End after duration
        if (timer >= chargeDuration)
        {
            FinishAndComboCheck();
        }
    }

    private void FinishAndComboCheck()
    {
        if (comboChecked) return;
        comboChecked = true;

        // Stop movement cleanly
        boss.rb.linearVelocity = Vector3.zero;

        AnimEvent("comboCheck");
        isFinished = true;
    }

    public override void End()
    {
        // boss.hitboxManager.EnableGroup("Charge", false);
        boss.rb.linearVelocity = Vector3.zero;
        base.End();
    }

    public override void AnimEvent(string evt)
    {
        switch (evt)
        {
            case "comboCheck":
                boss.BossMoveComboDetails(GetType(), out bool hit, out bool LOS, out float dist);

                bool close = dist < 10f;
                bool far = dist >= 10f;

                string nextMoveId = "null";

                // Example chaining logic (tune however you like)
                if (hit && close) nextMoveId = boss.mm.Choose("posMelee", "EnergySlash");
                else if (!hit && LOS) nextMoveId = boss.mm.Choose("LaserBeam", "EnergySlash");
                else nextMoveId = "null";

                if (!string.IsNullOrEmpty(nextMoveId))
                    boss.mm.PlayMove(nextMoveId);

                isFinished = true;
                break;
        }
    }

    public override BossMove Clone()
    {
        return new PhoenixCharge(boss, chargeSpeed, chargeDuration, stopDistance);
    }
}