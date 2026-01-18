using UnityEngine;

public class LokiInvis : BossState
{
    public LokiInvis(BossStateMachine sm, BossBehaviour boss) : base(sm, boss)
    {
    }
    BossStats bossStat;

    private float timer = 0f;
    private float invisTime = 5f;

    private bool needMove = false;


    public override void Enter()
    {
        bossStat = boss.boss;
        timer = invisTime;

        //dont render boss
    }

    public override void Execute()
    {
        var anim = boss.animator;

        anim.Play("Invis");

        if (timer <= 0)
        {
            //change this to a 3 hit combo atk
            boss.sm.ChangeState<LokiAttack>();
        }
        else
        {
            timer -= Time.deltaTime;
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
