using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputActionReference directionInput;
    [SerializeField] private InputActionReference runningInput;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
        
    }

    private void OnDownRunningInput(InputAction.CallbackContext context)
    {
        
    }
}
