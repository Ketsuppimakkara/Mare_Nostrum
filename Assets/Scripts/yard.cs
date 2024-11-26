using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yard : MonoBehaviour
{
    public GameObject sail;
    public float maxRotationAngle; 
    public float rotationSpeed;
    private float currentRotation; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        // Get current rotation angle from the GameObject's local rotation
        currentRotation = transform.localEulerAngles.y;
        if (currentRotation > 180)
        {
            currentRotation -= 360; // Convert to range -180 to 180
        }
    }

    public void rotateLeft()
    {
        // Calculate rotation amount
        float rotationAmount = -rotationSpeed * Time.deltaTime;

        // Calculate new rotation angle
        float newRotation = currentRotation + rotationAmount;

        // Clamp new rotation angle within bounds
        if (newRotation < -maxRotationAngle)
        {
            newRotation = -maxRotationAngle;
            rotationAmount = -maxRotationAngle - currentRotation;
        }

        // Apply rotation
        transform.Rotate(Vector3.up, rotationAmount);

        // Update current rotation
        currentRotation = newRotation;
    }

    public void rotateRight()
    {
        // Calculate rotation amount
        float rotationAmount = rotationSpeed * Time.deltaTime;

        // Calculate new rotation angle
        float newRotation = currentRotation + rotationAmount;

        // Clamp new rotation angle within bounds
        if (newRotation > maxRotationAngle)
        {
            newRotation = maxRotationAngle;
            rotationAmount = maxRotationAngle - currentRotation;
        }

        // Apply rotation
        transform.Rotate(Vector3.up, rotationAmount);

        // Update current rotation
        currentRotation = newRotation;
    }
}
