using UnityEngine;

public class PhoenixCharge : BossMove
{
    private PhoenixBoss boss;

    private float chargeSpeed;
    private float chargeDuration;
    private float stopDistance;

    private float timer;
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
        comboChecked = false;

        // Defensive: in case player spawns late or tag missing
        // BossBehaviour normally sets this in Start() using "Player" tag. :contentReference[oaicite:3]{index=3}
        if (boss.player == null)
            boss.player = GameObject.FindGameObjectWithTag("Player");

        // Lock direction ONCE (straight charge, not homing)
        Vector3 toPlayer = boss.DistanceToPlayer(); // uses BossBehaviour helper :contentReference[oaicite:4]{index=4}
        toPlayer.y = 0f;

        chargeDir = toPlayer.sqrMagnitude > 0.001f ? toPlayer.normalized : boss.transform.forward;

        // Face the charge direction
        if (chargeDir.sqrMagnitude > 0.001f)
            boss.transform.rotation = Quaternion.LookRotation(chargeDir);

        // Optional: If you have a HitboxGroup for charge, enable it here.
        // boss.hitboxManager.EnableGroup("Charge", true);
        // boss.hitboxManager.SetGroupHitReports("Charge", GetType());
        // boss.hitboxManager.SetGroupMoveMachines("Charge", boss.mm);
    }

    public override void Execute()
    {
        timer += Time.deltaTime;

        // Move forward
        boss.rb.linearVelocity = chargeDir * chargeSpeed;

        // Use the same combo detail method as EnergySlash :contentReference[oaicite:5]{index=5}
        boss.BossMoveComboDetails(GetType(), out bool hit, out bool LOS, out float dist);

        // Stop early if close enough
        if (dist <= stopDistance)
        {
            FinishAndComboCheck();
            return;
        }

        // End quickly if hit confirmed after a short minimum time
        if (!comboChecked && timer >= 0.1f && hit)
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
        // Optional: disable charge hitbox group
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

                string nextMoveId = "null";

                // Example chaining logic (edit to your taste)
                // If charge hit and we're still close, follow with melee / slash.
                if (hit && close) nextMoveId = boss.mm.Choose("posMelee", "EnergySlash");
                // If it missed but boss has LOS, try a ranged option.
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