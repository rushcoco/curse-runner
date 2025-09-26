using System;
using Unity.VisualScripting;
using UnityEngine;

public class AltarBehaviour : MonoBehaviour
{
    [SerializeField] private LayerMask layerOfPlayer;

    [SerializeField] private LavaIsRising monoBehaviourLavaIsRising;

    [SerializeField] private PlayerMovement monoBehaviourPlayerMovement;

    [SerializeField] private Grappling monoBehaviourGrappling;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == layerOfPlayer)
        {
            // Player has won
            // Lava stop rising
            
        }
    }
}
