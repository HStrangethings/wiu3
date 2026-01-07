using UnityEngine;

public class HitboxAnimEvents : MonoBehaviour
{
    public HitboxManager hitboxManager;

    // Called by Animation Event
    public void EnableHitbox(HitboxID id)
    {
        if (hitboxManager && id)
            hitboxManager.SetGroup(id, true);
    }

    // Called by Animation Event
    public void DisableHitbox(HitboxID id)
    {
        if (hitboxManager && id)
            hitboxManager.SetGroup(id, false);
    }
}