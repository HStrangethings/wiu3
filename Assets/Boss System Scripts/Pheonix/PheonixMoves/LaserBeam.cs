using UnityEngine;

public class LaserBeam : BossMove
{
    private PhoenixBoss boss;
    private float chargeUpTime;
    public LaserBeam(PhoenixBoss boss,float chargeUpTime) : base(boss)
    {
        this.boss = boss;
        this.chargeUpTime = chargeUpTime;
    }
    private float timer = 0;
    bool fired;
    bool comboChecked;
    private float comboCheckTime = 1.5f;
    private GameObject proj;
    public override void Start()
    {
        boss.animator.Play("LaserBeamAnim");
    }
    public override void Execute()
    {

        timer += Time.deltaTime;


        if (!comboChecked && timer >= 0.2f && boss.mm.HitConfirmed(GetType()))
        {
            comboChecked = true;
            AnimEvent("comboCheck");
            isFinished = true;
            return;
        }

        ////call its own comboCheck instead of animation
        //if (!comboChecked && timer >= 3f + comboCheckTime)
        //{
        //    comboChecked = true;
        //    AnimEvent("comboCheck");
        //    isFinished = true;
        //}
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
                //TODO: make the laser come from the head instead, and make it look at the player, or mayb not uty
                // maybe make the laser parented to the pheonix too so that it follows wherever the head looks
                proj = Object.Instantiate(boss.laserBeam, boss.transform.position + boss.transform.forward * 5, Quaternion.LookRotation(boss.transform.forward));
                SetupBossHitReporting(proj);
                break;

            case "end":
                GameObject.Destroy(proj);
                isFinished = true;
                break;
            case "comboCheck":
                boss.BossMoveComboDetails(GetType(), out bool hit, out bool LOS, out float dist);
                Debug.Log(hit);

                bool close = dist < 15f;
                bool far = dist > 15f;
                string nextMoveId = "null";

                //if (hit && !LOS) { nextMoveId = boss.mm.Choose("wideWaterBlast", "null"); }
                //else if (hit && close) { nextMoveId = boss.mm.Choose("posMelee", "wideWaterBlast", "posMelee"); }
                //else if (hit && far) { nextMoveId = boss.mm.Choose("waterBlast", "boatShield", "null"); }
                //else if (!hit && LOS) { nextMoveId = boss.mm.Choose("wideWaterBlast", "waterWave"); }
                //else { nextMoveId = boss.mm.Choose("waterBlast", "posMelee", "null"); }

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
        return new LaserBeam(this.boss, this.chargeUpTime);
    }

}
