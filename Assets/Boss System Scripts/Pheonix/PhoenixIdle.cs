using UnityEngine;

public class PhoenixIdle : BossState
{
    private float cooldownTimer;

    private readonly float attackCooldown = 1.0f;

    // Distance rules:
    private readonly float meleeRange = 6.0f;      // close => melee
    private readonly float laserMinRange = 7.0f;  // far => laser
    // between meleeRange and laserMinRange => aerial

    public PhoenixIdle(BossStateMachine sm, BossBehaviour boss) : base(sm, boss) { }

    public override void Enter()
    {
        cooldownTimer = 0f;
        boss.rb.linearVelocity = Vector3.zero;
    }

    public override void Execute()
    {
        if (Time.frameCount % 30 == 0)
            Debug.Log($"Idle tick | playerNull={(boss.currPlayer == null)} | los={boss.HasLOS()} | cd={cooldownTimer:F2}");

        boss.rb.linearVelocity = Vector3.zero;
        boss.transform.rotation = boss.RotateToPlayer();

        cooldownTimer += Time.deltaTime;

        if (boss.currPlayer == null) return;

        float dist = boss.DistanceToPlayer().magnitude;
        bool los = boss.HasLOS();

        if (!los) return;
        if (cooldownTimer < attackCooldown) return;

        if (dist <= meleeRange)
        {
            sm.ChangeState<PhoenixAttackMelee>();
            return;
        }

        if (dist >= laserMinRange)
        {
            //sm.ChangeState<PhoenixAttackLaser>();
            sm.ChangeState<PhoenixAttackMelee>();
            return;
        }

        sm.ChangeState<PhoenixAerialSlash>();
    }

    public override void Exit() { }
}