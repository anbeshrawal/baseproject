using UnityEngine;
using System.Collections.Generic;

public class FeatherFollow : MonoBehaviour
{
    #region Feather State Enum
    public enum FeatherState { Idle, Walk, Run, Jump, Landing }
    #endregion

    [Header("Follow Settings")]
    [Tooltip("How many seconds the feather lags behind the player's movement")]
    [Range(0.05f, 2f)]
    public float followDelay = 0.3f;

    [Header("Smoothing")]
    [Tooltip("Additional smoothing speed for the feather to catch up. Higher = snappier.")]
    public float smoothSpeed = 8f;

    [Header("State Transition")]
    [Tooltip("How fast the feather blends between state motions. Higher = faster transition.")]
    public float stateTransitionSpeed = 5f;

    #region Idle Motion Settings
    [Header("Idle Motion — Perlin Drift")]
    [Tooltip("Speed of Perlin noise sampling for idle drift")]
    public float idleDriftSpeed = 0.8f;

    [Tooltip("Maximum drift distance from resting position")]
    public float idleDriftAmount = 0.12f;

    [Tooltip("How quickly the feather returns toward resting position between drifts")]
    public float idleReturnSpeed = 2f;

    [Header("Idle Motion — Wandering")]
    [Tooltip("Maximum radius the feather can wander from the player visibility area")]
    public float idleWanderRadius = 3.5f;

    [Tooltip("Speed at which the feather wanders from point to point")]
    public float idleWanderSpeed = 1.2f;

    [Tooltip("Minimum time (seconds) to stay at a wander point before choosing a new one")]
    public float idleWanderIntervalMin = 2f;

    [Tooltip("Maximum time (seconds) to stay at a wander point before choosing a new one")]
    public float idleWanderIntervalMax = 5f;

    [Tooltip("How slowly the feather blends back to the player when the player starts moving")]
    public float idleReturnTransitionSpeed = 1.5f;
    #endregion

    #region Walk Motion Settings
    [Header("Walk Motion — Wave Oscillation")]
    [Tooltip("Primary vertical wave speed")]
    public float walkWaveSpeed = 5f;

    [Tooltip("Primary vertical wave amplitude")]
    public float walkWaveAmount = 0.15f;

    [Tooltip("Horizontal sway amount via Perlin noise")]
    public float walkSwayAmount = 0.08f;

    [Tooltip("Speed of horizontal sway Perlin sampling")]
    public float walkSwaySpeed = 1.5f;
    #endregion

    #region Run Motion Settings
    [Header("Run Motion — Amplified Wave + Turbulence")]
    [Tooltip("Primary vertical wave speed during run")]
    public float runWaveSpeed = 8f;

    [Tooltip("Primary vertical wave amplitude during run")]
    public float runWaveAmount = 0.22f;

    [Tooltip("Additional Perlin turbulence intensity")]
    public float runTurbulence = 0.15f;

    [Tooltip("Perlin turbulence sampling speed")]
    public float runTurbulenceSpeed = 3f;
    #endregion

    #region Jump Motion Settings
    [Header("Jump Motion — Swirl Orbit")]
    [Tooltip("Base swirl rotation speed")]
    public float jumpSwirlSpeed = 6f;

    [Tooltip("Swirl orbit radius")]
    public float jumpSwirlRadius = 0.25f;

    [Tooltip("Perlin perturbation scale on swirl")]
    public float jumpPerlinScale = 0.1f;

    [Tooltip("How much the swirl speeds up/down randomly")]
    public float jumpSpeedVariation = 2f;
    #endregion

    #region Landing Settings
    [Header("Landing Transition — Dampening")]
    [Tooltip("Time in seconds for the feather to settle after landing")]
    public float landingDampTime = 0.5f;
    #endregion

    [Header("Blur / Motion Trail Effect")]
    [Tooltip("Enable the ghost/blurry trail effect when moving")]
    public bool enableBlurTrail = true;

    [Tooltip("How long each ghost after-image lasts before fading out completely")]
    public float trailLifetime = 0.3f;

    [Tooltip("Time (in seconds) between spawning trail clones")]
    public float trailSpawnRate = 0.05f;

    [Tooltip("Initial transparency/alpha of the trail clones (0 to 1)")]
    [Range(0f, 1f)]
    public float startAlpha = 0.4f;

