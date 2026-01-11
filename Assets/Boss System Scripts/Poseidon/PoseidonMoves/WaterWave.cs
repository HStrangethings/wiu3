using UnityEngine;

public class WaterWave : BossMove
{
    private PoseidonBoss boss;
    private float projSpeed;
    private float xSize;
    private float chargeUpTime;
    public WaterWave(PoseidonBoss boss, float projSpeed,float xSize,float chargeUpTime) : base(boss)
    {
        this.boss = boss;
        this.projSpeed = projSpeed;
        this.xSize = xSize;
        this.chargeUpTime = chargeUpTime;
    }
    private float timer = 0;
    bool fired;
    bool comboChecked;
    private float comboCheckTime = 1.5f;
    private GameObject proj;
    public override void Start()
    {
        Debug.Log("Starting WaterWave");
        proj = Object.Instantiate(boss.waterWaveProj, boss.transform.position + boss.transform.forward * 7, Quaternion.LookRotation(boss.transform.forward));
        proj.transform.localScale = new Vector3(xSize, 0, boss.waterWaveProj.transform.localScale.z);
        SetupBossHitReporting(proj);
    }
    public override void Execute()
    {

        timer += Time.deltaTime;

        if (!fired && timer <= 3f)
        {
            float t = Mathf.Clamp01(timer);
            float scaleY = Mathf.Lerp(0, boss.waterWaveProj.transform.localScale.y, t);
            proj.transform.localScale = new Vector3(xSize, scaleY, boss.waterWaveProj.transform.localScale.z);
        }

        if (!fired && timer >= 3f)
        {
            fired = true;
            if (proj != null)
            {
                proj.transform.localScale = new Vector3(xSize, boss.waterWaveProj.transform.localScale.y, boss.waterWaveProj.transform.localScale.z);
                var projRb = proj.GetComponent<Rigidbody>();
                projRb.AddForce(projRb.transform.forward * projSpeed, ForceMode.Impulse);
            }
        }

        if (!comboChecked && timer >= 3f + 0.2f && boss.mm.HitConfirmed(GetType()))
        {
            comboChecked = true;
            AnimEvent("comboCheck");
            isFinished = true;
            return;
        }

        //call its own comboCheck instead of animation
        if (!comboChecked && timer >= 3f + comboCheckTime)
        {
            comboChecked = true;
            AnimEvent("comboCheck");
            isFinished = true;
        }
    }

    public override void End()
    {
        Debug.Log("Ending WaterWave");
        base.End();
    }
    public override void AnimEvent(string evt)
    {
        switch (evt)
        {
            case "start":
                //create wave projectile and start its own animation
                break;
            
            case"end":
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
                else if (hit && !LOS) { nextMoveId = boss.mm.Choose("wideWaterBlast", "null"); }
                else if (hit && close) { nextMoveId = boss.mm.Choose("posMelee", "wideWaterBlast", "posMelee"); }
                else if (hit && far) { nextMoveId = boss.mm.Choose("waterBlast", "boatShield", "null"); }
                else if (!hit && LOS) { nextMoveId = boss.mm.Choose("wideWaterBlast", "waterWave"); }
                else { nextMoveId = boss.mm.Choose("waterBlast", "posMelee", "null"); }

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
        return new WaterWave(this.boss, this.projSpeed,this.xSize,this.chargeUpTime);
    }

}
