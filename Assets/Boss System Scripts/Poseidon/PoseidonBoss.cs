using UnityEngine;

public class PoseidonBoss : BossBehaviour
{
    public GameObject waterBlastProj;
    public GameObject waterWaveProj;
    public GameObject boatShieldProj;
    public GameObject actualShield;
    public GameObject geyserWarning;
    public GameObject geyser;
    public GameObject rain;

    // list of all hitboxes
    public HitboxID Harms;

    public bool shielded = false;

    public override void Start()
    {
        base.Start();
        //add all the bosses states
        //pass in the statemachine and this boss instance to the state
        sm.AddState(new PosIdleState(sm,this));
        sm.AddState(new PosAttackState(sm,this));
        sm.AddState(new PoseidonSecondState(sm, this));
        sm.AddState(new PoseidonSecondStateIdle(sm, this));
        sm.AddState(new PoseidonSecondStateAttack(sm, this));

        //add all the bosses attacks
        mm.AddMove("waterBlast", new WaterBlast(this, 20));
        mm.AddMove("wideWaterBlast", new WideWaterBlast(this, 20));
        mm.AddMove("waterWave", new WaterWave(this, 10, 9.5f, 3f)); 
        mm.AddMove("posMelee", new PosMeleeAttack(this,0.1f));
        mm.AddMove("quickPosMelee", new PosMeleeAttack(this,0.3f)); 
        mm.AddMove("boatShield", new BoatShield(this,50));
        mm.AddMove("geyser", new WaterGeyser(this, 50, 5, 2, 10));
        mm.AddMove("rain", new PosRainMove(this));

        //start the statemachine with its first state
        sm.Initialize<PosIdleState>();

        hitboxManager.SetGroupMoveMachines(Harms, mm);
    }

    private void Update()
    {
        CurrentBossDebug();
        if (Input.GetKeyDown(KeyCode.P))
        {
            boss.health -= 50;
            boss.RecalcHealth01();
        }
    }
}