    [Tooltip("Color tint for the trail clones")]
    public Color trailColor = Color.white;

    [Tooltip("Minimum movement speed of the feather to trigger trail generation")]
    public float minSpeedForTrail = 0.5f;

    // Auto-detected from the parent before un-parenting
    private Transform player;
    private Vector3 offset;
    private SpriteRenderer mySR;

    // Ghost spawning timer
    private float spawnTimer;
    private Vector3 lastPosition;

    // Stores past player positions with timestamps
    private List<PositionRecord> positionHistory = new List<PositionRecord>();

    // State tracking
    private StateManager playerSM;
    private FeatherState featherState = FeatherState.Idle;
    private FeatherState previousFeatherState = FeatherState.Idle;

    // Motion blending
    private Vector3 currentMotionOffset = Vector3.zero;
    private Vector3 targetMotionOffset = Vector3.zero;

    // Perlin noise seeds (randomized per instance so multiple feathers don't move in sync)
    private float perlinSeedX;
    private float perlinSeedY;
    private float perlinSeedSwirl;

    // Jump swirl accumulator (continuous angle for smooth orbit)
    private float swirlAngle = 0f;

    // Landing dampening
    private float landingTimer = 0f;
    private Vector3 landingStartOffset = Vector3.zero;

    // Track if the player was jumping last frame (to detect landing)
    private bool wasJumping = false;

    // Wandering variables (Idle)
    private Vector3 currentIdleWanderOffset = Vector3.zero;
    private Vector3 idleWanderTargetOffset = Vector3.zero;
    private float idleWanderTimer = 0f;
    private bool isReturningFromIdle = false;

    private struct PositionRecord
    {
        public float time;
        public Vector3 position;

        public PositionRecord(float time, Vector3 position)
        {
            this.time = time;
            this.position = position;
        }
    }

    void Awake()
    {
        // Safety fallback for Unity serialization assigning 0 to new fields on existing instances in the scene
        if (stateTransitionSpeed == 0f)
        {
            stateTransitionSpeed = 5f;
            idleDriftSpeed = 0.8f;
            idleDriftAmount = 0.12f;
            idleReturnSpeed = 2f;
            walkWaveSpeed = 5f;
            walkWaveAmount = 0.15f;
            walkSwayAmount = 0.08f;
            walkSwaySpeed = 1.5f;
            runWaveSpeed = 8f;
            runWaveAmount = 0.22f;
            runTurbulence = 0.15f;
            runTurbulenceSpeed = 3f;
            jumpSwirlSpeed = 6f;
            jumpSwirlRadius = 0.25f;
            jumpPerlinScale = 0.1f;
            jumpSpeedVariation = 2f;
            landingDampTime = 0.5f;
        }

        // Separate safety checks for the new wander fields in case they serialize as 0 on existing objects in the scene
        if (idleWanderRadius == 0f) idleWanderRadius = 3.5f;
        if (idleWanderSpeed == 0f) idleWanderSpeed = 1.2f;
        if (idleWanderIntervalMin == 0f) idleWanderIntervalMin = 2f;
        if (idleWanderIntervalMax == 0f) idleWanderIntervalMax = 5f;
        if (idleReturnTransitionSpeed == 0f) idleReturnTransitionSpeed = 1.5f;
    }

