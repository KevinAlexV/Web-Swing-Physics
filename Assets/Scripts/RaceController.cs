using UnityEngine;

public class RaceController : MonoBehaviour
{
    [Header("Physics Objects")]
    [SerializeField]
    private Transform endpoint;
    [SerializeField]
    private GameObject spiderMan;
    [SerializeField]
    private GameObject runningMan;

    [Header("Physics Behaviors")]
    [SerializeField]
    private BasicPendulumBehavior basicPendulumBehavior;
    [SerializeField]
    private RunningBehavior runningBehavior;

    // Freeze transforms as they pass/approach the endpoint
    private void Update()
    {
        // Check if running man has passed the endpoint
        if (runningMan.transform.position.x > endpoint.position.x) 
            runningBehavior.enabled = false;
        else 
            runningBehavior.enabled = true;
        
        // Check if spider-man has passed the endpoint
        if (spiderMan.transform.position.x > endpoint.position.x) 
            basicPendulumBehavior.enabled = false;
        else
            basicPendulumBehavior.enabled = true;
    }
}
