using UnityEngine;

public class CoralFan : BossMove
{
    private PhoenixBoss boss;

    [Header("Prefab")]
    private GameObject beamPrefab;

    [Header("Spawn / Shape")]
    private float wingOffset;        // left/right wing tip distance from boss center
    private float spawnForward;      // spawn slightly in front of boss
    private float crossOffset;       // how wide the X is near the player

    [Header("Swipe")]
    private float swipeDuration;     // how long to rotate into X
    private float holdAfterSwipe;    // how long to hold after swipe before disappearing
    private float rollAngle;         // 45 degrees tilt for X look
    private Vector3 rollAxisLocal;   // which local axis is the beam "forward" (usually Z)

    [Header("Combo")]
    private float comboCheckTime;    // fallback if you want a hard timeout

    private float timer;
    private bool comboChecked;

    private GameObject leftBeam;
    private GameObject rightBeam;

    private Quaternion leftStartRot;
    private Quaternion rightStartRot;
    private Quaternion leftTargetRot;
    private Quaternion rightTargetRot;

    public CoralFan(
        PhoenixBoss boss,
        GameObject beamPrefab,
        float wingOffset = 5.0f,
        float spawnForward = 1.5f,
        float crossOffset = 2.5f,
        float swipeDuration = 0.35f,
        float holdAfterSwipe = 0.1f,
        float rollAngle = 45f,
        Vector3? rollAxisLocal = null,   // default: Vector3.forward
        float comboCheckTime = 1.0f
    ) : base(boss)
    {
        this.boss = boss;
        this.beamPrefab = beamPrefab;

        this.wingOffset = wingOffset;
        this.spawnForward = spawnForward;
        this.crossOffset = crossOffset;

        this.swipeDuration = Mathf.Max(0.01f, swipeDuration);
        this.holdAfterSwipe = Mathf.Max(0f, holdAfterSwipe);

        this.rollAngle = rollAngle;
        this.rollAxisLocal = rollAxisLocal ?? Vector3.forward; // change to Vector3.right if your prefab is built along X

        this.comboCheckTime = comboCheckTime;
    }

    public override void Start()
    {
        timer = 0f;
        comboChecked = false;

        if (boss == null || beamPrefab == null)
        {
            Debug.LogError("CoralFan: boss or beamPrefab is null.");
            isFinished = true;
            return;
        }

        // Defensive: ensure player exists
        if (boss.player == null)
            boss.player = GameObject.FindGameObjectWithTag("Player");

        Vector3 forward = boss.transform.forward;
        Vector3 right = boss.transform.right;

        // Spawn at wing tips (approx)
        Vector3 leftSpawn = boss.transform.position - right * wingOffset + forward * spawnForward;
        Vector3 rightSpawn = boss.transform.position + right * wingOffset + forward * spawnForward;

        // Start straight forward
        leftStartRot = Quaternion.LookRotation(forward);
        rightStartRot = Quaternion.LookRotation(forward);

        // Aim points so the two slashes cross near the player
        Vector3 playerPos = boss.transform.position + boss.DistanceToPlayer();
        Vector3 lateral = boss.transform.right; // boss-relative; swap to camera-right if you prefer screen-space

        Vector3 leftAimPoint = playerPos + lateral * crossOffset;   // left beam aims to player's right
        Vector3 rightAimPoint = playerPos - lateral * crossOffset;  // right beam aims to player's left

        Vector3 leftDir = leftAimPoint - leftSpawn;
        Vector3 rightDir = rightAimPoint - rightSpawn;
        leftDir.y = 0f;
        rightDir.y = 0f;

        leftTargetRot = leftDir.sqrMagnitude > 0.001f ? Quaternion.LookRotation(leftDir.normalized) : leftStartRot;
        rightTargetRot = rightDir.sqrMagnitude > 0.001f ? Quaternion.LookRotation(rightDir.normalized) : rightStartRot;

        // Apply +/- 45 degree "tilt" so the swipe looks like an X
        // Roll around the beam's own forward axis (as defined by rollAxisLocal)
        leftTargetRot = ApplyRoll(leftTargetRot, +rollAngle);
        rightTargetRot = ApplyRoll(rightTargetRot, -rollAngle);

        // Instantiate beams
        leftBeam = Object.Instantiate(beamPrefab, leftSpawn, leftStartRot);
        rightBeam = Object.Instantiate(beamPrefab, rightSpawn, rightStartRot);

        // We are rotating them manually, so make physics not fight us
        MakeKinematic(leftBeam);
        MakeKinematic(rightBeam);

        // Enable boss hit reporting on these spawned objects (same pattern as your other projectiles)
        SetupBossHitReporting(leftBeam);
        SetupBossHitReporting(rightBeam);
    }

