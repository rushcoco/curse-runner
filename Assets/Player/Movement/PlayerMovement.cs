using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputActionReference directionInput;
    [SerializeField] private InputActionReference runningInput;
    [SerializeField] private InputActionReference jumpingInput;
    
    [SerializeField] private float walkingMovementSpeed;
    [SerializeField] private float runningMovementSpeed;

    [SerializeField] private float idleJumpingHeight;
    [SerializeField] private float walkingJumpingHeight;
    [SerializeField] private float runningJumpingHeight;

    [SerializeField] private float rangeOfVariatiableJumpingHeight; // If you long press jumping button, this value is being added. if its very short a low percentage will be added. Anything between anything between
    
    
    private CharacterController selfCharacterController;
    private bool isRunningActivated;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Awake()
    {
        isRunningActivated = false;
        selfCharacterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        directionInput.action.Enable();
        runningInput.action.Enable();
        jumpingInput.action.Enable();
        
        runningInput.action.performed += OnDownRunningInput;
    }

    private void OnDisable()
    {
        directionInput.action.Disable();
        runningInput.action.Disable();
        jumpingInput.action.Disable();

        runningInput.action.performed -= OnDownRunningInput;
    }

    // Update is called once per frame
    void Update()
    {
        float movementSpeed = isRunningActivated ? runningMovementSpeed : walkingMovementSpeed;
        
        Vector2 directionVector = directionInput.action.ReadValue<Vector2>();
        Vector3 movementVector = (transform.right * directionVector.x + transform.forward * directionVector.y) * movementSpeed;

        selfCharacterController.Move(movementVector * Time.deltaTime);
    }

    private void OnDownRunningInput(InputAction.CallbackContext context)
    {
        isRunningActivated = !isRunningActivated;
    }
}
