using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager 
{
    ///
    /// Enemies who add decay/attack antibounce.
    ///


    //Used for the player clicking enemies.
    public float GetClickForce()
    {
        return ClickForce;
    }


    /// <summary>
    /// Method for periodically spawning enemies.
    /// Currently - 
    /// 100% to spawn Basic enemy group
    /// </summary>
    private void PeriodicallySpawnEnemy()
    {
        EnemyCoolingDown += Time.deltaTime;
        if (EnemyCoolingDown >= CooldownForEnemies)
        {
            int whichGroup = 0; //temporary

            //Find the wall that the enemies will spawn from.
            CalculateWall();
            switch (whichGroup)
            {
                case (int)EnemyGroupingTypes.BasicGroup:
                    EnemyList.Add(new Enemy(EnemyTypes.BasicEnemy, 10.0f, 10.0f, 0.0f, 0.0f, 10.0f, 2.0f, TempEnemy, WhichWall, EnemyList.Count));
                    EnemyIndex += 1;
                    EnemyList.Add(new Enemy(EnemyTypes.HealerEnemy, 3.0f, 5.0f, 1.0f, 10.0f, 5.0f, 1.0f, TempEnemy, WhichWall, EnemyList.Count));
                    EnemyIndex += 1;
                    EnemyCoolingDown = 0.0f;
                    break;
            }
            
        }
    }
    //Used to pick a random wall for enemies to spawn from.
    private void CalculateWall()
    {
        int tempint = Random.Range(0, 3);
        WhichWall = WallContainerTemp.transform.GetChild(tempint).transform.position;
    }
}



public class Enemy
{

    private EnemyCollision MyInformation;
    private GameObject Body;
    private GameObject MyTarget;
    private EnemyTypes MyType;

    //used for the EnemyBehavior method
    private float EnemyAbilityCooldown; 
    private float CooldownTracker;
    private float EffectDist; //effects radius/range of any particular ability an enemy might have.

    public Enemy(EnemyTypes type, float att, float multiplier, float cd, float effect, float hp, float speed, GameObject enemyBody, Vector3 spawn, int i)
    {
        MyType = type;
        Body = GameObject.Instantiate(enemyBody);
        //set numerical values for enemy
        MyInformation = Body.GetComponent<EnemyCollision>();
        MyInformation.SetEnemyAttack(att);
        MyInformation.SetEnemyHealth(hp);
        MyInformation.SetEnemyMovement(speed);
        MyInformation.SetEnemyID(i); //track the position of enemies within the EnemyList
        MyInformation.SetEnemyCrashMultiplier(multiplier);
        //used for handling any monster's specialized behaviors (abilities, attack cd, etc.)
        EnemyAbilityCooldown = cd;
        CooldownTracker = 0.0f;
        EffectDist = effect;

        Body.transform.position = new Vector3(spawn.x + Random.Range(0, 5), 0, spawn.z + Random.Range(0, 5)); //testing
        
    }

    //general getters for enemy
    public GameObject GetEnemyBody()
    {
        return Body;
    }
    public EnemyCollision GetEnemyInformation()
    {
        return MyInformation;
    }


    //general methods for enemy
    public void EnemyMovement(List<Ball> whereAmIGoing)
    {
        MyTarget = CalculateClosestBall(whereAmIGoing);
        Body.transform.Translate(dirVec * MyInformation.GetEnemyMovement() * Time.deltaTime);
    }
    public void EnemyBehavior()
    {
        int badGuyMask = 1 << 9;
        CooldownTracker += Time.deltaTime;
        if (CooldownTracker >= EnemyAbilityCooldown)
        {
            switch (MyType)
            {
                case EnemyTypes.HealerEnemy:
                    Collider[] hitColliders = Physics.OverlapSphere(Body.transform.position, EffectDist, badGuyMask);
                    int i = 0;
                    while (i < hitColliders.Length)
                    {
                        hitColliders[i].gameObject.GetComponent<EnemyCollision>().HealEnemy(MyInformation.GetEnemyAttack());
                        i++;
                    }
                    break;
            }
            CooldownTracker = 0.0f;
        }
    }

    /// <summary>
    /// Find the ball I am supposed to be walking to.
    /// </summary>
    /// <param name="balls"></param>
    /// <returns></returns>
    private Vector3 dirVec;
    private GameObject CalculateClosestBall(List<Ball> balls)
    {
        float closestDistance = float.MaxValue;
        GameObject closestBall = null;

        foreach(Ball ball in balls)
        {
            if(Vector3.Distance(Body.transform.position, ball.GetMyFloor().transform.position) < closestDistance)
            {
                closestDistance = Vector3.Distance(Body.transform.position, ball.GetMyFloor().transform.position);
                closestBall = ball.GetMyFloor();
            }
        }
        dirVec = (closestBall.transform.position - Body.transform.position).normalized;
        return closestBall;
    }

    public void DecreasedID()
    {
        MyInformation.SetEnemyID(MyInformation.GetEnemyID() - 1);
    }
}
