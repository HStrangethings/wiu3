using UnityEngine;

public class PhoenixBoss : BossBehaviour
{
    public GameObject singleSlash;
    public GameObject crossSlash;
    public GameObject laserBeam;
    public override void Start()
    {
        base.Start();
        //add all the bosses states
        //pass in the statemachine and this boss instance to the state
        sm.AddState(new PhoenixIdle(sm, this));

        //add all the bosses attacks
        mm.AddMove("EnergySlash", new EnergySlash(this, 50, 0));
        mm.AddMove("TripleSlash", new TripleSlash(this, 50, 0));
        mm.AddMove("CrossEnergySlash", new EnergySlash(this, 50, 1));
        mm.AddMove("TripleCrossEnergySlash", new TripleSlash(this, 50, 1));
        mm.AddMove("LaserBeam", new LaserBeam(this, 3f));
        mm.AddMove("PhoenixCharge", new PhoenixCharge(this, 25f, 1.5f, 2.5f));
        //mm.AddMove("posMelee", new PosMeleeAttack(this)); //default wave xSize is 9.5
        //mm.AddMove("boatShield", new BoatShield(this, 50)); //default wave xSize is 9.5

        //start the statemachine with its first state
        sm.Initialize<PhoenixIdle>();

       // hitboxManager.SetGroupMoveMachines(Harms, mm);
    }
}
