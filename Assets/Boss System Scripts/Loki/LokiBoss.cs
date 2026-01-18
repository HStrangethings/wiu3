using UnityEngine;

public class LokiBoss : BossBehaviour
{
    public enum ATTACK
    {
        Melee = 0,
        QuickMelee,
        Range
    }

    public GameObject coinProj;

    // list of all hitboxes
    public HitboxID Harms;

    public override void Start()
    {
        base.Start();
        //add all the bosses states
        //pass in the statemachine and this boss instance to the state
        sm.AddState(new LokiIdle(sm, this));
        sm.AddState(new LokiAttack(sm, this));
        sm.AddState(new LokiInvis(sm, this));

        //add all the bosses attacks
        mm.AddMove("LokiMelee", new LokiMelee(this, 0.1f,"CardSlash"));
        mm.AddMove("LokiQuickMelee", new LokiMelee(this, 0.3f, "CardSlash"));
        mm.AddMove("LokiRange", new LokiRange(this, 5f));

        mm.AddMove("LokiFakeMelee", new LokiMelee(this, 0.1f, "FakeCardSlash"));
        mm.AddMove("LokiFakeQuickMelee", new LokiMelee(this, 0.1f, "FakeCardSlash"));
        mm.AddMove("LokiFakeRange", new LokiRange(this, 5f));

        mm.AddMove("LokiCloneMelee", new LokiClone(this, ATTACK.Melee));
        mm.AddMove("LokiCloneQuickMelee", new LokiClone(this, ATTACK.QuickMelee));
        mm.AddMove("LokiCloneRange", new LokiClone(this, ATTACK.Range));

        //start the statemachine with its first state

        hitboxManager.SetGroupMoveMachines(Harms, mm);
    }

    private void Update()
    {
        CurrentBossDebug();
    }
}
