using System.Collections;
using UnityEngine.UI;
using UnityEngine;


public partial class GameManager : MonoBehaviour
{
    ///
    /// Make the UI.
    ///


    /// <summary>
    /// Method for initializing the UI.
    /// </summary>
    private void UIInitialize()
    {
        ///
        /// Joystick related UI objects - JoyStick
        /// Values are chosen arbitrarily
        UIObject temp = new UIObject(UITypes.Joystick, "LeftJoystick", 200, 200, 300, 300, FixedJoystick);
        inVector = temp.GetUIBody().GetComponent<FixedJoystick>();

        /// Initialize user panel
        /// Values are chosen arbitrarily
        UIObject temp1 = new UIObject(UITypes.Panel, "UserPanel", 0, 805, 0, 520, UserPanel);

        ///Initialize UI Display
        ///Values are chosen arbitrarily
        UIObject tempText = new UIObject(UITypes.Text, "PlayerScore", 402.5f, -107.5f, 475, 105, UserText);
        UIDisplay.Add(tempText); //[0] - Player score
        UIObject tempText2 = new UIObject(UITypes.Text, "GameTimer", 540, 307.5f, 200, 105, UserText);
        UIDisplay.Add(tempText2); //[1] - Game timer

        ///Create Minimap
        /// Values are chosen arbitrarily
        UIObject temp5 = new UIObject(UITypes.Minimap, "Minimap", -490, 210, 300, 300, Minimap);

        //test - inside loop = generic code for creating button.
        for (int i = 0; i < 3; i++)
        {
            UIObject temp2 = new UIObject(ButtonTypes.Bounce_Add_Type, "Button" + i.ToString(), 50.0f, UserButton, i);
            StartCoroutine(SetInitPositionForButtons(temp2));
            UIButtons.Add(temp2);
        }
        UIObject temp3 = new UIObject(ButtonTypes.AntiBounce_Add_Type, "Button3" , 1.0f, UserButton, 3);
        StartCoroutine(SetInitPositionForButtons(temp3));
        UIButtons.Add(temp3);

    }
    //correction for finding the localposition of buttons placed inside of Grid Layout (on the UIPanel).
    IEnumerator SetInitPositionForButtons(UIObject getPosition)
    {
        yield return new WaitForEndOfFrame();
        // Find position of objects in grid
        getPosition.SetButtonInitPosition(getPosition.GetUIBody().transform.localPosition);
    }

    /// <summary>
    /// Used for moving the camera with the OnDrag method in FixedJoystick.
    /// </summary>
    public void MoveCamera()
    {
        //create boundary using BoundaryX and BoundaryY
        if(inVector.Horizontal < 0.0f && CameraMain.transform.position.x + inVector.Horizontal > -BoundaryX )
        {
            CameraMain.transform.position += new Vector3(inVector.Horizontal, 0.0f, 0.0f);
        }
        else if(inVector.Horizontal > 0.0f && CameraMain.transform.position.x + inVector.Horizontal < BoundaryX)
        {
            CameraMain.transform.position += new Vector3(inVector.Horizontal, 0.0f, 0.0f);
        }

        if (inVector.Vertical > 0.0f && CameraMain.transform.position.z + inVector.Vertical < BoundaryZ)
        {
            CameraMain.transform.position += new Vector3(0.0f, 0.0f, inVector.Vertical);
        }
        else if (inVector.Vertical < 0.0f && CameraMain.transform.position.z + inVector.Vertical > -BoundaryZ)
        {
            CameraMain.transform.position += new Vector3(0.0f, 0.0f, inVector.Vertical);
        }

    }

    /// <summary>
    /// The main method for generating FloatingText to indicate changes in bounceforce, antiforce, etc.
    /// </summary>

    public void FloatingText(float theValue, Vector3 theLocation, AddOrSubtract theColor)
    {
        GameObject tempText = GameObject.Instantiate(FloatText);
        //choose the color
        switch(theColor)
        {
            case AddOrSubtract.Add:
                tempText.GetComponentInChildren<TextMesh>().color = Color.green;
                break;
            case AddOrSubtract.Subtract:
                tempText.GetComponentInChildren<TextMesh>().color = Color.red;
                break;
            case AddOrSubtract.Buff:
                tempText.GetComponentInChildren<TextMesh>().color = Color.blue;
                break;
            case AddOrSubtract.CountDown:
                tempText.GetComponentInChildren<TextMesh>().color = Color.black;
                break;
        }

        tempText.transform.position = theLocation;

        //The floating 3d object must be a child of the empty gameobject FloatText
        tempText.GetComponentInChildren<TextMesh>().text = theValue.ToString();
        Destroy(tempText, tempText.GetComponentInChildren<Animation>().clip.length);
    }
}

public class UIObject 
{

    private UITypes OBJECT_TYPE;
    private string OBJECT_NAME;
    private float OBJECT_XPOSITION;
    private float OBJECT_YPOSITION;
    private float OBJECT_WIDTH;
    private float OBJECT_HEIGHT;
    //for text only
    private Text OBJECT_TEXT;
    private GameObject temp;

