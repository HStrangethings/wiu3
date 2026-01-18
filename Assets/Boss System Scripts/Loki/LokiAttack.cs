using UnityEngine;

public class LokiAttack : BossState
{
    public LokiAttack(BossStateMachine sm, BossBehaviour boss) : base(sm, boss) { }

    BossStats bossStats;

    private bool inMeleeRange;


    public override void Enter()
    {
        bossStats = boss.boss;
        boss.rb.linearVelocity = Vector3.zero;
    }

    public override void Execute()
    {
        inMeleeRange = boss.DistanceToPlayer().magnitude < bossStats.bossRad + bossStats.bossMeleeReach;

        boss.transform.rotation = boss.RotateToPlayer();

        if (inMeleeRange)
        {
            boss.mm.PlayMove("LokiMelee");
        }
        else
        {
            boss.mm.PlayMove("LokiRange");
        }
            //boss.mm.PlayMove("LokiSneak");



    }

    public override void Exit()
    {
    }

    public override void ComboFin()
    {
        base.ComboFin();
        boss.sm.ChangeState<LokiIdle>();
    }
}
