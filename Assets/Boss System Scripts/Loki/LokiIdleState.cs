using UnityEngine;

public class LokiIdleState : BossState
{
    public LokiIdleState(BossStateMachine sm, BossBehaviour boss) : base(sm, boss) { }
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
        
    }

    public override void Exit()
    {
    }
}
