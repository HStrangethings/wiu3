using UnityEngine;

public class TripleSlash : BossMove
{
    private PhoenixBoss boss;
    private float projSpeed;
    private int SlashORCross;
    public TripleSlash(PhoenixBoss boss, float projSpeed, int SlashORCross) : base(boss)
    {
        this.boss = boss;
        this.projSpeed = projSpeed;
        this.SlashORCross = SlashORCross;
    }
    private float timer = 0;
    bool comboChecked;
    bool fired;
    private GameObject[] proj = new GameObject[3];
    private bool[] projCreated = new bool[3];

    private GameObject usingPrefab;

    private float comboCheckTime = 0.7f;
    public override void Start()
    {
        Debug.Log("Starting WaterBlast");
        if (SlashORCross == 0)
        {
            usingPrefab = boss.singleSlash;
        }
        else { usingPrefab = boss.crossSlash; }
    }
    public override void Execute()
    {
        timer += Time.deltaTime;
        float timeIntervals = 0.2f;

        if (!fired && timer > timeIntervals && !projCreated[0])
        {
            projCreated[0] = true;
            proj[0] = Object.Instantiate(usingPrefab, boss.transform.position + boss.transform.forward * 9, Quaternion.LookRotation(boss.transform.forward));
            SetupBossHitReporting(proj[0]);
            var projRb = proj[0].GetComponent<Rigidbody>();
            projRb.AddForce(projRb.transform.forward * projSpeed, ForceMode.Impulse);
        }
        if (!fired && timer > timeIntervals * 2 && !projCreated[1])
        {
            projCreated[1] = true;
            proj[1] = Object.Instantiate(usingPrefab, boss.transform.position + boss.transform.forward * 9, Quaternion.LookRotation(boss.transform.forward));
            SetupBossHitReporting(proj[1]);
            var projRb = proj[1].GetComponent<Rigidbody>();
            projRb.AddForce(projRb.transform.forward * projSpeed, ForceMode.Impulse);

            SetupBossHitReporting(proj[1]);
        }
        if (!fired && timer > timeIntervals * 3 && !projCreated[2])
        {
            projCreated[2] = true;
            proj[2] = Object.Instantiate(usingPrefab, boss.transform.position + boss.transform.forward * 9, Quaternion.LookRotation(boss.transform.forward));
            SetupBossHitReporting(proj[2]);
            var projRb = proj[2].GetComponent<Rigidbody>();
            projRb.AddForce(projRb.transform.forward * projSpeed, ForceMode.Impulse);

            SetupBossHitReporting(proj[2]);
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
        return new TripleSlash(this.boss, this.projSpeed,this.SlashORCross);
    }
}
