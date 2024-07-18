using UnityEngine;
using UnityEngine.EventSystems;

public class Drag3D : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 offset;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
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
        // Optional: Add any logic you want to execute when dragging ends
    }

    private Vector3 GetMouseWorldPosition(PointerEventData eventData)
    {
        Vector3 screenPosition = new Vector3(eventData.position.x, eventData.position.y, mainCamera.WorldToScreenPoint(transform.position).z);
        return mainCamera.ScreenToWorldPoint(screenPosition);
    }
}