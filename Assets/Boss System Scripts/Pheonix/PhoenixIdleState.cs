using UnityEngine;

public class PhoenixIdleState : BossState
{
    public PhoenixIdleState(BossStateMachine sm, BossBehaviour boss) : base(sm, boss) { }

    private float timer;
    private bool needMove;
    private float idleDur;

    private BossStats bossStat;

    public override void Enter()
    {
        timer = 0f;

        bossStat = boss.boss;
        idleDur = Random.Range(2f, 4.5f);

        bool LOS = boss.HasLOS();
        needMove = !LOS;

        if (LOS)
        {
            int r = Random.Range(0, 100);
            if (r < 50) needMove = true;
        }
    }

    public override void Execute()
    {
        if (timer < idleDur)
        {
            timer += Time.deltaTime;

            float timeRemaining = idleDur - timer;

            // If player is close, speed up transition to attack
            if (boss.DistanceToPlayer().magnitude < bossStat.bossRad + bossStat.bossMeleeReach)
            {
                if (timeRemaining > 0.5f)
                    timer += 0.4f;
            }

            // If player is extremely far when time ends, extend idle slightly
            if (timeRemaining < 0.1f)
            {
                if (boss.DistanceToPlayer().magnitude > 40f)
                    idleDur += 2f;
            }
        }
        else
        {
            sm.ChangeState<PhoenixAttackState>();
            return;
        }

        if (needMove)
            boss.rb.linearVelocity = boss.MoveToPlayer();
        else
            boss.rb.linearVelocity = Vector3.zero;

        boss.transform.rotation = boss.RotateToPlayer();
    }

    public override void Exit() { }
}