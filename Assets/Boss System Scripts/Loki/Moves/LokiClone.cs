using UnityEngine;

public class LokiClone : BossMove
{
    private LokiBoss boss;
    private LokiBoss.ATTACK move;



    public LokiClone(LokiBoss boss, LokiBoss.ATTACK move) : base(boss)
    {
        this.boss = boss;
        this.move = move;
    }

    public override void Start()
    {
    }
    public override void Execute()
    {
        switch (move)
        {
            case LokiBoss.ATTACK.Melee:
                boss.mm.PlayMove("LokiFakeMelee");
                break;

            case LokiBoss.ATTACK.Range:
                boss.mm.PlayMove("LokiFakeRange");
                break;

            case LokiBoss.ATTACK.QuickMelee:
                boss.mm.PlayMove("LokiFakeQuickMelee");
                break;
        }
    }
    public override void End()
    {
        base.End();
    }
    public override void AnimEvent(string evt)
    {
    }

    public override BossMove Clone()
    {
        return new LokiClone(this.boss, this.move);
    }

}
