using UnityEngine;

public class LokiAttackState : BossState
{
    public LokiAttackState(BossStateMachine sm, BossBehaviour boss) : base(sm, boss) { }
    BossStats bossStat;

    public override void Enter()
    {
        bossStat = boss.boss;
        boss.rb.linearVelocity = Vector3.zero;
    }

    public override void Execute()
    {
        Quaternion rotateToPlayer = boss.RotateToPlayer();
        boss.transform.rotation = rotateToPlayer;
    }

    public override void Exit()
    {
        Debug.Log("Exitting Poseidon Attack State");
       //boss.sm.ChangeState<PosIdleState>();
    }

    public override void ComboFin()
    {
        base.ComboFin();
        boss.sm.ChangeState<LokiIdleState>();
    }
}
