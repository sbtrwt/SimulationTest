using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlatformerSpeedBooster : MonoBehaviour
{
    [Header("Boost Settings")]
    public float speedMultiplier = 1.5f;
    public float duration = 2f;
    public bool affectJump = true;
    public float jumpMultiplier = 1.2f;

    [Header("Effects")]
    public ParticleSystem collectEffect;
    public AudioClip collectSound;
    public UnityEvent onCollected;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyBoost(other.gameObject);
            PlayEffects();
            onCollected.Invoke();
            Destroy(gameObject);
        }
    }

    private void ApplyBoost(GameObject player)
    {
        // Try different movement controllers
        // if (player.TryGetComponent<PlatformerMovement>(out PlatformerMovement movement))
        // {
        //     movement.ApplySpeedBoost(speedMultiplier, duration, affectJump ? jumpMultiplier : 1f);
        // }
        // else 
        if (player.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            StartCoroutine(PhysicsBoost(rb));
        }
    }

    private IEnumerator PhysicsBoost(Rigidbody rb)
    {
        float originalSpeed = rb.velocity.magnitude;
        rb.velocity *= speedMultiplier;
        
        yield return new WaitForSeconds(duration);
        
        rb.velocity = rb.velocity.normalized * originalSpeed;
    }

    private void PlayEffects()
    {
        if (collectEffect != null)
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        
        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
    }
}