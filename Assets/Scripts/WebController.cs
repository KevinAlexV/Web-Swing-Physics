using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebController : MonoBehaviour
{
    [SerializeField]
    private BasicPendulumBehavior pendulum;
    [SerializeField]
    private List<Transform> pivots = new List<Transform>();
    [SerializeField]
    private int pivotIndex = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            if (pivotIndex == (pivots.Count - 1)) 
                pivotIndex = 0;
            else 
                pivotIndex++;

            pendulum.pivot = pivots[pivotIndex];

            StartCoroutine(WebSwing());
        }        
    }

    IEnumerator WebSwing()
    {
        pendulum.isOnPivot = false;

        yield return new WaitForSeconds(1.0f);    

        pendulum.isOnPivot = true;

        pendulum.webLength = Vector3.Distance(gameObject.transform.position, pendulum.pivot.position); 
    }
}
