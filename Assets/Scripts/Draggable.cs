using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {

	[SerializeField] private CanvasGroup canvasGroup;
	[SerializeField] private CircleCollider2D circleCollider;
	[SerializeField] private Bomb bomb;

	public void OnPointerDown(PointerEventData eventData) {
		canvasGroup.blocksRaycasts = false;
		bomb.beingDragged = true;
		circleCollider.enabled = false;

		Vector3 screenPoint = Input.mousePosition;
    	screenPoint.z = 1f; 
    	transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
	}

	public void OnDrag(PointerEventData eventData) {
		Vector3 screenPoint = Input.mousePosition;
    	screenPoint.z = 1f; 
    	transform.position = Camera.main.ScreenToWorldPoint(screenPoint);

    	transform.position = new Vector3 (Mathf.Clamp(transform.position.x, -6.5f, 6.5f),
    									  Mathf.Clamp(transform.position.y, -4.5f, 4.5f),
    									  transform.position.z);

	}

	public void OnPointerUp(PointerEventData eventData) {
		canvasGroup.blocksRaycasts = true;
		bomb.beingDragged = false;
		circleCollider.enabled = true;
	}
}
