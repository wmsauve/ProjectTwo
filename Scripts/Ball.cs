using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball 
{
    /// <summary>
    /// Ball relevant methods and constructor.
    /// Linked with the BallCollision component that sits on MyBody (Where numerical information is temporarily stored).
    /// 
    /// Need to add way to stop balls potentially spawning too close to each other.
    /// </summary>
    private GameObject MyBody;
    private GameObject MyFloor;
    private Rigidbody MyRigidBody;
    private Vector3 MySafePosition;
    private BallCollision MyInformation;
    private Vector3 MyDirectionVector;
    private float gravity = 9.81f;//acceleration due to gravity (magnitude)

    private float MyBoundaryX;
    private float MyBoundaryZ;
    private float spawnDist = 5.0f;//5.0f is arbitrary for now, test later for ideal value.

    //important for determining who is targetted
    private int MyID;

    //Bounce specific variables
    public float NotBouncingThreshold = 50.0f; //Chosen through trial and error.
    private float BallMaxLife = 5.0f;//value for how long the ball can bounce before being destroyed. 5.0f : temp value
    public float DeathCounter = 5.0f; //count down until the ball is destroyed. Also appears on the floating text.
    private float DeathFloatingTextCountdown = 1.0f; //Float 1 second increments to alert player that a ball is dying
    private float TextCounter; //Count down until ball is destroyed (floating text counter).



    public Ball(float bounce, float antibounce, float floorforce, GameObject body, GameObject floor, float xVal, float zVal, Vector3 safepos, int ballID)
    {
        //SetBoundary for the ball, based on map size.
        MyBoundaryX = xVal - spawnDist;
        MyBoundaryZ = zVal - spawnDist;
        
        //Set gameobject related stuff
        MyFloor = GameObject.Instantiate(floor);
        MyBody = GameObject.Instantiate(body);
        MyRigidBody = MyBody.GetComponent<Rigidbody>();

        //Change the orientation of the ball to make for interesting bounce angles.
        MySafePosition = safepos;
        BallFloorOrientation();

        //get stats for the ball
        MyInformation = MyBody.GetComponent<BallCollision>();
        MyInformation.SetMyBounce(bounce);
        MyInformation.SetMyAntiBounce(antibounce);
        MyInformation.SetMyFloorForce(floorforce);
        MyInformation.SetMyID(ballID);

    }
    //Work on making balls not spawn on top of each other.
    private void BallFloorOrientation()
    {
        //place floor randomly along the y =0 plane
        MyFloor.transform.position = MySafePosition;
        MyBody.transform.SetParent(MyFloor.transform);

        //ball location relative to floor.
        Vector3 tempRandomPoint = Random.onUnitSphere;
        Vector3 RandomBallPoint = MyFloor.transform.position + tempRandomPoint * spawnDist; 
        if(RandomBallPoint.z >= MyFloor.transform.position.z)
        {
            //Correction for making sure that the ball is always facing the camera.
            //1) check for ball being on the other side of the floor.
            //2) find distance between ball and floor if ball is facing away from camera.
            //3) place ball at negative position relative to the ball.
            RandomBallPoint = new Vector3(RandomBallPoint.x, RandomBallPoint.y, RandomBallPoint.z - (2.0f * (MyFloor.transform.position.z - RandomBallPoint.z)));
        }
        //Mathf.Abs the y value so that we are always placing the ball above the floor(makes it easier for setting a straightforward direction vector for the gravity)
        MyBody.transform.position = new Vector3(RandomBallPoint.x, Mathf.Abs(RandomBallPoint.y), RandomBallPoint.z);


        //floor placed directly under the ball
        MyFloor.transform.LookAt(MyBody.transform);
        MyFloor.transform.Rotate(new Vector3(1.0f, 0, 0), 90);

        //the ball is always falling "downwards" (relative positions)
        MyDirectionVector = new Vector3(0.0f, -1.0f, 0.0f); 

    }

    //Stats Related Methods
    /// <summary>
    /// Decay Bounce
    /// reduce decay by MyAntiBounce
    /// </summary>
    /// <param name="decay"></param>
    public void DecayMyBounce(float decay)
    {
        if(GetMyInformation().GetMyBounce() >= 0.0f)
        {
            MyInformation.SetMyBounce(MyInformation.GetMyBounce() - (decay - MyInformation.GetMyAntiBounce()) * Time.deltaTime);
        }
    }
    /// <summary>
    /// Increase Bounce if an "add button" is applied to the ball.
    /// 
    /// addForce is a property of UIObject(button)
    /// </summary>
    /// <param name="addForce"></param>
    public void ButtonAddition(float addForce)
    {
        MyInformation.SetMyBounce(MyInformation.GetMyBounce() + addForce);

    }

    /// <summary>
    /// Reduce or increase MyAntiBuff based on debuff.
    /// </summary>
    /// <param name="debuff"></param>
    public void ChangeMyAntiBounce(float debuff)
    {
        MyInformation.SetMyAntiBounce(MyInformation.GetMyAntiBounce() + debuff);
        Debug.Log(MyInformation.GetMyAntiBounce());
    }

    /// <summary>
    /// Custom Gravity for ball.
    /// The ball will always fall towards the floor.
    /// </summary>
    public void Gravity()
    {
        MyRigidBody.AddRelativeForce(MyDirectionVector * gravity, ForceMode.Force);
    }

    //Ball getter/setters
    public BallCollision GetMyInformation()
    {
        return MyInformation;
    }
    public GameObject GetMyBody()
    {
        return MyBody;
    }
    public GameObject GetMyFloor()
    {
        return MyFloor;
    }
    public Rigidbody GetRigidbody()
    {
        return MyRigidBody;
    }

    public void SetDeathCounter(float resetVal)
    {
        DeathCounter = resetVal;
    }
    public float GetLifeDuration()
    {
        return BallMaxLife;
    }

    //Other purpose methods
    public void MouseOverGlow(Material changeToThis)
    {
        MyBody.GetComponent<Renderer>().material = changeToThis;
        MyFloor.GetComponent<Renderer>().material = changeToThis;
    }
    /// <summary>
    /// Use NotBouncingThreshold to return true if the ball is not going up and down.
    /// Determined through trial and error.
    /// </summary>
    /// <returns></returns>
    public bool NotBouncing()
    {
        if(MyInformation.GetMyBounce() <= NotBouncingThreshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //Used to create the floating text for the player to read off when the ball will be destroyed.
    //This starts once the ball has come to rest.
    public bool FloatTextDeathCounter() //Change to a countdown, work on next time.... Also work on changing the target of enemies.
    {
        
        if (NotBouncing())
        {
            TextCounter += Time.deltaTime;
            if(TextCounter >= DeathFloatingTextCountdown)
            {
                TextCounter = 0.0f;//reset cooldown
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    public void DecreasedID()
    {
        MyInformation.SetMyID(MyInformation.GetMyID() - 1);
    }
}
