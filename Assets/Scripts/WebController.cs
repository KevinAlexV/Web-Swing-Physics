using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WebController : MonoBehaviour
{
    [SerializeField]
    private BasicPendulumBehavior pendulum;
    [SerializeField]
    private List<GameObject> pivots = new List<GameObject>();
    [SerializeField]
    private int pivotIndex = 0;

    private List<LineRenderer> webRenderers = new List<LineRenderer>();

    private void Start() => SetupWebs(); 

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) 
        {
            // Destroy previous web
            DestroyWeb(pivotIndex);

            if (pivotIndex == (pivots.Count - 1)) 
                pivotIndex = 0;
            else 
                pivotIndex++;

            pendulum.pivot = pivots[pivotIndex].transform;

            StartCoroutine(WebSwing());
        }        

        RenderWeb();
    }

    IEnumerator WebSwing()
    {
        // Detatch from pivot
        pendulum.isOnPivot = false;

        // Wait one second, Spider-Man 'falls' due to force of gravity 
        yield return new WaitForSeconds(1.0f);    

        // Reattach to next pivot and create web
        pendulum.isOnPivot = true;
        pendulum.webLength = Vector3.Distance(gameObject.transform.position, pendulum.pivot.position); 
    }

    private void SetupWebs()
    {
        LineRenderer web = null;

        // Generate a line renderer for each pivot to display 'webs'
        foreach (GameObject pivot in pivots)
        {
            web = pivot.AddComponent<LineRenderer>();
            web.material = new Material(Shader.Find("Sprites/Default"));
            web.startColor = Color.white;
            web.endColor = Color.white;
            web.startWidth = 0.1f;
            web.endWidth = 0.1f;
            web.useWorldSpace = false;
            webRenderers.Add(web);
        }
    }

    private void RenderWeb()
    {
        if (pendulum.isOnPivot) 
        {
            webRenderers[pivotIndex].enabled = true;
            webRenderers[pivotIndex].positionCount = 2;
            
            // Render the position on the player as they swing about the pivot
            webRenderers[pivotIndex].SetPosition(0, gameObject.transform.position - pendulum.pivot.transform.position);
            webRenderers[pivotIndex].SetPosition(1, Vector3.zero);
        }
    }

    private void DestroyWeb(int index) => webRenderers[index].enabled = false;

    public void ResetPivot() 
    { 
        // Disable all webs in the scene
        webRenderers.ForEach((x) => {
            x.enabled = false;
        });

        pivotIndex = 0;
        pendulum.pivot = pivots[pivotIndex].transform;

        // Recalculate the web length
        pendulum.webLength = Vector3.Distance(gameObject.transform.position, pendulum.pivot.position); 
    }
}
