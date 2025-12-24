using System.Threading;
using UnityEngine;

public class PosIdleState : BossState
{
    public PosIdleState(BossStateMachine sm, BossBehaviour boss) : base(sm, boss) { }
    private float timer = 0;
    private float randomCooldown = 0;

    public override void Enter()
    {
        timer = 0;
        //Debug.Log("Entering Poseidon Idle State");
    }

    public override void Execute()
    {
        //Debug.Log("Currently in Poseidon Idle State");
        if (timer < 3f) { 
            timer += Time.deltaTime; 
            //Debug.Log("Idleing");
            }
        else
        {
            if (randomCooldown < 0.5) { randomCooldown += Time.deltaTime; }
            else
            {
                randomCooldown = 0;
                Debug.Log("rolling random number");
                int r = UnityEngine.Random.Range(0, 100);
                if (r > 0)
                {
                    Debug.Log("Entering AttackState");
                    Vector3 distVect = boss.player.transform.position - boss.transform.position;
                    float dist = distVect.magnitude;
                    if (dist > 2) { sm.ChangeState<PosAttackState>(); return; }
                }
                else { Debug.Log("random number failed"); }
            }
        }

        Vector3 chasePlayerVel = boss.MoveToPlayer();
        boss.rb.linearVelocity = chasePlayerVel;
        Quaternion rotateToPlayer = boss.RotateToPlayer();
        boss.transform.rotation = rotateToPlayer;
    }

    public override void Exit()
    {
        //Debug.Log("Exitting Poseidon Idle State");
    }
}
