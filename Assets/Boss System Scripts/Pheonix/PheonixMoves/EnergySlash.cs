using UnityEngine;

public class EnergySlash : BossMove
{
    private PhoenixBoss boss;
    private float projSpeed;
    private int SlashORCross;
    public EnergySlash(PhoenixBoss boss, float projSpeed,int SlashORCross) : base(boss)
    {
        this.boss = boss;
        this.projSpeed = projSpeed;
        this.SlashORCross = SlashORCross;
    }
    private float timer = 0;
    bool fired;
    bool comboChecked;
    private GameObject proj;

    private GameObject usingPrefab;

    private float comboCheckTime = 1.5f;
    public override void Start()
    {
        if (SlashORCross == 0)
        {
            usingPrefab = boss.singleSlash;
        }
        else { usingPrefab = boss.crossSlash; }

            Debug.Log("Starting WaterBlast");
        proj = Object.Instantiate(usingPrefab, boss.transform.position + boss.transform.forward * 9, Quaternion.LookRotation(boss.transform.forward));
        SetupBossHitReporting(proj);
    }
    public override void Execute()
    {
        timer += Time.deltaTime;

        if (!fired)
        {
            fired = true;
            if (proj != null)
            {
                var projRb = proj.GetComponent<Rigidbody>();
                projRb.AddForce(projRb.transform.forward * projSpeed, ForceMode.Impulse);
            }
        }


        if (!comboChecked && timer >= 0.2f && boss.mm.HitConfirmed(GetType()))
        {
            comboChecked = true;
            AnimEvent("comboCheck");
            isFinished = true;
            return;
        }

        //call its own comboCheck instead of animation
        if (!comboChecked && timer >= comboCheckTime)
        {
            comboChecked = true;
            AnimEvent("comboCheck");
            isFinished = true;
        }
    }
    public override void End()
    {
        Debug.Log("Ending WaterBlast");
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
                //Debug.Log(hit);

                bool close = dist < 15f;
                bool far = dist > 15f;
                string nextMoveId = "null";

                //if (hit && !LOS) { nextMoveId = boss.mm.Choose("waterWave", "null"); }
                //else if (hit && close) { nextMoveId = boss.mm.Choose("posMelee", "wideWaterBlast"); }
                //else if (hit && far) { nextMoveId = boss.mm.Choose("waterWave", "boatShield", "null"); }
                //else if (!hit && LOS) { nextMoveId = boss.mm.Choose("wideWaterBlast", "waterWave"); }
                //else { nextMoveId = boss.mm.Choose("waterWave", "posMelee", "null"); }

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
        return new EnergySlash(this.boss, this.projSpeed, this.SlashORCross);
    }
}
