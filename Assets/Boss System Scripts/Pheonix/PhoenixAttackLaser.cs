using UnityEngine;

public class PhoenixAttackLaser : BossState
{
    private bool started;

    public PhoenixAttackLaser(BossStateMachine sm, BossBehaviour boss) : base(sm, boss) { }

    public override void Enter()
    {
        started = false;
        boss.rb.linearVelocity = Vector3.zero;
    }

    public override void Execute()
    {
        boss.rb.linearVelocity = Vector3.zero;
        boss.transform.rotation = boss.RotateToPlayer();

        if (!started)
        {
            started = true;
            boss.mm.PlayMove("LaserBeam");
        }

        //  FAILSAFE
        AnimatorStateInfo st = boss.animator.GetCurrentAnimatorStateInfo(0);
        if (st.IsName("LaserBeamAnim") && st.normalizedTime >= 0.99f)
        {
            sm.ChangeState<PhoenixIdle>();
        }
    }

    public override void ComboFin()
    {
        sm.ChangeState<PhoenixIdle>();
    }

    public override void Exit() { }
}