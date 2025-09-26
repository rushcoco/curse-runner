using UnityEngine;

public class LavaIsRising : MonoBehaviour
{
    [SerializeField] private float rateLavaIsRisingInMetersPerSecond;

    private bool gameHasEnded;
    private Vector3 worldPositionWhereLavaStartedRising;
    private Vector3 worldPositionWhereLavaStoppedRising;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameHasEnded = false;
        worldPositionWhereLavaStartedRising = transform.position;
    }

    // Update is called once per frame
    void Update()
    { 
        if (!gameHasEnded)
            transform.position += Vector3.up * (rateLavaIsRisingInMetersPerSecond * Time.deltaTime);
    }

    public void StopLavaFromRising()
    {
        gameHasEnded = true;
        worldPositionWhereLavaStoppedRising = transform.position;
        Debug.Log(Vector3.Distance(worldPositionWhereLavaStartedRising,worldPositionWhereLavaStoppedRising));
    }
}
