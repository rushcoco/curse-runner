using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementB : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference inputMoving;
    [SerializeField] private InputActionReference inputBoosting;
    [SerializeField] private InputActionReference inputJumping;

    [Header("Movement")] 
    [SerializeField] private float movementSpeedWalk;
    [SerializeField] private float movementSpeedLightJog;
    [SerializeField] private float movementSpeedJog;
    [SerializeField] private float movementSpeedLightRun;
    [SerializeField] private float movementSpeedRun;
    [SerializeField] private float movementSpeedBoosted;

    [Header("Acceleration Rates")]
    [SerializeField] private float rateOfAccelerationTowardsLightJog;
    [SerializeField] private float rateOfAccelerationTowardsJog;
    [SerializeField] private float rateOfAccelerationTowardsLightRun;
    [SerializeField] private float rateOfAccelerationTowardsRun;
    
    [Header("Deceleration Rates")]
    [SerializeField] private float rateOfDecelerationTowardsIdle;
    [SerializeField] private float rateOfDecelerationTowardsWalk;
    [SerializeField] private float rateOfDecelerationTowardsLightJog;
    [SerializeField] private float rateOfDecelerationTowardsJog;
    [SerializeField] private float rateOfDecelerationTowardsLightRun;
    [SerializeField] private float rateOfDecelerationTowardsRun;

    [Header("Angles Between Input and Velocity")] [SerializeField]
    private float angleOfHighDecelerationBecauseAngleIsLargerThan;// Angle at which higher deceleration kicks in because angle between playerInput and player Velocity is greater than this angle.

    [Header("Jump Heights (meters)")]
    [SerializeField] private float minJumpHeight;
    [SerializeField] private float maxJumpHeightWhenPlayerIsIdle;
    [SerializeField] private float maxJumpHeightWhenPlayerIsRunning;
    [SerializeField] private float maxJumpHeightWhenPlayerIsBoosted;

    [Header("Jump Tuning")]
    [SerializeField] private float coyoteTime;
    [SerializeField] private float jumpBuffer;
    [SerializeField] private float gravity;
    
    // General Movement and Direction
    private Vector2 playerVelocity;
    private Vector2 playerAcceleration;
    private Vector2 inputDirectionOfThePlayer;
    
    // 
    
    // Jump State
    private bool isJumpInputBeingHeld;
    private float lastGroundedTime;
    private float lastJumpPressedTime;
    private float lastMinimumJumpHeightTimeStamp;
    private float lastMaximumJumpHeightTimeStamp;
    private float jumpStartTime;

    // Boost State
    private bool boostPending;
    private Vector2 pendingBoostVelocity;
    private bool skipDecelThisFrame; 
    
    
    // Accelerate Movement 
    private void OnEnable()
    {
        inputMoving.action.Enable();
        inputBoosting.action.Enable();
    }

    private void OnDisable()
    {
        inputMoving.action.Disable();
        inputBoosting.action.Disable();
    }

    private void Awake()
    {
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerVelocity = Vector2.zero;
        inputDirectionOfThePlayer = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
        // Get the Input Direction
        Vector2 inputDirection = inputMoving.action.ReadValue<Vector2>();
        
        // Check if Player wants to move
        if (inputDirection.magnitude > 0f)
        {
            // Check velocity direction and null velocity check
            if (playerVelocity.magnitude == 0f)
            {
                // Player is not moving
            }
            else
            {
                
                // Player is moving
                // Movement + accelerated movement
                float angle = Vector2.Angle(playerVelocity, inputDirection);
                
                // Somehow mix in the angle between input and direction to quicker slow down or have better change of direction
                
                playerAcceleration = playerVelocity * (rateOfAccelerationTowardsRun * 0.5f * Time.deltaTime);
                playerVelocity += playerAcceleration;
                // Call move script
                playerVelocity += playerAcceleration;
            }
        }
        else
        {
            // Player does not want to move -> Natural deceleration
        }
        // Get the current velocity of the player
        // Figure out in what state the player is in (walk, light jog)
        // Figure out the difference between the current velocity and the current input direction and check if there is a strong change in direction
        // No sliding to a side? At a specific speed maybe change in direction is like drifting?
        // If it is perpendicular then player wants to slow down

    }
}
