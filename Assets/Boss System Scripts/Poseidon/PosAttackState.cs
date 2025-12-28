using UnityEngine;

public class PosAttackState : BossState
{
    public PosAttackState(BossStateMachine sm, BossBehaviour boss) : base(sm, boss) { }
    BossStats bossStat;

    public override void Enter()
    {
        bossStat = boss.boss;
        boss.rb.linearVelocity = Vector3.zero;

        if (boss.DistanceToPlayer().magnitude < bossStat.bossRad + bossStat.bossMeleeReach)
        {
            boss.mm.PlayMove("posMelee");
        }
    }

    public override void Execute()
    {
        Quaternion rotateToPlayer = boss.RotateToPlayer();
        boss.transform.rotation = rotateToPlayer;
    }

    public override void Exit()
    {
       // Debug.Log("Exitting Poseidon Attack State");
    }
}
