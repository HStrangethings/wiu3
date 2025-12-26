using UnityEngine;

public class BossBehaviour : MonoBehaviour
{
    public BossStats boss;
    [HideInInspector]
    public BossStateMachine sm;
    [HideInInspector]
    public BossMoveMachine mm;

    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public Rigidbody rb;

    [HideInInspector]
    public Animator animator;

    [HideInInspector]
    public HitboxManager hitboxManager;

    public virtual void Start()
    {
        sm = GetComponent<BossStateMachine>();
        mm = GetComponent<BossMoveMachine>();
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        hitboxManager = GetComponent<HitboxManager>();
    }
    public Vector3 MoveToPlayer()
    {
        //Debug.Log("moving to player");
        Vector3 dir = (player.transform.position - rb.position).normalized;
        Vector3 movement = dir * boss.speed;
        Vector3 finalMove = new Vector3(movement.x, 0, movement.z);
        return finalMove;
    }

    public Quaternion RotateToPlayer()
    {
        if (player == null) return Quaternion.identity;

        Vector3 dir = player.transform.position - transform.position;
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
}
