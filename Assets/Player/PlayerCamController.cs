using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamController : MonoBehaviour
{
    public CinemachineCamera freeLookCam;
    public CinemachineCamera lockOnCam;
    public Transform target;

    bool lastLock; // remembers previous state
    public bool isLocked = false;

    void Start()
    {
        InputManager.camToggle += Apply;
    }

    private void OnDisable()
    {
        InputManager.camToggle -= Apply;
    }

    void Update()
    {
    }

    void Apply(bool locked)
    {
        if (target == null) {
            isLocked = false;
            freeLookCam.gameObject.SetActive(true);
            lockOnCam.gameObject.SetActive(false);
            return;
        }
        isLocked = locked;
        freeLookCam.gameObject.SetActive(!locked);
        lockOnCam.gameObject.SetActive(locked);
    }
}
