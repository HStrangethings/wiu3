using UnityEditor;
using UnityEngine;

public class PosMeleeAttack : BossMove
{
    private PoseidonBoss boss;
    private float animSpeed;
    bool canRotate = false;
    public PosMeleeAttack(PoseidonBoss boss, float animSpeed) : base(boss)
    {
        this.boss = boss;
        this.animSpeed = animSpeed;
    }
    public override void Start()
    {
        Debug.Log("Starting PosMeleeAttack");
        boss.animator.speed = animSpeed;
        boss.animator.Play("ArmAttack");
        canRotate = false;
    }
    public override void Execute()
    {
        AnimatorStateInfo stateInfo = boss.animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("ArmAttack") && stateInfo.normalizedTime >= 0.99f)
            isFinished = true;

        if (canRotate)
        {
            //boss.transform.rotation = boss.RotateToPlayer();
        }
    }
    public override void End()
    {
        Debug.Log("Ending PosMeleeAttack");
        boss.hitboxManager.SetGroup(boss.Harms, false);
        boss.animator.speed = 1;
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
            case "canrotate":
                canRotate = true;
                break;
            case "stoprotate":
                canRotate = false;
                break;
            case "comboCheck":
                boss.BossMoveComboDetails(GetType(), out bool hit, out bool LOS, out float dist);

                bool close = dist < 11f;
                bool far = dist > 11f;
                string nextMoveId = "null";

                bool closeAttacking = boss.IsPlayerAttacking();
                float angleToPlayer = boss.ToPlayerAngle();
                bool isBack = Mathf.Abs(angleToPlayer) > 135f && close;
                if (isBack) { nextMoveId = boss.mm.Choose("quickPosMelee", "null"); }
                else if (closeAttacking && LOS) { nextMoveId = boss.mm.Choose("quickPosMelee", "null"); }
                else if (hit && !LOS) { nextMoveId = boss.mm.Choose("waterWave", "null","waterWave"); }
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
        return new PosMeleeAttack(this.boss,this.animSpeed);
    }

}
