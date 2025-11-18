using UnityEngine;

public class PoseidonBoss : BossBehaviour
{
    public override void Start()
    {
        base.Start();
        //add all the bosses states
        //pass in the statemachine and this boss instance to the state
        sm.AddState(new PosIdleState(sm,this));
        sm.AddState(new PosAttackState(sm,this));

        //add all the bosses attacks
        mm.AddMove("waterBlast", new WaterBlast());

        //start the statemachine with its first state
        sm.Initialize<PosIdleState>();
    }

    private void Update()
    {
    }
}
