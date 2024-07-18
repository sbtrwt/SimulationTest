using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class CleaningItem : MonoBehaviour, IDragHandler
{
    public ParticleSystem actionParticles;
    [SerializeField] private LayerMask cleanLayerMask;
    [SerializeField] private List<Transform> cleaningPoints;
    [SerializeField] private List<Transform> cleanedPoints;
    public delegate void CleanPointHandler();
    public static event CleanPointHandler OnCleanedPoint;
    void Start()
    {
        // Ensure the Particle System starts off
        actionParticles.Stop();
        cleanedPoints = new List<Transform>();
    }
    public void OnDrag(PointerEventData eventData)
    {
        Ray ray = new Ray(transform.position, Vector3.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, cleanLayerMask))
        {

            actionParticles.Play();
            if (hit.collider.CompareTag("cleanpoint"))
            {
                if (!cleanedPoints.Contains(hit.transform))
                {
                    cleanedPoints.Add(hit.transform);
                    OnCleanedPoint?.Invoke();
                }
            }
        }
        else
        {
            actionParticles.Stop();
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.forward * 10);
    }
}
