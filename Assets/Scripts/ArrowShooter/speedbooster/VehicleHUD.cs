using UnityEngine;
using UnityEngine.UI;
using System;

public class VehicleHUD : MonoBehaviour
{
    [Header("References")]
    public Rigidbody vehicleRigidbody;
    public Text speedText;
    public Text distanceText;
    public Text timeText;
    public Image speedometerNeedle;
    
    [Header("Settings")]
    public float speedMultiplier = 3.6f; // Convert m/s to km/h
    public string speedUnit = " km/h";
    public string distanceUnit = " m";
    
    [Header("Needle Settings")]
    public float minNeedleAngle = 0f;
    public float maxNeedleAngle = -270f;
    public float maxSpeed = 200f;

    // Private variables
    private float totalDistance;
    private Vector3 lastPosition;
    private float elapsedTime;
    private bool isRunning;

    private void Start()
    {
        if (vehicleRigidbody == null)
            vehicleRigidbody = GetComponent<Rigidbody>();
            
        ResetMetrics();
    }

    public void ResetMetrics()
    {
        totalDistance = 0f;
        elapsedTime = 0f;
        lastPosition = vehicleRigidbody.position;
        isRunning = true;
    }

    private void Update()
    {
        if (!isRunning) return;

        // Calculate speed
        float speed = vehicleRigidbody.velocity.magnitude * speedMultiplier;
        
        // Update speed display
        if (speedText != null)
            speedText.text = Mathf.RoundToInt(speed) + speedUnit;
        
        // Update needle rotation
        if (speedometerNeedle != null)
        {
            float speedNormalized = Mathf.Clamp01(speed / maxSpeed);
            float needleAngle = Mathf.Lerp(minNeedleAngle, maxNeedleAngle, speedNormalized);
            speedometerNeedle.rectTransform.localEulerAngles = new Vector3(0, 0, needleAngle);
        }
        
        // Calculate distance
        float frameDistance = Vector3.Distance(vehicleRigidbody.position, lastPosition);
        totalDistance += frameDistance;
        lastPosition = vehicleRigidbody.position;
        
        // Update distance display
        if (distanceText != null)
            distanceText.text = string.Format("{0:0.00}",totalDistance/1000) + distanceUnit;
        
        // Update time
        elapsedTime += Time.deltaTime;
        
        // Update time display
        if (timeText != null)
            timeText.text = FormatTime(elapsedTime);
    }

    private string FormatTime(float timeInSeconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(timeInSeconds);
        
        // Format as MM:SS.mm
        return string.Format("{0:00}:{1:00}.{2:00}", 
            timeSpan.Minutes, 
            timeSpan.Seconds, 
            timeSpan.Milliseconds / 10);
    }
    

    public void PauseTracking() => isRunning = false;
    public void ResumeTracking() => isRunning = true;
}