using UnityEngine;

public class BossBehaviour : MonoBehaviour
{
    public BossStats boss;
    public BossStateMachine sm;
    public BossMoveMachine mm;

    public virtual void Start()
    {
        sm = GetComponent<BossStateMachine>();
        mm = GetComponent<BossMoveMachine>();
    }
}