    void Start()
    {
        mySR = GetComponent<SpriteRenderer>();
        if (mySR == null)
        {
            Debug.LogError("FeatherFollow: No SpriteRenderer found on this GameObject!");
        }

        // Randomize Perlin seeds so each feather instance drifts differently
        perlinSeedX = Random.Range(0f, 1000f);
        perlinSeedY = Random.Range(0f, 1000f);
        perlinSeedSwirl = Random.Range(0f, 1000f);

        // Auto-detect: find StateManager in parent hierarchy (handles nested child structures)
        playerSM = GetComponentInParent<StateManager>();
        if (playerSM != null)
        {
            player = playerSM.transform;

            // Record local offset relative to the player root (StateManager transform)
            offset = player.InverseTransformPoint(transform.position);

            // Un-parent at runtime so we can control position independently
            transform.SetParent(null);

            Debug.Log("FeatherFollow: Auto-detected StateManager on " + player.name + " with root-relative offset " + offset);
        }
        else if (transform.parent != null)
        {
            player = transform.parent;
            offset = transform.localPosition;
            transform.SetParent(null);
            Debug.LogWarning("FeatherFollow: No StateManager found on parent or hierarchy of " + name + ". Feather state system disabled.");
        }
        else
        {
            Debug.LogError("FeatherFollow: This GameObject has no parent in hierarchy! " +
                "It should be a child of the Player in the hierarchy.");
            return;
        }

        // Initialize history with current player position
        for (int i = 0; i < 10; i++)
        {
            positionHistory.Add(new PositionRecord(Time.time, player.position));
        }

        lastPosition = transform.position;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // --- State Detection ---
        DetectFeatherState();

        // --- Delayed Follow (existing logic) ---
        positionHistory.Add(new PositionRecord(Time.time, player.position));

        float targetTime = Time.time - followDelay;
        Vector3 delayedPosition = GetPositionAtTime(targetTime);

        // Apply both the player's rotation and scale to the offset to correctly mirror and size the distance
        Vector3 targetPosition = delayedPosition + (player.TransformPoint(offset) - player.position);

        // --- State-Aware Motion ---
        targetMotionOffset = ComputeStateMotion();

        // Smooth blend the motion offset for seamless transitions
        float blendSpeed = stateTransitionSpeed;
        if (featherState == FeatherState.Walk || featherState == FeatherState.Run)
        {
            if (previousFeatherState == FeatherState.Idle || isReturningFromIdle)
            {
                isReturningFromIdle = true;
                blendSpeed = idleReturnTransitionSpeed;

                // If we've successfully returned close to the target, reset the flag
                if (Vector3.Distance(currentMotionOffset, targetMotionOffset) < 0.15f)
                {
                    isReturningFromIdle = false;
                }
            }
        }
        else
        {
            isReturningFromIdle = false;
        }

        currentMotionOffset = Vector3.Lerp(currentMotionOffset, targetMotionOffset, Time.deltaTime * blendSpeed);

        // Apply the motion offset on top of the follow target
        targetPosition += currentMotionOffset;

        // Smoothly move the feather toward the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);

        // --- Ghost Trail (existing logic) ---
        if (enableBlurTrail && mySR != null)
        {
            float currentSpeed = (transform.position - lastPosition).magnitude / Time.deltaTime;

            if (currentSpeed > minSpeedForTrail)
            {
                spawnTimer += Time.deltaTime;
                if (spawnTimer >= trailSpawnRate)
                {
                    SpawnGhost();
                    spawnTimer = 0f;
                }
            }
            else
            {
                spawnTimer = 0f;
            }
        }

        lastPosition = transform.position;

        // Clean up old history entries (keep up to 2x the delay)
        float cutoffTime = Time.time - (followDelay * 2f);
        while (positionHistory.Count > 2 && positionHistory[0].time < cutoffTime)
        {
            positionHistory.RemoveAt(0);
        }
    }

    #region State Detection

    /// <summary>
    /// Maps the player's current state to the feather's FeatherState.
    /// Detects landing by tracking the Jump→Grounded transition.
    /// </summary>
    private void DetectFeatherState()
    {
        if (playerSM == null)
        {
            featherState = FeatherState.Idle;
            return;
        }

        previousFeatherState = featherState;
        BaseStates playerState = playerSM.CurrentState;

        if (playerState is Jump)
        {
            featherState = FeatherState.Jump;
            wasJumping = true;
        }
        else if (wasJumping && !(playerState is Jump))
        {
            // Player just landed — enter landing transition
            featherState = FeatherState.Landing;
            wasJumping = false;
            landingTimer = landingDampTime;
            landingStartOffset = currentMotionOffset;
        }
        else if (featherState == FeatherState.Landing)
        {
            // Continue landing dampening until timer expires
            landingTimer -= Time.deltaTime;
            if (landingTimer <= 0f)
            {
                // Landing complete, resolve to current ground state
                featherState = ResolveGroundState(playerState);
            }
        }
        else
        {
            featherState = ResolveGroundState(playerState);
        }

        // Reset swirl angle when entering Jump for a fresh orbit
        if (featherState == FeatherState.Jump && previousFeatherState != FeatherState.Jump)
        {
            swirlAngle = 0f;
        }

        // Initialize wander offset to current motion offset when entering Idle
        if (featherState == FeatherState.Idle && previousFeatherState != FeatherState.Idle)
        {
            currentIdleWanderOffset = currentMotionOffset;
            idleWanderTargetOffset = currentMotionOffset;
            idleWanderTimer = 0f; // Force pick a new target immediately
        }
    }

