using UnityEditor;
using UnityEngine;

public class PosMeleeAttack : BossMove
{
    private PoseidonBoss boss;
    public PosMeleeAttack(PoseidonBoss boss) : base(boss)
    {
        this.boss = boss;
    }
    public override void Start()
    {
        Debug.Log("Starting PosMeleeAttack");
        boss.animator.Play("ArmAttack");
    }
    public override void Execute()
    {
        AnimatorStateInfo stateInfo = boss.animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("ArmAttack") && stateInfo.normalizedTime >= 0.99f)
            isFinished = true;
    }
    public override void End()
    {
        Debug.Log("Ending PosMeleeAttack");
        boss.hitboxManager.SetGroup(boss.Harms, false);
        boss.animator.Play("New State");
        base.End();
    }
    public override void AnimEvent(string evt)
    {
        switch (evt)
        {
            case "start":
                boss.hitboxManager.SetGroupHitReports(boss.Harms, GetType());
                boss.hitboxManager.SetGroup(boss.Harms, true);
                break;

            case "end":
                boss.hitboxManager.SetGroup(boss.Harms, false);
                break;
            case "comboCheck":
                boss.BossMoveComboDetails(GetType(), out bool hit, out bool LOS, out float dist);
                Debug.Log(hit);

                bool close = dist < 15f;
                bool far = dist > 15f;
                string nextMoveId = "null";

                if (hit && !LOS) { nextMoveId = boss.mm.Choose("waterWave", "null","waterWave"); }
                else if (hit && close) { nextMoveId = boss.mm.Choose("posMelee", "null", "posMelee"); }
                else if (hit && far) { nextMoveId = boss.mm.Choose("waterBlast", "boatShield", "boatShield", "null"); }
                else if (!hit && LOS && far) { nextMoveId = boss.mm.Choose("boatShield", "waterBlast", "waterWave", "null"); }
                else if (!hit && LOS && close) { nextMoveId = boss.mm.Choose("posMelee", "null"); }
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
        return new PosMeleeAttack(this.boss);
    }

}
