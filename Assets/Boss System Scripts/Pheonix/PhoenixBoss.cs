using UnityEngine;

public class PhoenixBoss : BossBehaviour
{
    public GameObject singleSlash;
    public GameObject crossSlash;
    public GameObject laserBeam;
    public GameObject coralFanBeamPrefab;
    public bool phase2 = false;
    public override void Start()
    {
        base.Start();
        //add all the bosses states
        //pass in the statemachine and this boss instance to the state
        //sm.AddState(new PhoenixIdle(sm, this));
        sm.AddState(new PhoenixIdleState(sm, this));
        sm.AddState(new PhoenixAttackState(sm, this));
        sm.AddState(new PhoenixSecondState(sm, this));

        //add all the bosses attacks

        mm.AddMove("EnergySlash", new EnergySlash(this, 50, 0));
        mm.AddMove("EnergySlash2", new EnergySlash(this, 100, 0));// i can change the pararmeters to enhance it
        mm.AddMove("TripleSlash", new TripleSlash(this, 50, 0));
        mm.AddMove("CrossEnergySlash", new EnergySlash(this, 50, 1));
        mm.AddMove("TripleCrossEnergySlash", new TripleSlash(this, 50, 1));
        mm.AddMove("LaserBeam", new LaserBeam(this, 3f));
        mm.AddMove("PhoenixCharge", new PhoenixCharge(this, 25f, 1.5f, 2.5f));
        mm.AddMove("CoralFan", new CoralFan(this, coralFanBeamPrefab, crossOffset: 8f, swipeDuration: 0.7f, rollAngle: 45f));
        //mm.AddMove("posMelee", new PosMeleeAttack(this)); //default wave xSize is 9.5
        //mm.AddMove("boatShield", new BoatShield(this, 50)); //default wave xSize is 9.5

        //start the statemachine with its first state
        //sm.Initialize<PhoenixIdle>();
        sm.Initialize<PhoenixIdleState>();

        // hitboxManager.SetGroupMoveMachines(Harms, mm);
    }
}
