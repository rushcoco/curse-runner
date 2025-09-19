using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private InputActionReference cameraMovementInput;
    private Transform playerBody;

    [SerializeField] private float mouseSensitivity;
    [SerializeField] private bool invertedHorizontalCamera;
    [SerializeField] private bool invertedVerticalCamera;

    private float xAxis;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        cameraMovementInput.action.Enable();
    }

    private void OnDisable()
    {
        cameraMovementInput.action.Disable();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Locked at the center. Not arbitrary
        playerBody = transform.parent;
        xAxis = 0f;
    }

    private void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 directionInput = cameraMovementInput.action.ReadValue<Vector2>() * (mouseSensitivity * Time.deltaTime);
        playerBody.Rotate(Vector3.up * directionInput.x);

        xAxis -= directionInput.y; // Moving camera up and down -> in room is rotation on x axis
        xAxis = Mathf.Clamp(xAxis, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xAxis, 0f, 0f);

    }
    
}
