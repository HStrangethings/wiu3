using System.Runtime.CompilerServices;
using UnityEngine;

public class LokiSwap : BossState
{
    public LokiSwap(BossStateMachine sm, BossBehaviour boss) : base(sm, boss) { }
    private BossStats bossStats;

    private float timer = 0f;

    private float idleTime = 5f;
    private bool needMove = false;

    private bool inMeleeRange = false;


    public override void Enter()
    {
        bossStats = boss.boss;
    }

    public override void Execute()
    {
        inMeleeRange = boss.DistanceToPlayer().magnitude < bossStats.bossRad + bossStats.bossMeleeReach;

        //slowly make way to player 
        if (timer <= 0)
        {
            //only reset timer whrn idle timer is over
            timer = idleTime;

            boss.sm.ChangeState<LokiAttack>();
        }
        else
        {
            timer -= Time.deltaTime;

            if (!inMeleeRange)
            {
                needMove = true;
            }
        }

        if (needMove)
        {
            boss.rb.linearVelocity = boss.MoveToPlayer();
        }
        else
        {
            boss.rb.linearVelocity = Vector3.zero;
        }

        boss.transform.rotation = boss.RotateToPlayer();
    }

    public override void Exit()
    {
    }

}
