using System;
using System.Text;
using TMPro;
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
    public GameObject currPlayer;
    private PlayerWeaponController playerWeapon;

    //own components
    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public HitboxManager hitboxManager;

    [SerializeField] LayerMask losBlockMask; // set this to "Environment/Walls" layers

    public TextMeshProUGUI DebugText;

    public virtual void Start()
    {
        sm = GetComponent<BossStateMachine>();
        mm = GetComponent<BossMoveMachine>();
        currPlayer = GameObject.FindGameObjectWithTag("Player");
        playerWeapon = currPlayer.GetComponentInChildren<PlayerWeaponController>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        hitboxManager = GetComponent<HitboxManager>();
        mm.comboFin += GetComboFin;
    }

    private void OnDisable()
    {
        mm.comboFin -= GetComboFin;
    }

    public Vector3 DistanceToPlayer()
    {
        return currPlayer.transform.position - rb.position;
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
        if (currPlayer == null) return Quaternion.identity;

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
        if (currPlayer == null) return false;

        Vector3 origin = transform.position;
        Vector3 target = currPlayer.transform.position;
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

    public void GetComboFin()
    {
        sm.ComboFin();
    }

    public bool IsPlayerAttacking()
    {
        if (playerWeapon.isAttacking == true)
        {
            return true;
        }
        return false;
    }

    public float ToPlayerAngle()
    {
        Vector3 toPlayer = currPlayer.transform.position - transform.position;
        toPlayer.y = 0f; // ignore vertical difference

        float angle = Vector3.SignedAngle(
            transform.forward,
            toPlayer.normalized,
            Vector3.up
        );

        return angle;
    }

    public void CurrentBossDebug()
    {
        //print out current states. they dont have names, just print out class name
        //then print out current queue of moves
        //assign this string to a TextMeshProUGUI.text

        Debug.Log(sm.currentState.GetType().Name);
        //var sb = new StringBuilder(512);
        //sb.AppendLine("States:");
        //sb.AppendLine($"  {sm.currentState.GetType().Name}");
        //if (mm.currentMove != null)
        //{
        //    sb.AppendLine($"Current Move ({mm.currentMove.GetType().Name}");
        //}
        //else { sb.AppendLine($"Current Move (null)"); }
        //    sb.AppendLine($"Queued Moves ({mm.queuedMoves.Count})");
        //if (mm.queuedMoves.Count == 0){
        //    sb.AppendLine("  (empty)");
        //}
        //else
        //{
        //    int i = 0;
        //    foreach (var move in mm.queuedMoves) // does NOT dequeue
        //    {
        //        sb.AppendLine($"  {i}: {move}");
        //        i++;
        //    }
        //}
        //sb.AppendLine($"AngleToPlayer: {ToPlayerAngle()}");

        //DebugText.text = sb.ToString();
    }

    private void OnDrawGizmos()
    {
        // Set color
        Gizmos.color = Color.red;

        // Draw sphere around the boss
        Gizmos.DrawWireSphere(transform.position, boss.bossRad);

        if (currPlayer == null) return;

        Vector3 origin = transform.position;
        Vector3 target = currPlayer.transform.position;

        bool canSee = HasLOS();

        Gizmos.color = canSee ? Color.green : Color.red;
        Gizmos.DrawLine(origin, target);

        // Optional: draw a small marker at player position
        Gizmos.DrawWireSphere(target, 0.2f);
    }
}
