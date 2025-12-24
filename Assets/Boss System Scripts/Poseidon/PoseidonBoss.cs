using UnityEngine;

public class PoseidonBoss : BossBehaviour
{

    //TODO: add teleporting next

    public GameObject waterBlastProj;
    public override void Start()
    {
        base.Start();
        //add all the bosses states
        //pass in the statemachine and this boss instance to the state
        sm.AddState(new PosIdleState(sm,this));
        sm.AddState(new PosAttackState(sm,this));

        //add all the bosses attacks
        mm.AddMove("waterBlast", new WaterBlast(this, 20));
        mm.AddMove("wideWaterBlast", new WideWaterBlast(this, 20));

        //start the statemachine with its first state
        sm.Initialize<PosIdleState>();
    }


    private void OnDrawGizmos()
    {
        // Set color
        Gizmos.color = Color.red;

        // Draw sphere around the boss
        Gizmos.DrawWireSphere(transform.position, 2);
    }
}
