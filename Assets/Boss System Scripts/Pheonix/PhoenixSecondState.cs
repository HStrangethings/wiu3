using UnityEngine;

public class PhoenixSecondState : BossState
{
    public PhoenixSecondState(BossStateMachine sm, BossBehaviour boss) : base(sm, boss) { }

    private float timer;
    private bool started;

    public override void Enter()
    {
        timer = 0f;
        started = false;

        boss.rb.linearVelocity = Vector3.zero;

        // Optional: play your phase change animation
        // boss.animator.Play("SecondPhase");

        // If you want a flag your moves can read:
        // (Add public bool phase2 = false in PhoenixBoss)
        if (boss is PhoenixBoss pb)
            pb.phase2 = true;
    }

    public override void Execute()
    {
        timer += Time.deltaTime;

        // If you have an animation, you can wait for it:
        // var st = boss.animator.GetCurrentAnimatorStateInfo(0);
        // if (!started) started = true;
        // if (started && st.IsName("SecondPhase") && st.normalizedTime < 1f) return;

        // Simple timed transition (works even without animations)
        if (timer >= 1.0f)
        {
            //sm.ChangeState<PhoenixIdleStatePhase2>();
        }

        boss.transform.rotation = boss.RotateToPlayer();
    }

    public override void Exit() { }
}