using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class UIManager : MonoBehaviour
{
    [Header("Physics Objects")]
    [SerializeField]
    [Tooltip("Used to reset physics objects to their default positions")]
    private BasicPendulumBehavior physicsEngine;
    [SerializeField]
    [Tooltip("Required to reset web swinging pivot points")]
    private WebController webController;

    [Header("Cameras")]
    [SerializeField]
    private MouseOrbitImproved povCamera;
    [SerializeField]
    private CinemachineVirtualCamera lookAtCamera;

    [Header("Animations")]
    [SerializeField]
    private Animator dataPanelAnimator;

    [Header("UI Elements")]
    [SerializeField]
    private Text perspectiveTextField;
    [Space]
    [SerializeField]
    private Text gravityForceText;
    [Space]
    [SerializeField]
    private Text tensionForceText;
    [SerializeField]
    private Text tensionDirX;
    [SerializeField]
    private Text tensionDirY;
    [SerializeField]
    private Text tensionDirZ;
    [Space]
    [SerializeField]
    private Text deltaThetaText;
    [Space]
    [SerializeField]
    private Text velocityX;
    [SerializeField]
    private Text velocityY;
    [SerializeField]
    private Text velocityZ;
    [Space]
    [SerializeField]
    private Text angularMomentumX;
    [SerializeField]
    private Text angularMomentumY;
    [SerializeField]
    private Text angularMomentumZ;

    // Game State Booleans
    private bool isPOV = false;
    private bool isPaused = false;
    private bool isDataPanelOut = false;

    private void Start()
    {
        // POV Camera is disabled by default
        lookAtCamera.enabled = true;
        povCamera.enabled = false;
        isPOV = false;
    }

    private void Update() => UpdateDataPanel(); 

    // Update panel with physics engine data
    private void UpdateDataPanel()
    {
        string floatFormat = "0.00";

        // Set gravity data
        gravityForceText.text = physicsEngine.gravityForce.ToString(floatFormat);

        // Set tension data
        tensionForceText.text = physicsEngine.tensionForce.ToString(floatFormat);
        tensionDirX.text = "x: " + physicsEngine.tensionDirection.x.ToString(floatFormat);
        tensionDirY.text = "y: " + physicsEngine.tensionDirection.y.ToString(floatFormat);
        tensionDirZ.text = "z: " + physicsEngine.tensionDirection.z.ToString(floatFormat);

        // Set theta data
        deltaThetaText.text = physicsEngine.deltaTheta.ToString(floatFormat) + "°";

        // Set velocity data
        velocityX.text = "x: " + physicsEngine.velocityVector.x.ToString(floatFormat);
        velocityY.text = "y: " + physicsEngine.velocityVector.y.ToString(floatFormat);
        velocityZ.text = "z: " + physicsEngine.velocityVector.z.ToString(floatFormat);

        // Set angular momentum data
        angularMomentumX.text = "x: " + physicsEngine.angularMomentum.x.ToString(floatFormat);
        angularMomentumY.text = "y: " + physicsEngine.angularMomentum.y.ToString(floatFormat);
        angularMomentumZ.text = "z: " + physicsEngine.angularMomentum.z.ToString(floatFormat);
    }

    // Resets the simulation to the beginning 
    public void Reset() 
    {
        // Reset the position of physics objects in the scene
        physicsEngine.ResetPosition();

        // Reset the forces applied to physics objects in the scene
        physicsEngine.ResetForces();

        // Reset the swinging pivot points, must be done last to use reset position for webLength
        webController.ResetPivot();
    }

    // Switches camera perspectives between the lookAt and mouse orbit cameras
    public void ChangeCamera()
    {
        if (isPOV) 
        {
            lookAtCamera.enabled = true;
            povCamera.enabled = false;
            perspectiveTextField.text = "POV";
        } 
        else
        {
            lookAtCamera.enabled = false;
            povCamera.enabled = true;
            perspectiveTextField.text = "LookAt";
        }

        isPOV = !isPOV;
    }

    // Pauses the simulation
    public void PauseSimulation()
    {
        if (isPaused) 
            Time.timeScale = 0;
        else
            Time.timeScale = 1;

        isPaused = !isPaused;
    }

    public void ToggleDataPanel()
    {
        if (!isDataPanelOut) 
            dataPanelAnimator.SetBool("isDataPanelOut", true);
        else 
            dataPanelAnimator.SetBool("isDataPanelOut", false);

        isDataPanelOut = !isDataPanelOut;
    }
}
