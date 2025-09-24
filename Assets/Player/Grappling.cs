using System;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class Grappling : MonoBehaviour
{
    [SerializeField] private float radiusOfAreaWhichGrappableIsDetected;
    [SerializeField] private float lengthOfDetectableGrappable;
    [SerializeField] private float distanceWhereDetectionForGrappableStarts;
    [SerializeField] private float distanceWhereDetectionForGrappableEnds;
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask layerToGrapple;
    // Needs to recognize which objects should be hit and which shouldn't be hit
    // - layers?
    // - get component "Blood Prefab" check?
    [SerializeField] private Camera cameraPlayer;
    // Needs a thing to hit the other Objects and do check
    // - Raycast?
    // - Physics
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    private Vector3 point1;
    private Vector3 point2;
    void Start()
    {
        //cameraPlayer = Camera.main;
        point1 = Vector3.zero;
        point2 = Vector3.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Shoot Ray that is forward to camera
        if (Physics.SphereCast(cameraPlayer.transform.position, radiusOfAreaWhichGrappableIsDetected, cameraPlayer.transform.forward * lengthOfDetectableGrappable, out var hit,
                maxDistance, layerToGrapple, QueryTriggerInteraction.Ignore))
        {
            Debug.Log("Detected Sphere");
        }

        RaycastHit[] results = new RaycastHit[] { };
        point1 = cameraPlayer.transform.position;
        point2 = cameraPlayer.transform.forward * lengthOfDetectableGrappable;
        Physics.CapsuleCastNonAlloc(point1, point2, radiusOfAreaWhichGrappableIsDetected,
            cameraPlayer.transform.forward, results, maxDistance, layerToGrapple,
            QueryTriggerInteraction.Ignore);
        {
            Debug.Log("Capsule Cast found Sphere: \npoint1: " + point1 + "\npoint2: " + point2);

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(point1,radiusOfAreaWhichGrappableIsDetected);
        Gizmos.DrawWireSphere(point2,radiusOfAreaWhichGrappableIsDetected);
    }
}
