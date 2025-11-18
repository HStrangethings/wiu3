using UnityEngine;

public class PosIdleState : BossState
{
    public PosIdleState(BossStateMachine sm, BossBehaviour boss) : base(sm, boss) { }

    public override void Enter()
    {
        Debug.Log("Entering Poseidon Idle State");
    }

    public override void Execute()
    {
        Debug.Log("Currently in Poseidon Idle State");
        if (Input.GetKeyDown(KeyCode.F))
        {
            boss.mm.PlayMove("waterBlast");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            sm.ChangeState<PosAttackState>();
        }
    }

    public override void Exit()
    {
        Debug.Log("Exitting Poseidon Idle State");
    }
}
