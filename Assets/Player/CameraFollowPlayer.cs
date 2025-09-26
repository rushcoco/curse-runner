using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private InputActionReference cameraMovementInput;
    private Transform playerBody;

    [SerializeField] private float mouseSensitivity;
    [SerializeField] private bool invertedHorizontalCamera;
    [SerializeField] private bool invertedVerticalCamera;
    [SerializeField] private float maxDegreesPlayerCanRotateTheCameraOnLocalXAxis;

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
        playerBody.Rotate(Vector3.up * directionInput.x * Mathf.);

        xAxis -= directionInput.y; // Moving camera up and down -> in room is rotation on x-axis
        xAxis = Mathf.Clamp(xAxis, -maxDegreesPlayerCanRotateTheCameraOnLocalXAxis, maxDegreesPlayerCanRotateTheCameraOnLocalXAxis);

        transform.localRotation = Quaternion.Euler(Vector3.right * xAxis);

    }
    
}
