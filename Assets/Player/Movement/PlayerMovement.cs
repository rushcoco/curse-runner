using System;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Input")] [SerializeField] private InputActionReference inputMoving;
    [SerializeField] private InputActionReference inputBoosting;
    [SerializeField] private InputActionReference inputJumping;

    [Header("Movement")] [SerializeField] private float movementSpeedStarting;
    [SerializeField] private float movementSpeedBoosting;
    [SerializeField] private float movementSpeedMax;
    [SerializeField] private float accelerationGrounded;
    [SerializeField] private float accelerationAir;
    [SerializeField] private float deceleration;

    [Header("Landing/Grounding")] [SerializeField]
    private float stickToGroundAtXVelocity;

    [SerializeField] private float snapToGroundAtXDistance;
    [SerializeField] private LayerMask groundLayer;

    [Header("Jump Heights (meters)")] [SerializeField]
    private float minJumpHeightWhenPlayerIsIdle;

    [SerializeField] private float minJumpHeightWhenPlayerIsWalking;
    [SerializeField] private float minJumpHeightWhenPlayerIsRunning;
    [SerializeField] private float maxJumpHeightWhenPlayerIsIdle;
    [SerializeField] private float maxJumpHeightWhenPlayerIsWalking;
    [SerializeField] private float maxJumpHeightWhenPlayerIsRunning;

    [Header("Jump Tuning")] [SerializeField]
    private float holdJumpButtonForXAmountOfSecondsForMaxJump;

    [SerializeField] private float coyoteTime; // The max time you are able to still jump, after leaving ground
    [SerializeField] private float jumpBuffer;
    [SerializeField] private float maxFallSpeed;
    [SerializeField] private float gravity;

    [Header("Movement State Thresholds (m/s)")] [SerializeField]
    private float walkSpeedThreshold;

    [SerializeField] private float runSpeedThreshold;

    private CharacterController characterSelf;
    private Vector3 velocity;
    private Vector3 groundNormal;
    private bool grounded;

    // Jump State
    private bool isJumpInputBeingHeld;
    private float lastGroundedTime;
    private float lastJumpPressedTime;
    private float lastJumpVelocityForMaxHeightJump; // i have no idea what its doing but it works
    private float lastMinimumJumpHeight;
    private float lastMaximumJumpHeight;
    private float jumpStartTime;

    // Boost State
    private bool boostPending;
    private Vector3 pendingBoostVelocity;
    private bool skipDecelThisFrame;

    private void Awake()
    {
        characterSelf = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        inputMoving.action.Enable();
        inputBoosting.action.Enable();
        inputJumping.action.Enable();

        inputJumping.action.started += OnJumpStarted;
        inputJumping.action.canceled += OnJumpCanceled;
        inputBoosting.action.performed += OnPerformedBoost;
    }

    private void OnDisable()
    {
        inputBoosting.action.performed -= OnPerformedBoost;
        inputJumping.action.started -= OnJumpStarted;
        inputJumping.action.canceled -= OnJumpCanceled;

        inputMoving.action.Disable();
        inputBoosting.action.Disable();
        inputJumping.action.Disable();
    }

    private void Start()
    {
        lastGroundedTime = float.NegativeInfinity;
        lastJumpPressedTime = float.NegativeInfinity;
        jumpStartTime = float.NegativeInfinity;

        boostPending = false;
        skipDecelThisFrame = false;
        isJumpInputBeingHeld = false;
    }


    private void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;
        skipDecelThisFrame = false;

        UpdateGround();

        Vector2 inputDirection = inputMoving.action.ReadValue<Vector2>();
        Vector3 wishedDirection = (transform.right * inputDirection.x + transform.forward * inputDirection.y);
        wishedDirection = Vector3.ClampMagnitude(wishedDirection, 1f);

        Vector3 currentPlanarVelocity = Vector3.ProjectOnPlane(velocity, Vector3.up);
        Vector3 wishedMovement = wishedDirection * movementSpeedMax;

        // Apply Boost
        if (boostPending)
        {
            currentPlanarVelocity = pendingBoostVelocity;
            boostPending = false;
            skipDecelThisFrame = true;
        }

        float accelerate = grounded ? accelerationGrounded : accelerationAir;
        float decelerate = grounded ? deceleration : 0f;

        // Apply Acceleration
        var deltaVelocity = wishedMovement - currentPlanarVelocity;
        var singleFrameAccelerationStep = Vector3.ClampMagnitude(deltaVelocity, accelerate * deltaTime);
        currentPlanarVelocity += singleFrameAccelerationStep;

        // Apply Deceleration, when grounded and no input (unless just boosted)
        if (!skipDecelThisFrame && grounded && wishedMovement.sqrMagnitude < 1e-4f) // what does 1e-4f stand for?
        {
            var drop = Mathf.Min(currentPlanarVelocity.magnitude, decelerate * deltaTime);
            currentPlanarVelocity = currentPlanarVelocity.magnitude > 1e-4f
                ? currentPlanarVelocity.normalized * (currentPlanarVelocity.magnitude - drop)
                : Vector3.zero;
        }

        // Apply Gravity
        velocity.y += -gravity * deltaTime;
        if (grounded && velocity.y < 0f)
            velocity.y = stickToGroundAtXVelocity;

        // Coyote
        bool pressedRecently = (Time.time - lastJumpPressedTime) <= Mathf.Max(jumpBuffer, deltaTime);
        bool canUseCoyote = grounded || (Time.time - lastGroundedTime) <= coyoteTime;
        if (pressedRecently && canUseCoyote)
        {
            Debug.Log("Jump Button entered coyote");
            TryStartJump(currentPlanarVelocity);
            lastJumpPressedTime = float.NegativeInfinity;
        }

        velocity.x = currentPlanarVelocity.x;
        velocity.z = currentPlanarVelocity.z;

        // Project planar motion onto ground plane for slope sticking
        /*
        if (grounded)
        {
            var alongGround = Vector3.ProjectOnPlane(new Vector3(velocity.x, 0f, velocity.z), groundNormal);
            velocity.x = alongGround.x;
            velocity.z = alongGround.z;
        */
        //}

        if (velocity.y < -maxFallSpeed)
            velocity.y = -maxFallSpeed;

        characterSelf.Move(velocity * deltaTime);
    }

    private void UpdateGround()
    {
        grounded = false;
        groundNormal = Vector3.up;
        
        Debug.Log("Update Ground entered");

        var origin = transform.position + Vector3.up * 0.05f; // why 0.05f? what does that number mean?
        var rayLength = characterSelf.height * 0.5f + snapToGroundAtXDistance;

        if (Physics.SphereCast(origin, characterSelf.radius * 0.95f, Vector3.down, out var hit, rayLength, groundLayer,
                QueryTriggerInteraction.Ignore))
        {
            Debug.Log("Found Ground?");
            var slope = Vector3.Angle(hit.normal, Vector3.up);
            if (slope <= characterSelf.slopeLimit + 0.5f) // what do these numbers mean?
            {
                grounded = true;
                groundNormal = hit.normal;
                lastGroundedTime = Time.time;
                Debug.Log("Ground is true");
            }
        }
    }

    private void OnJumpStarted(InputAction.CallbackContext context)
    {
        Debug.Log("Jump Button Held");
        isJumpInputBeingHeld = true;
        lastJumpPressedTime = Time.time;
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("Jump Button Let Go");
        isJumpInputBeingHeld = false;

        if (velocity.y > 0f && (Time.time - jumpStartTime < holdJumpButtonForXAmountOfSecondsForMaxJump))
        {
            var targetUpVelocity = Mathf.Sqrt(
                // Why is 0.01f used here?
                Mathf.Max(
                    movementSpeedStarting,
                    lastMinimumJumpHeight / Mathf.Max(
                        movementSpeedStarting,
                        lastMaximumJumpHeight)
                )
            ) * lastJumpVelocityForMaxHeightJump;

            velocity.y = Mathf.Min(velocity.y, targetUpVelocity);
        }
    }

    private void OnPerformedBoost(InputAction.CallbackContext context)
    {
        Debug.Log("Performed Boost");
        var selfForward = transform.forward;
        selfForward.y = 0f;
        if (selfForward.sqrMagnitude > 1e-6f) // What does 1e-6f mean?
            selfForward.Normalize();

        pendingBoostVelocity = selfForward * movementSpeedBoosting;
        boostPending = true;
    }

    private void TryStartJump(Vector3 currentPlanarVelocity)
    {
        if (velocity.y > movementSpeedStarting) return;

        GetJumpHeightsFromSpeed(currentPlanarVelocity.magnitude, out var minimumHeight, out var maximumHeight);

        float v0 = Mathf.Sqrt(2f * gravity * Mathf.Max(movementSpeedStarting, maximumHeight)); // Why 2f?
        velocity.y = v0;

        lastJumpVelocityForMaxHeightJump = v0;
        lastMinimumJumpHeight = minimumHeight;
        lastMaximumJumpHeight = maximumHeight;
        jumpStartTime = Time.time;

        grounded = false;
        lastGroundedTime = float.NegativeInfinity;
    }

    private void GetJumpHeightsFromSpeed(float planarSpeed, out float minimumHeight, out float maximumHeight)
    {
        switch (planarSpeed)
        {
            case var _ when planarSpeed < walkSpeedThreshold:
                minimumHeight = minJumpHeightWhenPlayerIsIdle;
                maximumHeight = maxJumpHeightWhenPlayerIsIdle;
                break;
            case var _ when planarSpeed < runSpeedThreshold:
                minimumHeight = minJumpHeightWhenPlayerIsWalking;
                maximumHeight = maxJumpHeightWhenPlayerIsWalking;
                break;
            default:
                minimumHeight = minJumpHeightWhenPlayerIsRunning;
                maximumHeight = maxJumpHeightWhenPlayerIsRunning;
                break;
        }
    }

    private void GetJumpHeightsNow(out float minimumHeight, out float maximumHeight)
    {
        var planarVelocityOfPlayer = characterSelf.velocity;
        planarVelocityOfPlayer.y = 0f;
        var playerMovementSpeed = planarVelocityOfPlayer.magnitude;

        switch (playerMovementSpeed)
        {
            case var _ when playerMovementSpeed < walkSpeedThreshold:
                minimumHeight = minJumpHeightWhenPlayerIsIdle;
                maximumHeight = maxJumpHeightWhenPlayerIsIdle;
                break;
            case var _ when playerMovementSpeed < runSpeedThreshold:
                minimumHeight = minJumpHeightWhenPlayerIsWalking;
                maximumHeight = maxJumpHeightWhenPlayerIsWalking;
                break;
            default:
                minimumHeight = minJumpHeightWhenPlayerIsRunning;
                maximumHeight = maxJumpHeightWhenPlayerIsRunning;
                break;
        }
    }

    private void GetJumpHeightsAtJump(out float minimumHeight, out float maximumHeight)
    {
        minimumHeight = lastMinimumJumpHeight;
        maximumHeight = lastMaximumJumpHeight;
    }
}
