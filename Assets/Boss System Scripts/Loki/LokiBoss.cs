using UnityEngine;

public class LokiBoss : BossBehaviour
{

    // list of all hitboxes
    public HitboxID Harms;

    public bool shielded = false;

    public override void Start()
    {
        base.Start();
        //add all the bosses states
        //pass in the statemachine and this boss instance to the state
        sm.AddState(new LokiIdleState(sm,this));
        sm.AddState(new LokiAttackState(sm,this));

        //add all the bosses attacks
        //mm.AddMove("move name", new MoveClass(this));

        //start the statemachine with its first state
        sm.Initialize<LokiIdleState>();

        hitboxManager.SetGroupMoveMachines(Harms, mm);
    }
}
