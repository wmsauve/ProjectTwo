using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollision : MonoBehaviour
{
    ///Component for detecting the ball collision.
    ///Temporary until I find a more compact way of doing this.
    ///Temporarily contains the bounce numerical information of the ball (Should probably be contained in the Ball class, but OnCollisionEnter has to be on the gameobject).
    ///

    //Ball Numerical Values
    private float MyBounce;
    private float MyAntiBounce;
    private float MyFloorForce; //amount of bounce removed by contacting the floor
    private int MyID;

    public void SetMyBounce(float bounce)
    {
        MyBounce = bounce;
    }
    public void SetMyAntiBounce(float antibounce)
    {
        MyAntiBounce = antibounce;
    }
    public void SetMyFloorForce(float floorforce)
    {
        MyFloorForce = floorforce;
    }
    public void SetMyID(int id)
    {
        MyID = id;
    }


    public float GetMyBounce()
    {
        return MyBounce;
    }
    public float GetMyAntiBounce()
    {
        return MyAntiBounce;
    }
    public float GetMyFloorForce()
    {
        return MyFloorForce;
    }
    public int GetMyID()
    {
        return MyID;
    }
    /// <summary>
    /// My Collision.
    /// Bounce me based on MyBounce once I collide with a Floor.
    /// </summary>
    /// <param name="col"></param>
    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Floor")
        {
            //Debug.Log(MyBounce);
            Vector3 dirVector = (gameObject.transform.position - col.transform.position).normalized;
            MyBounce -= MyFloorForce;
            //floatingtext
            manager.FloatingText(MyFloorForce, gameObject.transform.position, AddOrSubtract.Subtract);
            manager.UpdatePlayerScore(MyBounce); 
            gameObject.GetComponent<Rigidbody>().AddForce(dirVector * MyBounce);
            
        }

    }
    //Correction to make sure the ball is always bouncing on the spot.
    //The machine precision error is causing the ball to not bounce properly on angled surfaces.
    //Seriously, that is fucking retarded.
    private void FixedUpdate()
    {
        gameObject.transform.localPosition = new Vector3(0.0f, gameObject.transform.localPosition.y, 0.0f);
    }

    /// <summary>
    /// Reference to GameManager
    /// </summary>
    private GameManager manager;
    private void Awake()
    {
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
}
