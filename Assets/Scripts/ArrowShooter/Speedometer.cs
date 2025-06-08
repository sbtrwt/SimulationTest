using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{
    public Rigidbody targetRigidbody; // Assign the object you want to measure
    public Text speedText; // Assign your UI Text element
    public string unit = " km/h"; // Unit to display
    public float multiplier = 3.6f; // Convert m/s to km/h (use 2.237 for mph)

    private void Update()
    {
        if (targetRigidbody != null && speedText != null)
        {
            // Calculate speed and update the UI
            float speed = targetRigidbody.velocity.magnitude * multiplier;
            speedText.text = Mathf.RoundToInt(speed) + unit;
        }
    }
}