    /// <summary>
    /// Determines the feather state for grounded player states.
    /// </summary>
    private FeatherState ResolveGroundState(BaseStates playerState)
    {
        if (playerState is Run)
            return FeatherState.Run;
        else if (playerState is Walk)
            return FeatherState.Walk;
        else
            return FeatherState.Idle;
    }

    #endregion

    #region Per-State Motion Computation

    /// <summary>
    /// Routes to the correct motion method based on current feather state.
    /// </summary>
    private Vector3 ComputeStateMotion()
    {
        switch (featherState)
        {
            case FeatherState.Idle:
                return ComputeIdleMotion();
            case FeatherState.Walk:
                return ComputeWalkMotion();
            case FeatherState.Run:
                return ComputeRunMotion();
            case FeatherState.Jump:
                return ComputeJumpMotion();
            case FeatherState.Landing:
                return ComputeLandingMotion();
            default:
                return Vector3.zero;
        }
    }

    /// <summary>
    /// Idle: Wanders organically to random points within a visibility radius,
    /// with micro-drifting for a floaty, "alive" feel.
    /// </summary>
    private Vector3 ComputeIdleMotion()
    {
        // Update the timer for picking new wander locations
        idleWanderTimer -= Time.deltaTime;
        if (idleWanderTimer <= 0f)
        {
            // Pick a random target offset within a circle
            Vector2 randomCircle = Random.insideUnitCircle * idleWanderRadius;
            idleWanderTargetOffset = new Vector3(randomCircle.x, randomCircle.y, 0f);

            // Randomize the timer for the next position
            idleWanderTimer = Random.Range(idleWanderIntervalMin, idleWanderIntervalMax);
        }

        // Smoothly steer the base wander offset towards the target offset
        currentIdleWanderOffset = Vector3.MoveTowards(currentIdleWanderOffset, idleWanderTargetOffset, idleWanderSpeed * Time.deltaTime);

        // Add secondary small Perlin noise drift so it floats organically
        float time = Time.time * idleDriftSpeed;
        float noiseX = (Mathf.PerlinNoise(perlinSeedX + time, 0f) - 0.5f) * 2f * idleDriftAmount;
        float noiseY = (Mathf.PerlinNoise(0f, perlinSeedY + time) - 0.5f) * 2f * idleDriftAmount;

        Vector3 microDrift = new Vector3(noiseX, noiseY, 0f);

        return currentIdleWanderOffset + microDrift;
    }

    /// <summary>
    /// Walk: Multi-frequency sine wave for vertical bob + Perlin horizontal sway.
    /// Two sine frequencies prevent a robotic repeating pattern.
    /// </summary>
    private Vector3 ComputeWalkMotion()
    {
        float time = Time.time;

        // Primary wave + secondary harmonic for organic feel
        float waveY = Mathf.Sin(time * walkWaveSpeed) * walkWaveAmount
                     + Mathf.Sin(time * walkWaveSpeed * 1.7f) * (walkWaveAmount * 0.3f);

        // Horizontal sway driven by Perlin (not sine) so it's non-repeating
        float swayX = (Mathf.PerlinNoise(perlinSeedX + time * walkSwaySpeed, 0f) - 0.5f) * 2f * walkSwayAmount;

        return new Vector3(swayX, waveY, 0f);
    }

    /// <summary>
    /// Run: Amplified wave with added Perlin turbulence in both axes.
    /// The feather feels buffeted by wind/speed.
    /// </summary>
    private Vector3 ComputeRunMotion()
    {
        float time = Time.time;

        // Faster, larger primary wave + harmonic
        float waveY = Mathf.Sin(time * runWaveSpeed) * runWaveAmount
                     + Mathf.Sin(time * runWaveSpeed * 1.4f) * (runWaveAmount * 0.4f);

        // Perlin turbulence layered on top for chaotic feel
        float turbX = (Mathf.PerlinNoise(perlinSeedX + time * runTurbulenceSpeed, 0f) - 0.5f) * 2f * runTurbulence;
        float turbY = (Mathf.PerlinNoise(0f, perlinSeedY + time * runTurbulenceSpeed) - 0.5f) * 2f * runTurbulence;

        return new Vector3(turbX, waveY + turbY, 0f);
    }

