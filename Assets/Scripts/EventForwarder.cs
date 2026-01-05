using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

public class EventForwarder : MonoBehaviour, IPointerMoveHandler, IPointerExitHandler, IPointerDownHandler
{
    public Camera worldCamera;
    private GameObject currentHoveredObject;
    private RectTransform rectTransform;

    void Awake() => rectTransform = GetComponent<RectTransform>();

    public void OnPointerMove(PointerEventData eventData)
    {
        GameObject hitObject = PerformWorldRaycast(eventData);
        if (hitObject != currentHoveredObject)
        {
            if (currentHoveredObject != null)
            {
                ExecuteEvents.Execute(currentHoveredObject, eventData, ExecuteEvents.pointerExitHandler);
            }

            if (hitObject != null)
            {
                ExecuteEvents.Execute(hitObject, eventData, ExecuteEvents.pointerEnterHandler);
            }

            currentHoveredObject = hitObject;
        }

        if (currentHoveredObject != null)
        {
            ExecuteEvents.Execute(currentHoveredObject, eventData, ExecuteEvents.pointerMoveHandler);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentHoveredObject != null)
        {
            ExecuteEvents.Execute(currentHoveredObject, eventData, ExecuteEvents.pointerExitHandler);
            currentHoveredObject = null;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentHoveredObject != null)
        {
            ExecuteEvents.Execute(currentHoveredObject, eventData, ExecuteEvents.pointerDownHandler);
        }
    }

    private GameObject PerformWorldRaycast(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, 
            eventData.position, 
            eventData.pressEventCamera, 
            out Vector2 localPoint
            );
        
        Vector2 normalizedPoint = new Vector2(
            (localPoint.x / rectTransform.rect.width) + 0.5f,
            (localPoint.y / rectTransform.rect.height) + 0.5f
        );

        Ray ray = worldCamera.ViewportPointToRay(normalizedPoint);
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.collider.gameObject;
        }
        return null;
    }
}