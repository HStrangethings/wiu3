using UnityEngine;

public class LokiMelee : BossMove
{
    private LokiBoss boss;
    private float animSpeed;
    private string animation;

    public LokiMelee(LokiBoss boss, float animSpeed, string animation) : base(boss)
    {
        this.boss = boss;
        this.animSpeed = animSpeed;
        this.animation = animation;
    }
    public override void Start()
    {
        boss.animator.speed = animSpeed;
        boss.animator.Play("CardSlash");
    }
    public override void Execute()
    {
        AnimatorStateInfo stateInfo = boss.animator.GetCurrentAnimatorStateInfo(0);

        if(stateInfo.IsName("CardSlash") && stateInfo.normalizedTime >= 0.99f)
        {
            isFinished = true;
        }
    }
    public override void End()
    {
        boss.hitboxManager.SetGroup(boss.Harms, false);
        boss.animator.speed = 1;
        boss.animator.Play("New State");

        base.End();
    }
    public override void AnimEvent(string evt)
    {
        switch (evt)
        {
            case "start":
                boss.hitboxManager.SetGroupHitReports(boss.Harms, GetType());
                boss.hitboxManager.SetGroup(boss.Harms, true);
                break;

            case "end":
                boss.hitboxManager.SetGroup(boss.Harms, false);
                break;

            case "comboCheck":
                boss.BossMoveComboDetails(GetType(), out bool hit, out bool LOS, out float dist);

                bool close = dist < 11f;
                bool far = dist > 11f;
                string nextMoveId = "null";

                bool closeAttacking = boss.IsPlayerAttacking();

                if (!string.IsNullOrEmpty(nextMoveId))
                {
                    boss.mm.PlayMove(nextMoveId);
                }
                isFinished = true;
                break;
        }
    }
    public override BossMove Clone()
    {
        return new LokiMelee(this.boss, this.animSpeed, this.animation);
    }

}