    /// <summary>
    /// Jump: Swirling elliptical orbit with Perlin perturbation.
    /// The feather spins around the rest position, lifted by air currents.
    /// </summary>
    private Vector3 ComputeJumpMotion()
    {
        float time = Time.time;

        // Variable swirl speed via Perlin for non-mechanical orbits
        float speedMod = 1f + (Mathf.PerlinNoise(perlinSeedSwirl + time * 0.5f, 0f) - 0.5f) * jumpSpeedVariation;
        swirlAngle += jumpSwirlSpeed * speedMod * Time.deltaTime;

        // Elliptical orbit (slightly wider in X than Y)
        float swirlX = Mathf.Cos(swirlAngle) * jumpSwirlRadius * 1.2f;
        float swirlY = Mathf.Sin(swirlAngle) * jumpSwirlRadius;

        // Perlin perturbation for organic randomness
        float pertX = (Mathf.PerlinNoise(perlinSeedX + time * 2f, 0f) - 0.5f) * 2f * jumpPerlinScale;
        float pertY = (Mathf.PerlinNoise(0f, perlinSeedY + time * 2f) - 0.5f) * 2f * jumpPerlinScale;

        return new Vector3(swirlX + pertX, swirlY + pertY, 0f);
    }

    /// <summary>
    /// Landing: Smoothly damps the motion offset from whatever it was at landing toward zero.
    /// Uses the landing timer to produce a natural settling curve.
    /// </summary>
    private Vector3 ComputeLandingMotion()
    {
        if (landingDampTime <= 0f)
            return Vector3.zero;

        // Progress from 1 (just landed) to 0 (fully settled)
        float progress = Mathf.Clamp01(landingTimer / landingDampTime);

        // Ease-out curve for natural deceleration
        float easedProgress = progress * progress;

        // Damp from the offset captured at landing toward zero
        return landingStartOffset * easedProgress;
    }

    #endregion

    #region Ghost Trail

    private void SpawnGhost()
    {
        // Create an empty GameObject for the ghost trail clone
        GameObject ghostObj = new GameObject(gameObject.name + "_GhostTrail");
        
        // Match feather's transform settings
        ghostObj.transform.position = transform.position;
        ghostObj.transform.rotation = transform.rotation;
        ghostObj.transform.localScale = transform.localScale;

        // Copy sprite renderer details
        SpriteRenderer ghostSR = ghostObj.AddComponent<SpriteRenderer>();
        ghostSR.sprite = mySR.sprite;
        ghostSR.sortingLayerID = mySR.sortingLayerID;
        ghostSR.sortingOrder = mySR.sortingOrder - 1; // Render behind the actual feather
        
        // Add the fade logic
        FeatherGhost ghostScript = ghostObj.AddComponent<FeatherGhost>();
        ghostScript.Init(trailLifetime, startAlpha, trailColor);
    }

    #endregion

    #region Position History (Delayed Follow)

    /// <summary>
    /// Interpolates the player's position at a given past time using recorded history.
    /// </summary>
    private Vector3 GetPositionAtTime(float time)
    {
        if (time <= positionHistory[0].time)
        {
            return positionHistory[0].position;
        }

        if (time >= positionHistory[positionHistory.Count - 1].time)
        {
            return positionHistory[positionHistory.Count - 1].position;
        }

        for (int i = 0; i < positionHistory.Count - 1; i++)
        {
            if (positionHistory[i].time <= time && positionHistory[i + 1].time >= time)
            {
                float segmentDuration = positionHistory[i + 1].time - positionHistory[i].time;
                if (segmentDuration <= 0f) return positionHistory[i].position;

                float t = (time - positionHistory[i].time) / segmentDuration;
                return Vector3.Lerp(positionHistory[i].position, positionHistory[i + 1].position, t);
            }
        }

        return positionHistory[positionHistory.Count - 1].position;
    }

    #endregion
}
