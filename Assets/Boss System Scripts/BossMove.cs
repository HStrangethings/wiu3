using UnityEngine;

public abstract class BossMove
{
    // change boss type to your own boss to reference boss specific variables.
    //look at Poseidon water blast for example.
    protected BossBehaviour boss;

    public bool isFinished = false;
    protected BossMove(BossBehaviour boss)
    {
        this.boss = boss;
    }

    public abstract void Start();
    public abstract void Execute();
    public virtual void End()
    {
        isFinished = true;
    }
    public abstract void AnimEvent(string evt);
    public abstract BossMove Clone();


    public void SetupBossHitReporting(GameObject go)
    {
        if (go == null) return;

        var dealer = go.GetComponent<BossDamageDealer>();
        if (dealer == null) return;

        dealer.sourceMove = GetType();
        dealer.mm = boss.mm;
    }

}
