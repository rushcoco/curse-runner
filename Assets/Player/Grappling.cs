using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController))]
public class Grappling : MonoBehaviour
{
    private static readonly int Grapple = Animator.StringToHash("GRAPPLE");
    [SerializeField] private InputActionReference inputGrapple;
    [SerializeField] private float radiusOfAreaWhichGrappableIsDetected;
    [SerializeField] private float lengthOfDetectableGrappable;
    [SerializeField] private float distanceWhereDetectionForGrappableStarts;
    [SerializeField] private float distanceWhereDetectionForGrappableEnds;
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask layerToGrapple;
    // Needs to recognize which objects should be hit and which shouldn't be hit
    // - layers?
    // - get component "Blood Prefab" check?
    // - cursor position to screen?
    [SerializeField] private Camera cameraPlayer;
    [SerializeField] private float rangeBetweenPlayerAndGrappableToStopGrappling;
    [SerializeField] private float speedAtWhichPlayerIsReachingToTheOrb;
    [SerializeField] private float secondsAfterReachingOrbWhereJumpEffectHoldsOn;
    [SerializeField] private float minimumJumpHeightAfterOrb;
    [SerializeField] private float maximumJumpHeightAfterOrb;
    [SerializeField] private float adjustingJumpLerpByMultiplyingSeconds;
    [SerializeField] private float adjustingJumpHeightByMultiplyingFinalValue;
    [SerializeField] private GameObject armGrapple;
    
    // Needs a thing to hit the other Objects and do check
    // - Raycast?
    // - Physics
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    private Vector3 mousePos;
    private Vector3 mouseInWorldPos;

    private Vector3 point1;
    private Vector3 point2;
    private Vector3 grappledObjectPosition;
    private CharacterController ccSelf;
    private PlayerMovement playerMovement;
    private bool isGrappling;
    private Animator grapplingAnimator;

    private void OnEnable()
    {
        inputGrapple.action.Enable();
        inputGrapple.action.performed += OnPerformedGrapple;
    }

    private void OnDisable()
    {
        inputGrapple.action.performed -= OnPerformedGrapple;
        inputGrapple.action.Disable();
    }

    private void Awake()
    {
        ccSelf = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();
        Debug.Log(armGrapple.scene);
        grapplingAnimator = armGrapple.GetComponent<Animator>();
    }

    void Start()
    {
        //cameraPlayer = Camera.main;
        point1 = Vector3.zero;
        point2 = Vector3.zero;
        isGrappling = false;

    }

    private void Update()
    {
        mousePos = Mouse.current.position.ReadValue();
        mousePos.z = cameraPlayer.nearClipPlane;
        mouseInWorldPos = cameraPlayer.ScreenToWorldPoint(mousePos);
        
        
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        /*// Shoot Ray that is forward to camera
        if (Physics.SphereCast(cameraPlayer.transform.position, radiusOfAreaWhichGrappableIsDetected, cameraPlayer.transform.forward * lengthOfDetectableGrappable, out var hit,
                maxDistance, layerToGrapple, QueryTriggerInteraction.Ignore))
        {
            Debug.Log("Detected Sphere");
        }

        RaycastHit[] results = new RaycastHit[] { };
        point1 = cameraPlayer.transform.position;
        point2 = point1 + cameraPlayer.transform.forward * lengthOfDetectableGrappable;
        Physics.CapsuleCastNonAlloc(point1, point2, radiusOfAreaWhichGrappableIsDetected,
            cameraPlayer.transform.forward, results, maxDistance, layerToGrapple);
        foreach (var raycastHit in results)
        {
            Debug.Log("raycastHits: " + raycastHit.transform.gameObject.name);
        }*/
            //Debug.Log("Capsule Cast found Sphere: \npoint1: " + point1 + "\npoint2: " + point2);
            
        
        
        
        
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(new Ray(cameraPlayer.transform.position,cameraPlayer.transform.forward * maxDistance));
    }

