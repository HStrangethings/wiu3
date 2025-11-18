using UnityEngine;

public abstract class BossMove
{
    //TODO: make it need to pass in the boss later

    public bool isFinished = false;

    public abstract void Start();
    public abstract void Execute();
    public virtual void End()
    {
        isFinished = true;
    }

}
