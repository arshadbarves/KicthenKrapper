using UnityEngine;

public class DroneCamera : MonoBehaviour
{
    public float rotationSpeed = 20.0f;
    public Vector3 rotationAxis = Vector3.up;
    public Vector3 centerPoint = Vector3.zero;
    public float radius = 5.0f;

    private float currentAngle = 0.0f;

    void Update()
    {
        // Calculate the position of the camera using polar coordinates
        float x = centerPoint.x + radius * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
        float y = centerPoint.y;
        float z = centerPoint.z + radius * Mathf.Sin(currentAngle * Mathf.Deg2Rad);

        transform.position = new Vector3(x, y, z);

        // Rotate the camera around the center point
        transform.RotateAround(centerPoint, rotationAxis, rotationSpeed * Time.deltaTime);

        // Increment the current angle
        currentAngle += rotationSpeed * Time.deltaTime;

        // Keep the angle between 0 and 360 degrees
        if (currentAngle >= 360.0f)
        {
            currentAngle = 0.0f;
        }
    }
}
