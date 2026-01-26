using UnityEngine;

public class PhoenixAttackState : BossState
{
    public PhoenixAttackState(BossStateMachine sm, BossBehaviour boss) : base(sm, boss) { }

    private BossStats bossStat;

    public override void Enter()
    {
        bossStat = boss.boss;
        boss.rb.linearVelocity = Vector3.zero;

        float dist = boss.DistanceToPlayer().magnitude;
        bool close = dist < bossStat.bossRad + bossStat.bossMeleeReach;
        bool LOS = boss.HasLOS();

        // ---- Choose Phoenix move here ----
        // Replace these strings with your real move IDs.
        if (close)
        {
            boss.mm.PlayMove("PhoenixCharge");   // or your melee move
        }
        else
        {
            if (LOS)
                boss.mm.PlayMove("CoralFan");    // or LaserBeam / EnergySlash
            else
                boss.mm.PlayMove("EnergySlash"); // fallback when no LOS
        }
    }

    public override void Execute()
    {
        boss.transform.rotation = boss.RotateToPlayer();
    }

    public override void Exit() { }

    public override void ComboFin()
    {
        base.ComboFin();
        boss.sm.ChangeState<PhoenixIdleState>();
    }
}