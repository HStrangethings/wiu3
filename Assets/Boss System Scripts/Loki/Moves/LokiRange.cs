using UnityEngine;

public class LokiRange : BossMove
{
    private LokiBoss boss;
    private float projSpeed;

    public LokiRange(LokiBoss boss, float projSpeed) : base(boss)
    {
        this.boss = boss;
        this.projSpeed = projSpeed;
    }

    private float timer = 0;
    bool fired;
    bool comboChecked;
    private GameObject proj;

    private float comboCheckTime = 1.5f;
    public override void Start()
    {
    
        proj = Object.Instantiate(boss.coinProj, boss.transform.position + boss.transform.forward * 9, Quaternion.LookRotation(boss.transform.forward));
        SetupBossHitReporting(proj);
    }
    public override void Execute()
    {
        timer += Time.deltaTime;

        if (!fired && timer >= 1f)
        {
            fired = true;
            if (proj != null)
            {
                var projRb = proj.GetComponent<Rigidbody>();
                projRb.AddForce(projRb.transform.forward * projSpeed, ForceMode.Impulse);
            }
        }
        if (!comboChecked && timer >= 1f + 0.2f && boss.mm.HitConfirmed(GetType()))
        {
            comboChecked = true;
            AnimEvent("comboCheck");
            isFinished = true;
            return;
        }
        //call its own comboCheck instead of animation
        if (!comboChecked && timer >= 1f + comboCheckTime)
        {
            comboChecked = true;
            AnimEvent("comboCheck");
            isFinished = true;
        }
    }
    public override void End()
    {
        base.End();
    }
    public override void AnimEvent(string evt)
    {
        switch (evt)
        {
            case "start":
                //spawn projectile to start its own animation
                break;

            case "end":
                var projRb = proj.GetComponent<Rigidbody>();
                projRb.AddForce(projRb.transform.forward * projSpeed, ForceMode.Impulse);
                isFinished = true;
                break;
            case "comboCheck":
                boss.BossMoveComboDetails(GetType(), out bool hit, out bool LOS, out float dist);

                bool close = dist < 11f;
                bool far = dist > 11f;
                string nextMoveId = "null";

                bool closeAttacking = boss.IsPlayerAttacking();
                if (closeAttacking && LOS) { nextMoveId = boss.mm.Choose("quickPosMelee", "null"); }
                else if (hit && !LOS) { nextMoveId = boss.mm.Choose("waterWave", "null"); }
                else if (hit && close) { nextMoveId = boss.mm.Choose("posMelee", "wideWaterBlast"); }
                else if (hit && far) { nextMoveId = boss.mm.Choose("waterWave", "boatShield", "null"); }
                else if (!hit && LOS) { nextMoveId = boss.mm.Choose("wideWaterBlast", "waterWave"); }
                else { nextMoveId = boss.mm.Choose("waterWave", "posMelee", "null"); }

                if (!string.IsNullOrEmpty(nextMoveId))
                {
                    boss.mm.PlayMove(nextMoveId);
                }
                isFinished = true;
                break;
        }
    }
    public override BossMove Clone()
    {
        return new LokiRange(this.boss, this.projSpeed);
    }
}
