using System;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class Grappling : MonoBehaviour
{
    [SerializeField] private float radiusOfAreaWhichGrappableIsDetected;
    [SerializeField] private float distanceOfDetectableGrappable;
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
    void Start()
    {
        //cameraPlayer = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // Shoot Ray that is forward to camera
        if (Physics.SphereCast(cameraPlayer.transform.position, radiusOfAreaWhichGrappableIsDetected, cameraPlayer.transform.forward * distanceOfDetectableGrappable, out var hit,
                maxDistance, layerToGrapple, QueryTriggerInteraction.Ignore))
        {
            Debug.Log("Detected Sphere");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(cameraPlayer.transform.position + cameraPlayer.transform.forward * distanceOfDetectableGrappable,radiusOfAreaWhichGrappableIsDetected);
        
    }
}
