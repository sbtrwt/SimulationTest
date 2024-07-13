using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableCloth : MonoBehaviour
{
    private Cloth cloth;
    private Camera mainCamera;
    private Vector3 offset;
    private bool isDragging = false;

    void Start()
    {
        cloth = GetComponent<Cloth>();
        mainCamera = Camera.main;

        // Adjust these parameters for desired cloth behavior
        cloth.stretchingStiffness = 0.5f;
        cloth.bendingStiffness = 0.5f;
        cloth.useGravity = true;
        cloth.damping = 0.5f;
        cloth.friction = 0.5f;
    }
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    RaycastHit hit;
                    Ray ray = mainCamera.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
                    {
                        isDragging = true;
                        offset = transform.position - hit.point;
                    }
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        Vector3 newPosition = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, mainCamera.WorldToScreenPoint(transform.position).z));
                        transform.position = newPosition + offset;
                    }
                    break;

                case TouchPhase.Ended:
                    isDragging = false;
                    break;
            }
        }
    }
    void LateUpdate()
    {
        if (!isDragging)
        {
            // Reduce simulation quality when not interacting
            cloth.enabled = false;
        }
        else
        {
            cloth.enabled = true;
        }
    }
}
