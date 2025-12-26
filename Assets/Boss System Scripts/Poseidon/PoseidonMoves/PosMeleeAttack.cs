using UnityEditor;
using UnityEngine;

public class PosMeleeAttack : BossMove
{
    private PoseidonBoss boss;
    public PosMeleeAttack(PoseidonBoss boss) : base(boss)
    {
        this.boss = boss;
    }
    public override void Start()
    {
        Debug.Log("Starting PosMeleeAttack");
        boss.animator.Play("ArmAttack");
    }
    public override void Execute()
    {
        AnimatorStateInfo stateInfo = boss.animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("ArmAttack") && stateInfo.normalizedTime >= 0.99f)
            isFinished = true;
    }
    public override void End()
    {
        Debug.Log("Ending PosMeleeAttack");
        boss.animator.Play("New State");
        base.End();
    }
    public override void AnimEvent(string evt)
    {
        switch (evt)
        {
            case "start":
                boss.hitboxManager.SetGroup(boss.Harms, true);
                break;

            case "end":
                boss.hitboxManager.SetGroup(boss.Harms, false);
                break;
        }
    }

    public override BossMove Clone()
    {
        return new PosMeleeAttack(this.boss);
    }

}