    private void OnPerformedGrapple(InputAction.CallbackContext context)
    {
        if (isGrappling)
            return;
        
        Ray ray = new Ray(cameraPlayer.transform.position, cameraPlayer.transform.forward);
        /*RaycastHit[] hits = new RaycastHit[] {};
        Physics.RaycastNonAlloc(ray, hits, maxDistance, layerToGrapple, QueryTriggerInteraction.Ignore);

        foreach (var raycastHit in hits)
        {
            if (raycastHit.transform.gameObject.layer == LayerMask.NameToLayer(layerToGrapple.ToString()))
            {
                Debug.Log("Rair Rair");
            }
        }*/


        if (Physics.Raycast(ray, out var hit, maxDistance, layerToGrapple, QueryTriggerInteraction.Collide))
        {
            Vector3 kdsafj = hit.transform.position - transform.position;
            if (Physics.Raycast(ray, out RaycastHit hitInfo, kdsafj.magnitude))
            {
                LayerMask hitLayer = hitInfo.transform.gameObject.layer;
                if (hitLayer == LayerMask.NameToLayer("Ground") || hitLayer == LayerMask.NameToLayer("Default"))
                    return;
            }
            //Debug.Log(Physics.queriesHitTriggers);
            isGrappling = true;
            //
            //
            //Debug.Log("Grapple Hitted: " + hit.transform.gameObject.name);
            grapplingAnimator.SetBool(Grapple, true);
            Debug.Log(grapplingAnimator.GetBool(Grapple));
            StartCoroutine(MoveTowardsGrappledObject(hit));
        }
        // if (Physics.Raycast(cameraPlayer.transform.position, out var hit, cameraPlayer.transform.forward, maxDistance, layerToGrapple,
        //      QueryTriggerInteraction.Ignore))
        // {
        //     // Move Player towards orb
        //     grappledObjectPosition = hit
        //     Debug.Log("Found Orb");
        // }
        // Debug.Log("Grapple Performed");
    }

    private IEnumerator MoveTowardsGrappledObject(RaycastHit hit)
    {
        playerMovement.FreezeVelocity();
        var magnitudeOfPlayerBeforeReachingOrb = Mathf.Clamp(playerMovement.GetVelocityMagnitude(),minimumJumpHeightAfterOrb,maximumJumpHeightAfterOrb);
        var magnitudeOfPlayerBeforeReachingOrb2D = playerMovement.GetVelocity2DMagnitude();
        var directionInitial = hit.transform.position - transform.position;
        
        while (Vector3.Distance(transform.position, hit.transform.position) > rangeBetweenPlayerAndGrappableToStopGrappling)
        {
            var directionUpdatedByFrame = hit.transform.position - transform.position;
            playerMovement.externalGrapplingVelocity = directionUpdatedByFrame.normalized * speedAtWhichPlayerIsReachingToTheOrb;
            yield return null;
        }
        playerMovement.UnfreezeVelocity();
        var totalSecondsAfterReachingOrb = secondsAfterReachingOrbWhereJumpEffectHoldsOn;
        grapplingAnimator.SetBool(Grapple, false);
        Debug.Log(grapplingAnimator.GetBool(Grapple));
        while (secondsAfterReachingOrbWhereJumpEffectHoldsOn > 0f)
        {
            var nextPos = Vector3.Lerp(Vector3.zero, directionInitial, secondsAfterReachingOrbWhereJumpEffectHoldsOn / totalSecondsAfterReachingOrb);
            secondsAfterReachingOrbWhereJumpEffectHoldsOn -= Time.deltaTime;

            playerMovement.externalGrapplingVelocity = (nextPos.normalized + directionInitial.normalized) * (magnitudeOfPlayerBeforeReachingOrb + 1) + Vector3.up * magnitudeOfPlayerBeforeReachingOrb2D;
            Debug.Log(magnitudeOfPlayerBeforeReachingOrb);
            //playerMovement.externalGrapplingVelocity = (nextPos + Vector3.up * 
            //Mathf.Clamp(magnitudeOfPlayerBeforeReachingOrb * adjustingJumpHeightByMultiplyingFinalValue,minimumJumpHeightAfterOrb,maximumJumpHeightAfterOrb))
            // * adjustingJumpHeightByMultiplyingFinalValue;
            
            yield return null;
        }
        // playerMovement.UnfreezeVelocity();

        secondsAfterReachingOrbWhereJumpEffectHoldsOn = totalSecondsAfterReachingOrb;
        playerMovement.externalGrapplingVelocity = Vector3.zero;
        isGrappling = false;
    }
}
