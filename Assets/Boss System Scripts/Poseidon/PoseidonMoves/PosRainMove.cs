using UnityEngine;

public class PosRainMove : BossMove
{
    private PoseidonBoss boss;
    public PosRainMove(PoseidonBoss boss) : base(boss)
    {
        this.boss = boss;
    }
    private float timer = 0;
    bool fired;
    bool comboChecked;
    private GameObject proj;

    private float comboCheckTime = 1.5f;
    public override void Start()
    {
        Debug.Log("Starting WaterBlast");

        //start playing some kinda animation
        AnimEvent("start");
    }
    public override void Execute()
    {
        isFinished = true;
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
                GameObject.Instantiate(boss.rain,new Vector3(-22,46,0), Quaternion.identity);
                break;

            case "end":
                isFinished = true;
                break;
            case "comboCheck":
                boss.BossMoveComboDetails(GetType(), out bool hit, out bool LOS, out float dist);

                bool close = dist < 11f;
                bool far = dist > 11f;
                string nextMoveId = "null";

                //bool closeAttacking = boss.IsPlayerAttacking();
                //if (closeAttacking && LOS) { nextMoveId = boss.mm.Choose("quickPosMelee", "null"); }
                //else if (hit && !LOS) { nextMoveId = boss.mm.Choose("waterWave", "null"); }
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
        return new PosRainMove(this.boss);
    }

}
