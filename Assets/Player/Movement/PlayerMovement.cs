using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputActionReference directionInput;
    [SerializeField] private InputActionReference runningInput;
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

        runningInput.action.performed += OnDownRunningInput;
    }

    private void OnDisable()
    {
        directionInput.action.Disable();
        runningInput.action.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 directionVector = directionInput.action.ReadValue<Vector2>();
        Vector3 movementVector = Vector3.right * directionVector.x + Vector3.forward * directionVector.y;

        selfCharacterController.Move(movementVector * Time.deltaTime);
    }

    private void OnDownRunningInput(InputAction.CallbackContext context)
    {
        isRunningActivated = !isRunningActivated;
    }
}
