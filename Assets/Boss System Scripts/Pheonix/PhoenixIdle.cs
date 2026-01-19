using UnityEngine;

public class PhoenixIdle : BossState
{
    public PhoenixIdle(BossStateMachine sm, BossBehaviour boss) : base(sm, boss) { }
    private float timer = 0;

    BossStats bossStat;

    public override void Enter()
    {

    }

    public override void Execute()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            boss.mm.PlayMove("LaserBeam");
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            boss.mm.PlayMove("PhoenixCharge");
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            boss.mm.PlayMove("CoralFan");
        }
        //if (needMove)
        //{
        //    Vector3 chasePlayerVel = boss.MoveToPlayer();
        //    boss.rb.linearVelocity = chasePlayerVel;
        //}
        boss.rb.linearVelocity = Vector3.zero;
        Quaternion rotateToPlayer = boss.RotateToPlayer();
        boss.transform.rotation = rotateToPlayer;
    }

    public override void Exit()
    {
        //Debug.Log("Exitting Poseidon Idle State");
    }
}
