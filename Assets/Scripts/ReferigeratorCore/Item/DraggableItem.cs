using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 offset;
    private Camera mainCamera;
    [SerializeField] private Vector3 initialPosition;
    [SerializeField] private LayerMask snapSurfaceLayerMask;
    private Collider objectCollider;
    public delegate void ItemRemovedHandler();
    public static event ItemRemovedHandler OnItemRemoved;
    public static event ItemRemovedHandler OnItemOrganized;
    void Start()
    {
        mainCamera = Camera.main;
        objectCollider = GetComponent<Collider>();
        // Store the initial position
        initialPosition = transform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("drag start : " );
        // Calculate offset between object position and mouse position
        offset = transform.position - GetMouseWorldPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Update object position based on mouse position
        transform.position = GetMouseWorldPosition(eventData) + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Perform a raycast downward to detect the surface
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, snapSurfaceLayerMask))
        {
            // Get the offset based on the object's collider bounds
            float objectHeight = objectCollider.bounds.extents.y;
            // Snap the item's position to the hit point, adjusted to be on top of the surface
            transform.position = hit.point + Vector3.up * objectHeight;
            if (hit.collider.CompareTag("itemcounter"))
            {
                // Trigger the event
                OnItemRemoved?.Invoke();
            }
            if (hit.collider.CompareTag("section"))
            {
                // Trigger the event
                OnItemOrganized?.Invoke();
            }
        }
        else
        {
            // If no surface is detected, return to the initial position
            transform.position = initialPosition;
        }
    }

    private Vector3 GetMouseWorldPosition(PointerEventData eventData)
    {
        Vector3 screenPosition = new Vector3(eventData.position.x, eventData.position.y, mainCamera.WorldToScreenPoint(transform.position).z);
        return mainCamera.ScreenToWorldPoint(screenPosition);
    }
}
