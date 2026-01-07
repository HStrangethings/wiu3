using System.Threading;
using UnityEngine;

public class PosIdleState : BossState
{
    public PosIdleState(BossStateMachine sm, BossBehaviour boss) : base(sm, boss) { }
    private float timer = 0;

    bool LOS = false;
    bool needMove = false;
    float idleDur = 0;

    BossStats bossStat;

    public override void Enter()
    {
        timer = 0;

        bossStat = boss.boss;
        idleDur = Random.Range(2f, 4.5f);

        LOS = boss.HasLOS();
        if (!LOS) { needMove = true; return; }
        int r = Random.Range(0, 100);
        if ( r < 50)
        {
            needMove = true;
        }
    }

    public override void Execute()
    {
        //Debug.Log("Currently in Poseidon Idle State");
        if (timer < idleDur) { 
            timer += Time.deltaTime;

            float timeRemaining = idleDur - timer;

            //if (boss.DistanceToPlayer().magnitude < bossStat.bossRad + bossStat.bossMeleeReach)
            //{
            //    if (timeRemaining > 0.5f)
            //    {
            //        timer += 0.4f; //if theres still quite abit of time, make it faster since player is so close
            //    }
            //}

            //if (timeRemaining < 0.1f)
            //{
            //    if (boss.DistanceToPlayer().magnitude > 40)
            //    {
            //        idleDur += 2f; //if timer finish, but player still too far away, extend time first
            //    }
            //}
        }
        else
        {
            Debug.Log("Entering AttackState");
            sm.ChangeState<PosAttackState>(); 
            return;
            
        }

        if (needMove)
        {
            Vector3 chasePlayerVel = boss.MoveToPlayer();
            boss.rb.linearVelocity = chasePlayerVel;
        }
        else { boss.rb.linearVelocity = Vector3.zero; }
        Quaternion rotateToPlayer = boss.RotateToPlayer();
        boss.transform.rotation = rotateToPlayer;
    }

    public override void Exit()
    {
        //Debug.Log("Exitting Poseidon Idle State");
    }
}
