using UnityEngine;

public class PoseidonBoss : BossBehaviour
{
    public GameObject waterBlastProj;
    public GameObject waterWaveProj;
    public GameObject boatShieldProj;
    public GameObject actualShield;

    // list of all hitboxes
    public HitboxID Harms;

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
        mm.AddMove("waterWave", new WaterWave(this, 10, 9.5f, 3f)); //default wave xSize is 9.5
        mm.AddMove("posMelee", new PosMeleeAttack(this)); //default wave xSize is 9.5
        mm.AddMove("boatShield", new BoatShield(this,50)); //default wave xSize is 9.5

        //start the statemachine with its first state
        sm.Initialize<PosIdleState>();

        hitboxManager.SetGroupMoveMachines(Harms, mm);
    }
}
