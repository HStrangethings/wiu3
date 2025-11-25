using UnityEngine;

public class PosAttackState : BossState
{
    public PosAttackState(BossStateMachine sm, BossBehaviour boss) : base(sm, boss) { }

    public override void Enter()
    {
        //Debug.Log("Entering Poseidon Attack State");
        boss.rb.linearVelocity = Vector3.zero;
        Debug.Log(boss.rb.linearVelocity);
    }

    public override void Execute()
    {
        //Debug.Log("Currently in Poseidon Attack State");
        if (Input.GetKeyDown(KeyCode.F))
        {
            boss.mm.PlayMove("waterBlast");
        }
    }

    public override void Exit()
    {
       // Debug.Log("Exitting Poseidon Attack State");
    }
}
