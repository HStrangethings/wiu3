using UnityEngine;

public abstract class BossState
{
    //for the state to keep track of when to switch states
    protected BossBehaviour boss;
    protected BossStateMachine sm;
    protected BossState(BossStateMachine sm, BossBehaviour boss)
    {
        this.boss = boss;
        this.sm = sm;
    }
    public abstract void Enter();

    public abstract void Execute();

    

    public abstract void Exit();

    public virtual void ComboFin()
    {
        Exit();
    }
}
