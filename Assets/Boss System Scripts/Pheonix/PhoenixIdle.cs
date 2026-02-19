using UnityEngine;

public class PhoenixIdle : BossState
{
    private float cooldownTimer;

    // Tweak these in code (or convert to BossStats later)
    private readonly float attackCooldown = 2.0f;
    private readonly float attackRange = 6.0f;

    public PhoenixIdle(BossStateMachine sm, BossBehaviour boss) : base(sm, boss) { }

    public override void Enter()
    {
        cooldownTimer = 0f;
        // Optional: play idle animation if you have one
        // boss.mm.PlayMove("Idle");
    }

    public override void Execute()
    {
        // Stop movement
        boss.rb.linearVelocity = Vector3.zero;

        // Face player
        boss.transform.rotation = boss.RotateToPlayer();

        // Wait cooldown
        cooldownTimer += Time.deltaTime;

        // Check distance to decide attack
        // IMPORTANT: change boss.player to your actual player reference if needed
        if (boss.player == null) return;

        float dist = Vector3.Distance(boss.transform.position, boss.player.transform.position);

        if (cooldownTimer >= attackCooldown && dist <= attackRange)
        {
            sm.ChangeState<PhoenixAttack>();
        }
    }

    public override void Exit()
    {
    }

    public override void ComboFin()
    {
        // Idle usually doesn't care about combo finishing
    }
}