    /// <summary>
    /// Construct object and place it, size it, and name it.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="objectName"></param>
    /// <param name="xPos"></param>
    /// <param name="yPos"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public UIObject(UITypes type, string objectName, float xPos, float yPos, float width, float height, GameObject UIBody)
    {
        OBJECT_TYPE = type;
        OBJECT_NAME = objectName;
        OBJECT_XPOSITION = xPos;
        OBJECT_YPOSITION = yPos;
        OBJECT_WIDTH = width;
        OBJECT_HEIGHT = height;

        switch (OBJECT_TYPE)
        {
            case UITypes.Joystick:
                temp = Object.Instantiate(UIBody);
                temp.transform.SetParent(GameObject.FindGameObjectWithTag("UI").transform);
                temp.name = OBJECT_NAME;
                temp.transform.position = new Vector2(OBJECT_XPOSITION, OBJECT_YPOSITION);
                temp.GetComponent<RectTransform>().sizeDelta = new Vector2(OBJECT_WIDTH, OBJECT_HEIGHT);
                temp.SetActive(true);
                break;
            //panels use Left Right Top Bottom
            // xPos = right
            // yPos = left
            // width = bottom
            // height = top
            case UITypes.Panel:
                temp = Object.Instantiate(UIBody);
                temp.transform.SetParent(GameObject.FindGameObjectWithTag("UI").transform);
                temp.name = OBJECT_NAME;
                temp.transform.position = new Vector2(OBJECT_XPOSITION, OBJECT_YPOSITION);
                temp.GetComponent<RectTransform>().offsetMax = new Vector2(-OBJECT_XPOSITION, -OBJECT_HEIGHT);
                temp.GetComponent<RectTransform>().offsetMin = new Vector2(OBJECT_YPOSITION, OBJECT_WIDTH);
                temp.SetActive(true);
                break;
            //Requires resizing of children (temporary) 
            case UITypes.Text:
                //temporary for resizing text ui objects.
                float frameSize = 10.0f; //size of frame
                temp = Object.Instantiate(UIBody);
                OBJECT_TEXT = temp.transform.GetChild(1).GetComponent<Text>(); //GetChild(1) is the text child.
                temp.transform.SetParent(GameObject.FindGameObjectWithTag("UI").transform);
                temp.name = OBJECT_NAME;
                temp.transform.localPosition = new Vector2(OBJECT_XPOSITION, OBJECT_YPOSITION);
                temp.GetComponent<RectTransform>().sizeDelta = new Vector2(OBJECT_WIDTH, OBJECT_HEIGHT);
                temp.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(OBJECT_WIDTH - frameSize, OBJECT_HEIGHT - frameSize);
                temp.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(OBJECT_WIDTH - frameSize, OBJECT_HEIGHT - frameSize);
                temp.SetActive(true);
                break;
            case UITypes.Minimap:
                temp = Object.Instantiate(UIBody);
                temp.transform.SetParent(GameObject.FindGameObjectWithTag("UI").transform);
                temp.name = OBJECT_NAME;
                temp.transform.localPosition = new Vector2(OBJECT_XPOSITION, OBJECT_YPOSITION);
                temp.GetComponent<RectTransform>().sizeDelta = new Vector2(OBJECT_WIDTH, OBJECT_HEIGHT);
                temp.SetActive(true);
                break;
                
        }


    }
    //overload for Button.
    private float BOUNCE_ADD;
    private ButtonTypes BUTTON_TYPE;
    private Vector3 BUTTON_INIT_POSITION;
    private int BUTTON_ID;
    public UIObject(ButtonTypes buttontype, string objectName, float bounceAdd, GameObject buttonType, int id)
    {
        OBJECT_NAME = objectName;
        BOUNCE_ADD = bounceAdd;
        BUTTON_TYPE = buttontype;
        BUTTON_ID = id;

        temp = Object.Instantiate(buttonType);
        temp.name = OBJECT_NAME;
        temp.transform.SetParent(GameObject.FindGameObjectWithTag("UserPanel").transform);

        

        switch (BUTTON_TYPE)
        {
            case ButtonTypes.Bounce_Add_Type:
                temp.transform.GetChild(0).GetComponent<Text>().text = "B: "+ bounceAdd.ToString();
                break;
            case ButtonTypes.AntiBounce_Add_Type:
                temp.transform.GetChild(0).GetComponent<Text>().text = "AB: " + bounceAdd.ToString();
                break;
        }
        
        

    }

    //Button getters/setters

    public void SetButtonInitPosition(Vector3 initPos)
    {
        BUTTON_INIT_POSITION = initPos;
    }
    public void SetButtonID(int newID)
    {
        BUTTON_ID = newID;
    }
    public int GetButtonID()
    {
        return BUTTON_ID;
    }
    public ButtonTypes GetButtonType()
    {
        return BUTTON_TYPE;
    }
    public Vector3 GetButtonInitPosition()
    {
        return BUTTON_INIT_POSITION;
    }
    public float GetBounceAdd()
    {
        return BOUNCE_ADD;
    }

    //general getter/setter for UI elements
    public string GetUIName()
    {
        return OBJECT_NAME;
    }
    public GameObject GetUIBody()
    {
        return temp;
    }
    public Text GetUIText()
    {
        return OBJECT_TEXT;
    }

}

