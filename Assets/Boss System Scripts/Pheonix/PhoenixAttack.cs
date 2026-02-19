using UnityEngine;

public class PhoenixAttack : BossState
{
    private bool attackStarted;

    public PhoenixAttack(BossStateMachine sm, BossBehaviour boss) : base(sm, boss) { }

    public override void Enter()
    {
        attackStarted = false;
    }

    public override void Execute()
    {
        // Stop moving during attack
        boss.rb.linearVelocity = Vector3.zero;

        // Optional: keep facing player while attacking
        boss.transform.rotation = boss.RotateToPlayer();

        // Start attack only ONCE
        if (!attackStarted)
        {
            attackStarted = true;

            // Pick your attack move name
            boss.mm.PlayMove("LaserBeam");
        }
    }

    public override void Exit()
    {
        // Cleanup if needed (stop effects, etc.)
    }

    // This should be called when the attack finishes
    // (e.g., animation event, MoveManager callback, etc.)
    public override void ComboFin()
    {
        sm.ChangeState<PhoenixIdle>();
    }
}