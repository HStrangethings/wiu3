using System;
using UnityEngine;

public class BossBehaviour : MonoBehaviour
{
    public BossStats boss;
    [HideInInspector]
    public BossStateMachine sm;
    [HideInInspector]
    public BossMoveMachine mm;

    //keep track of player
    [HideInInspector]
    public GameObject player;

    //own components
    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public HitboxManager hitboxManager;

    [SerializeField] LayerMask losBlockMask; // set this to "Environment/Walls" layers

    public virtual void Start()
    {
        sm = GetComponent<BossStateMachine>();
        mm = GetComponent<BossMoveMachine>();
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        hitboxManager = GetComponent<HitboxManager>();
    }

    public Vector3 DistanceToPlayer()
    {
        return player.transform.position - rb.position;
    }

    public Vector3 MoveToPlayer()
    {
        //Debug.Log("moving to player");
        Vector3 dir = (DistanceToPlayer()).normalized;
        Vector3 movement = dir * boss.speed;
        Vector3 finalMove = new Vector3(movement.x, 0, movement.z);
        return finalMove;
    }

    public Quaternion RotateToPlayer()
    {
        if (player == null) return Quaternion.identity;

        Vector3 dir = DistanceToPlayer();
        dir.y = 0;

        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            Quaternion slerpRot = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                4 * Time.deltaTime
            );
            return slerpRot;
        }
        return Quaternion.identity;
    }

    public bool HasLOS()
    {
        if (player == null) return false;

        Vector3 origin = transform.position;
        Vector3 target = player.transform.position;
        Vector3 toTarget = DistanceToPlayer();

        float dist = toTarget.magnitude;
        if (dist <= 0.001f) return true;

        Vector3 dir = toTarget / dist;

        // ignore triggers colliders
        if (Physics.Raycast(origin, dir, out RaycastHit hit, dist, losBlockMask, QueryTriggerInteraction.Ignore))
        {
            // Something is in the way
            return false;
        }

        // can see player
        return true;
    }

    public void BossMoveComboDetails(Type moveType,out bool hit, out bool LOS, out float dist)
    {
        hit = mm.HitConfirmed(moveType, 0.25f);
        LOS = HasLOS();
        Vector3 distF = DistanceToPlayer(); //naming abit wrong but wtv
        dist = distF.magnitude;
    }

    private void OnDrawGizmos()
    {
        // Set color
        Gizmos.color = Color.red;

        // Draw sphere around the boss
        Gizmos.DrawWireSphere(transform.position, boss.bossRad);
    }
}
