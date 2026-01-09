using UnityEngine;

public class PlayerApplyRootMotion : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator animator;

    // Optional: how much to allow vertical root motion (usually 0)
    [SerializeField] private bool allowVertical = false;

    private void Reset()
    {
        controller = GetComponentInParent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void OnAnimatorMove()
    {
        if (controller == null || animator == null) return;

        // Root motion displacement this frame (already in world space)
        Vector3 delta = animator.deltaPosition;

        // Usually you do NOT want animation to drive Y for a CC (gravity/jumps do)
        if (!allowVertical) delta.y = 0f;

        // Apply motion through the controller (expects per-frame displacement)
        controller.Move(delta);

        // Apply root rotation
        transform.parent.rotation *= animator.deltaRotation;
    }
}
