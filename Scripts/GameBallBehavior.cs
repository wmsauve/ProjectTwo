using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager
{

    /// Ball Behavior
    /// Current Methods:
    /// BallManagerDecay
    /// PlayerClick
    /// PeriodicallySpawnBall

    private void BallManagerDecay(float decay)
    {
        
        foreach(Ball ball in BallList)
        {
            ball.DecayMyBounce(decay);
            //track when to instantiate a floating text.
            
        }
        TextTracker += Time.deltaTime;
        if (TextTracker >= DecayCooldownText)
        {
            foreach (Ball ball in BallList)
            {
                if(ball.GetMyInformation().GetMyBounce() >= ball.NotBouncingThreshold)
                {
                    FloatingText(decay - ball.GetMyInformation().GetMyAntiBounce(), ball.GetMyFloor().transform.position, AddOrSubtract.Subtract);
                }
                
            }
            TextTracker = 0.0f;
        }
    }

    private void CheckForMouseOverGlow()
    {
        if (DraggingButton)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {

                if (hit.transform.tag == "Ball")
                {
                    BallList[hit.transform.GetComponent<BallCollision>().GetMyID()].MouseOverGlow(Glow);
                    foreach (Ball ball in BallList)
                    {
                        if (ball.GetMyBody() != hit.transform.gameObject)
                        {
                            ball.MouseOverGlow(NotGlow);
                        }

                    }
                }
                else if (hit.transform.tag == "Floor")
                {
                    //child of floor must be ball with component BallCollision
                    BallList[hit.transform.GetComponentInChildren<BallCollision>().GetMyID()].MouseOverGlow(Glow);
                    foreach (Ball floor in BallList)
                    {
                        if (floor.GetMyFloor() != hit.transform.gameObject)
                        {
                            floor.MouseOverGlow(NotGlow);
                        }

                    }
                }
            }
            else
            {
                foreach (Ball ball in BallList)
                {
                    ball.MouseOverGlow(NotGlow);
                }

            }

        }
    }
    /// <summary>
    /// PlayerClick is the main way for 
    /// </summary>
    private void PlayerClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit) && !DraggingButton && DraggedButton == null)
            {


                if (hit.transform.tag == "Ball")
                {        
                    BallList[hit.transform.gameObject.GetComponent<BallCollision>().GetMyID()].ButtonAddition(ClickForce);
                    FloatingText(ClickForce, hit.point, AddOrSubtract.Add);
                }
                else if (hit.transform.tag == "Floor")
                {
                    //child of floor must be ball with component BallCollision
                    BallList[hit.transform.gameObject.GetComponentInChildren<BallCollision>().GetMyID()].ButtonAddition(ClickForce);
                    FloatingText(ClickForce, hit.point, AddOrSubtract.Add);
                }
                
            }
        }
    }

    /// <summary>
    /// Method for periodically spawning balls for the player to click to keep bouncing.
    /// </summary>
    private void PeriodicallySpawnBall()
    {
        //Run the check
        SpawnBallOkay();
        if(SpawnABall)
        {
            BallCoolingDown += Time.deltaTime;
            AddMoreBalls = true;
        }
        else
        {
            BallCoolingDown = 0.0f;
            AddMoreBalls = false;
        }
        
        //Spawn a ball if the check comes back true (ball is okay to spawn)
        if(BallCoolingDown >= CooldownForBalls && AddMoreBalls)
        {
            BallList.Add(new Ball(BounceForce, AntiBounceForce, FloorForce, TempBall, TempFloor, BoundaryX, BoundaryZ, AttemptedPosition, BallList.Count));
            BallIndex += 1;
            BallCoolingDown = 0.0f;
        }
    }
    //Bool used for determining if the game has room to stuff more balls
    private void SpawnBallOkay()
    {
        AttemptedPosition = new Vector3(Random.Range(-bufferZoneX, bufferZoneX), 0.0f, Random.Range(-bufferZoneZ, bufferZoneZ));
        float EffectDist = 1000.0f;
        int floorMask = 1 << 11;
        Collider[] hitColliders = Physics.OverlapSphere(AttemptedPosition, EffectDist, floorMask);
        int CorrectPlacement = 10; //Give the loop 10 attempts to find a correct placement
        int TryCounter = 0; //counter for finding a correct placement
        int PositionOkay = 0; // used for breaking the while loop and keep the okay position.

        if(hitColliders.Length == 0)
        {
            SpawnABall = true;
            return;
        }
        else
        {
            while (TryCounter < CorrectPlacement) //keep going until you find a safe position
            {
                //Check for safe location
                foreach (Collider other in hitColliders)
                {
                    if (Vector3.Distance(other.transform.position, AttemptedPosition) <= spawnDist)
                    {
                        //The random position is not safe
                        AttemptedPosition = new Vector3(Random.Range(-bufferZoneX, bufferZoneX), 0.0f, Random.Range(-bufferZoneZ, bufferZoneZ));
                        TryCounter += 1;
                        PositionOkay = 0;
                        break;
                    }
                    PositionOkay += 1;
                    //Break the while loop if we are in a safe location
                    if (PositionOkay == hitColliders.Length)
                    {
                        SpawnABall = true;
                        return;
                    }
                }
            }
        }

        SpawnABall = false;
    }

    /// <summary>
    /// method for handling balls that stop bouncing
    /// </summary>
    private void CheckForSleepingBalls()
    {
        for (int i = 0; i < BallList.Count; i++)
        {
            ////find a ball that is not bouncing
            if (BallList[i].NotBouncing())
            {
                //Give time before the ball is destroyed.
                BallList[i].DeathCounter -= Time.deltaTime;
                if(BallList[i].FloatTextDeathCounter())
                {
                    FloatingText(Mathf.RoundToInt(BallList[i].DeathCounter), BallList[i].GetMyFloor().transform.position, AddOrSubtract.CountDown);
                }
                
                if (BallList[i].DeathCounter <= 0.0f)
                {
                    //remove ball from list
                    Destroy(BallList[i].GetMyBody());
                    Destroy(BallList[i].GetMyFloor());
                    BallList.Remove(BallList[i]);
                    
                    for (int j = i; j < BallList.Count; j++)
                    {
                        BallList[j].DecreasedID();//fix integer position values for the remaining balls in the list.
                    }
                    break;
                }
            }
            else //The ball is still bouncing
            {
                BallList[i].SetDeathCounter(BallList[i].GetLifeDuration());
            }

        }
    }
}
