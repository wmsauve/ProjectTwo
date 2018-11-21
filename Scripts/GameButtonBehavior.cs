using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    /// <summary>
    /// Drop button 
    /// </summary>

    private void DroppingButton()
    {
        if(Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //I am successfully dropping the button on a ball
            if (!Physics.Raycast(ray, out hit) && hit.transform == null && DraggingButton)
            {
                DraggedButton.transform.localPosition = UIButtons[DraggedButton.GetComponent<HandleButton>().ReturnTempID()].GetButtonInitPosition();
            }
            else if (Physics.Raycast(ray, out hit) && DraggingButton && DraggedButton != null )
            {
                //Debug.Log("Hello World");
                //Apply Button effect
                if(hit.transform.tag == "Ball")
                {
                    ButtonEffect(UIButtons[DraggedButton.GetComponent<HandleButton>().ReturnTempID()], hit.transform.gameObject.GetComponent<BallCollision>().GetMyID());
                    BallList[hit.transform.GetComponent<BallCollision>().GetMyID()].MouseOverGlow(NotGlow);
                }
                else if(hit.transform.tag == "Floor")
                {
                    //child of floor must be ball with component BallCollision
                    ButtonEffect(UIButtons[DraggedButton.GetComponent<HandleButton>().ReturnTempID()],
                        hit.transform.GetComponentInChildren<BallCollision>().GetMyID());
                    BallList[hit.transform.GetComponentInChildren<BallCollision>().GetMyID()].MouseOverGlow(NotGlow);
                }
                //get rid of the game object
                UseButton();
            }
            DraggingButton = false;
          
        }

    }

    /// <summary>
    /// Handles the button being removed from the UI
    /// </summary>
    private void UseButton()
    {
        UIButtons.Remove(UIButtons[DraggedButton.GetComponent<HandleButton>().ReturnTempID()]);
        Destroy(DraggedButton);
        //Debug.Log(DraggedButton);
        //readjust the initial position and ID for the remaining buttons
        for(int i = 0; i < UIButtons.Count; i++)
        {
            //Debug.Log(UIButtons[i].GetUIBody());
            if (UIButtons[i].GetUIBody() != DraggedButton && UIButtons[i].GetUIBody() != null)
            {
                
                StartCoroutine(SetInitPositionForButtons(UIButtons[i]));
                UIButtons[i].SetButtonID(i);
            }
            
        }
        
    }


    /// <summary>
    /// Handles applying the button effect.
    /// WORK ON THIS AFTER SETTING BALL/FLOOR PAIR ID.
    /// </summary>
    private void ButtonEffect(UIObject button, int whichBall)
    {
        switch(button.GetButtonType())
        {
            case ButtonTypes.Bounce_Add_Type:
                BallList[whichBall].ButtonAddition(button.GetBounceAdd());
                FloatingText(button.GetBounceAdd(), BallList[whichBall].GetMyFloor().transform.position, AddOrSubtract.Add);
                break;
            case ButtonTypes.AntiBounce_Add_Type:
                BallList[whichBall].ChangeMyAntiBounce(button.GetBounceAdd());
                FloatingText(button.GetBounceAdd(), BallList[whichBall].GetMyFloor().transform.position, AddOrSubtract.Buff);
                break;
        }
    }
}
