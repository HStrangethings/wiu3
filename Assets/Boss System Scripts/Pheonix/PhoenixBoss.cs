using UnityEngine;

public class PhoenixBoss : BossBehaviour
{
    public GameObject singleSlash;
    public GameObject crossSlash;
    public GameObject laserBeam;

    public override void Start()
    {
        base.Start();

        //  Add ALL boss states here
        sm.AddState(new PhoenixIdle(sm, this));
        sm.AddState(new PhoenixAttack(sm, this)); // <-- NEW

        //  Add ALL boss moves here
        mm.AddMove("EnergySlash", new EnergySlash(this, 50, 0));
        mm.AddMove("TripleSlash", new TripleSlash(this, 50, 0));
        mm.AddMove("CrossEnergySlash", new EnergySlash(this, 50, 1));
        mm.AddMove("TripleCrossEnergySlash", new TripleSlash(this, 50, 1));
        mm.AddMove("LaserBeam", new LaserBeam(this, 3f));

        //  Start the state machine
        sm.Initialize<PhoenixIdle>();
    }
}