    public override void Execute()
    {
        timer += Time.deltaTime;

        // Keep attached to wing tips while swiping (optional but usually looks best)
        StickToWings();

        // Rotate from straight -> diagonal X over time
        float t = Mathf.Clamp01(timer / swipeDuration);
        float smoothT = t * t * (3f - 2f * t); // SmoothStep

        if (leftBeam != null) leftBeam.transform.rotation = Quaternion.Slerp(leftStartRot, leftTargetRot, smoothT);
        if (rightBeam != null) rightBeam.transform.rotation = Quaternion.Slerp(rightStartRot, rightTargetRot, smoothT);

        // Early combo decision if a hit is confirmed shortly after swipe starts
        if (!comboChecked && timer >= 0.2f && boss.mm.HitConfirmed(GetType()))
        {
            FinishAndComboCheck();
            return;
        }

        // Finish after swipe completes (+ optional tiny hold)
        if (!comboChecked && timer >= swipeDuration + holdAfterSwipe)
        {
            FinishAndComboCheck();
            return;
        }

        // Safety fallback
        if (!comboChecked && timer >= comboCheckTime)
        {
            FinishAndComboCheck();
        }
    }

    private void FinishAndComboCheck()
    {
        comboChecked = true;

        if (leftBeam != null) Object.Destroy(leftBeam);
        if (rightBeam != null) Object.Destroy(rightBeam);

        AnimEvent("comboCheck");
        isFinished = true;
    }

    private void StickToWings()
    {
        if (leftBeam == null && rightBeam == null) return;

        Vector3 forward = boss.transform.forward;
        Vector3 right = boss.transform.right;

        Vector3 leftPos = boss.transform.position - right * wingOffset + forward * spawnForward;
        Vector3 rightPos = boss.transform.position + right * wingOffset + forward * spawnForward;

        if (leftBeam != null) leftBeam.transform.position = leftPos;
        if (rightBeam != null) rightBeam.transform.position = rightPos;
    }

    private Quaternion ApplyRoll(Quaternion baseRot, float degrees)
    {
        // Convert local roll axis (usually forward) into world space for this rotation
        Vector3 rollAxisWorld = baseRot * rollAxisLocal.normalized;
        return Quaternion.AngleAxis(degrees, rollAxisWorld) * baseRot;
    }

    private void MakeKinematic(GameObject go)
    {
        if (go == null) return;
        var rb = go.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public override void End()
    {
        // Ensure cleanup even if interrupted
        if (leftBeam != null) Object.Destroy(leftBeam);
        if (rightBeam != null) Object.Destroy(rightBeam);
        base.End();
    }

    public override void AnimEvent(string evt)
    {
        switch (evt)
        {
            case "comboCheck":
                boss.BossMoveComboDetails(GetType(), out bool hit, out bool LOS, out float dist);

                // Replace these IDs with your real move keys
                string nextMoveId = "null";
                if (hit && dist < 12f) nextMoveId = boss.mm.Choose("PhoenixCharge", "EnergySlash", "null");
                else if (!hit && LOS) nextMoveId = boss.mm.Choose("LaserBeam", "EnergySlash", "null");
                else nextMoveId = "null";

                if (!string.IsNullOrEmpty(nextMoveId))
                    boss.mm.PlayMove(nextMoveId);

                break;
        }
    }

    public override BossMove Clone()
    {
        return new CoralFan(
            boss,
            beamPrefab,
            wingOffset,
            spawnForward,
            crossOffset,
            swipeDuration,
            holdAfterSwipe,
            rollAngle,
            rollAxisLocal,
            comboCheckTime
        );
    }
}