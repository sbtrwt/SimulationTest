using System.Collections;
using UnityEngine;

public class SpeedBooster : MonoBehaviour
{
    [Header("Boost Settings")]
    public float boostMultiplier = 2f; // How much to multiply the speed
    public float boostDuration = 3f; // How long the boost lasts
    public ParticleSystem boostEffect; // Optional visual effect

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object has a specific tag or component
        if (other.CompareTag("Player")) // Or check for other.GetComponent<VehicleController>()
        {
            // Apply boost to the object
            ApplyBoost(other.gameObject);
            
            // Play visual effect if assigned
            if (boostEffect != null)
            {
                boostEffect.Play();
            }
            
            // Optional: Disable the booster temporarily
            GetComponent<Collider>().enabled = false;
            GetComponent<Renderer>().enabled = false;
            Invoke("ResetBooster", 5f); // Reset after 5 seconds
        }
    }

    private void ApplyBoost(GameObject target)
    {
        // Option 1: If using Rigidbody physics
        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb != null)
        {
            StartCoroutine(BoostRigidbody(rb));
            return;
        }
        
        // // Option 2: If using a custom movement script
        // VehicleController vehicle = target.GetComponent<VehicleController>();
        // if (vehicle != null)
        // {
        //     StartCoroutine(BoostVehicle(vehicle));
        // }
    }

    private IEnumerator BoostRigidbody(Rigidbody rb)
    {
        float originalDrag = rb.drag;
        rb.drag = originalDrag / boostMultiplier; // Reduce drag to increase speed
        
        yield return new WaitForSeconds(boostDuration);
        
        rb.drag = originalDrag; // Reset to original drag
    }

    // private IEnumerator BoostVehicle(VehicleController vehicle)
    // {
    //     float originalSpeed = vehicle.maxSpeed;
    //     vehicle.maxSpeed *= boostMultiplier;
        
    //     yield return new WaitForSeconds(boostDuration);
        
    //     vehicle.maxSpeed = originalSpeed;
    // }

    private void ResetBooster()
    {
        GetComponent<Collider>().enabled = true;
        GetComponent<Renderer>().enabled = true;
    }
}