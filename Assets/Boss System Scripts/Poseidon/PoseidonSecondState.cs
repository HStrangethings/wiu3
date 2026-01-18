using UnityEngine;

public class PoseidonSecondState : BossState
{
    public PoseidonSecondState(BossStateMachine sm, BossBehaviour boss) : base(sm, boss) { }
    BossStats bossStat;

    public override void Enter()
    {
        bossStat = boss.boss;
        boss.rb.linearVelocity = Vector3.zero;
        boss.animator.Play("SecondPhase");
    }

    public override void Execute()
    {
        var anim = boss.animator;
        var st = anim.GetCurrentAnimatorStateInfo(0);

        if (!st.IsName("SecondPhase")) return;

        if (st.normalizedTime >= 1f && !anim.IsInTransition(0))
        {
            boss.mm.PlayMove("rain");
            boss.animator.Play("New State");
            sm.ChangeState<PoseidonSecondStateIdle>();
            return;
        }
    }

    public override void Exit()
    {
        Debug.Log("Exitting Poseidon Attack State");
        //boss.sm.ChangeState<PosIdleState>();
    }

    public override void ComboFin()
    {
        base.ComboFin();
        sm.ChangeState<PoseidonSecondStateIdle>();
        return;
    }
}
