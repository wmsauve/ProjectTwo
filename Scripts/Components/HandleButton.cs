using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandleButton : MonoBehaviour, IDragHandler, IPointerDownHandler//IPointerUpHandler
{
    private GameManager GM;
    private int tempID;
    public int ReturnTempID()
    {
        return tempID;
    }

    private void Start()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        gameObject.transform.position = Input.mousePosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GM.DraggingButton = true;
        GM.DraggedButton = gameObject;
        //fetch the buttons ID from class
        foreach(UIObject button in GM.UIButtons)
        {
            if(gameObject.name == button.GetUIName())
            {
                tempID = button.GetButtonID();
            }
        }
    }

    //public void OnPointerUp(PointerEventData eventData)
    //{


    //}
}
