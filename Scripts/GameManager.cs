using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{

    /// Game Manager
   
    void Update()
    {
        if(GameActive)//check for a pausing condition
        {
            //Check for mousing over a ball/floor pair to drop button
            CheckForMouseOverGlow();
            //Check for dropping button
            DroppingButton();

            //Check for PlayerClicking to increase bounce force
            PlayerClick();
            //Check for enemy moving
            foreach (Enemy enemy in EnemyList)
            {
                enemy.EnemyBehavior();
                if(BallList.Count > 0)
                {
                    enemy.EnemyMovement(BallList);
                }
                
                
            }
            //Check for decaying bounce force
            BallManagerDecay(DecayForce);

            //Spawn new ball and enemy
            PeriodicallySpawnBall();
            PeriodicallySpawnEnemy();

            //Check for updating and displaying the player's score
            ScoreDisplayManager();

            //Check for not bouncing balls
            //if (BallList.Count > 0)
            //{
            //    Debug.Log(BallList[0].NotBouncing());
            //}

            CheckForSleepingBalls();
            //Check for camera movement
            if (BeginCamera)
            {
                MoveCamera();
            }

            //count down until 0.0f;
            CountDownGameTimer();

            //Check for game win condition
            LevelCompleted();
        }

    }
    void FixedUpdate()
    {
        //Move my balls
        foreach (Ball ball in BallList)
        {
            ball.Gravity();
        }
    }

    //General method for starting off the whole game, called in Awake.
    void InitiateGame()
    {
        RetrieveGameMultiplier();
        BuildWorldInitialize();
        MainGameTimer();//initialize the timer.


        //temporarily spawn 2 ball
        for (int i = 0; i < 2; i++)
        {
            SpawnBallOkay();
            BallList.Add(new Ball(BounceForce, AntiBounceForce, FloorForce, TempBall, TempFloor, BoundaryX, BoundaryZ, AttemptedPosition, i));
            BallIndex += 1;
        }





        string teststring = "Ball wait = " + CooldownForBalls + " Enemy wait = " + CooldownForEnemies + " XMap size = " + BoundaryX + " ZMap size = " + BoundaryZ +
            " Timer = " + placeholder_Timer + " Floor force = " + FloorForce + " Decay force = " + DecayForce;

        Debug.Log(teststring);

    }
    //Temporarily only use BoundaryX. BoundaryY will become relevant when I'm ready to create rectangular zones
    private void BuildWorldInitialize()
    {
        //create walls according to the level.
        //position the walls according to the level.
        //Change minimap camera settings so that it views the entire playing field of the created level.
        WallContainerTemp = Instantiate(WallContainer);
        WallContainerTemp.transform.position = Vector3.zero; //always be centered on origin (temporary).
        //Child(0) = left, Child(1) = right, Child(2) = forward, Child(3) = back, Child(4) = floor
        float UnitLength = 1.0f;
        float WallDimensions = 2.0f * BoundaryX;
        WallContainerTemp.transform.GetChild(0).transform.position = new Vector3(-BoundaryX, 0.0f, 0.0f);
        WallContainerTemp.transform.GetChild(0).transform.localScale = new Vector3(UnitLength, WallDimensions, WallDimensions);

        WallContainerTemp.transform.GetChild(1).transform.position = new Vector3(BoundaryX, 0.0f, 0.0f);
        WallContainerTemp.transform.GetChild(1).transform.localScale = new Vector3(UnitLength, WallDimensions, WallDimensions);

        WallContainerTemp.transform.GetChild(2).transform.position = new Vector3(0.0f, 0.0f, BoundaryX);
        WallContainerTemp.transform.GetChild(2).transform.localScale = new Vector3(WallDimensions, WallDimensions, UnitLength);

        WallContainerTemp.transform.GetChild(3).transform.position = new Vector3(0.0f, 0.0f, -BoundaryX);
        WallContainerTemp.transform.GetChild(3).transform.localScale = new Vector3(WallDimensions, WallDimensions, UnitLength);

        WallContainerTemp.transform.GetChild(4).transform.position = new Vector3(0.0f, -BoundaryX, 0.0f);
        WallContainerTemp.transform.GetChild(4).transform.localScale = new Vector3(WallDimensions, UnitLength, WallDimensions);

        MinimapCamera.GetComponent<Camera>().orthographicSize = BoundaryX;
    }


}
