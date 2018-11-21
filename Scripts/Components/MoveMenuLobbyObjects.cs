using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveMenuLobbyObjects : MonoBehaviour, IPointerDownHandler, IDragHandler
{


    ///This component sits on the menu and lobby items.
    ///It allows the player to drag things around.
    ///

    private Vector3 offsetVector;
    public void OnPointerDown(PointerEventData eventData)
    {
        offsetVector = new Vector3(Input.mousePosition.x - gameObject.transform.position.x, Input.mousePosition.y - gameObject.transform.position.y);
    }

    public void OnDrag(PointerEventData eventData)
    {

        gameObject.transform.position = new Vector3(Mathf.Clamp(Input.mousePosition.x - offsetVector.x, 0.0f, Camera.main.pixelWidth), 
            Mathf.Clamp(Input.mousePosition.y - offsetVector.y, 0.0f, Camera.main.pixelHeight));
    }
}
