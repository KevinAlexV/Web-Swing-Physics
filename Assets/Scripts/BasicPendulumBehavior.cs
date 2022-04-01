using UnityEngine;

public class BasicPendulumBehavior : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField]
    private Transform axis;
    [SerializeField]
    private Transform ball;
    [Header("Mathematics")]
    [SerializeField]
    private float stringLength = 1.0f;
    [SerializeField]
    private float theta = 65.0f;
    [SerializeField]
    private float currentAngle;
    [SerializeField]
    private float speedOfAngleIncrease = 5.0f;
    
    private void Update() 
    {
        currentAngle += speedOfAngleIncrease * Time.deltaTime; 

        if (currentAngle > theta || currentAngle < (theta * -1))
            speedOfAngleIncrease *= -1;

        // Apply rotation quaternion and ensure ball is constricted by the string length
        Vector3 endPosition = axis.position + ((Quaternion.AngleAxis(currentAngle, Vector3.forward) * (Vector3.down * stringLength))); 

        ball.position = Vector3.Lerp(ball.position, endPosition, Time.deltaTime);
    }
}
