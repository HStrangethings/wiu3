using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamController : MonoBehaviour
{
    public CinemachineCamera freeLookCam;
    public CinemachineCamera lockOnCam;

    bool lastLock; // remembers previous state

    void Start()
    {
        lastLock = InputManager.camLockOn; // or however you access it
        Apply(lastLock);
    }

    void Update()
    {
        bool now = InputManager.camLockOn;
        if (now == lastLock) return;

        lastLock = now;
        Apply(now);
    }

    void Apply(bool locked)
    {
        freeLookCam.gameObject.SetActive(!locked);
        lockOnCam.gameObject.SetActive(locked);
    }
}
