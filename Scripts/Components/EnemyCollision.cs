using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    /// <summary>
    /// Contains numerical values for enemy and methods for the physical enemy body.
    /// </summary>

    //MyInformation related
    private float HealthPoints;
    private float currentHP; //used to track damage taken by the enemy
    private float Attack;
    private float MovementSpeed;
    private float Multiplier; //used for kamakaze attack into bouncing balls.
    private int ID;//use this to locate where the enemy is in the list.
    //getters and setters for numerical enemy values
    public void SetEnemyAttack(float att)
    {
        Attack = att;
    }
    public void SetEnemyHealth(float hp)
    {
        HealthPoints = hp;
        currentHP = hp;
    }
    public void SetEnemyMovement(float speed)
    {
        MovementSpeed = speed;
    }
    public void SetEnemyID(int id)
    {
        ID = id;
    }
    public void SetEnemyCrashMultiplier(float multi)
    {
        Multiplier = multi;
    }

    public float GetEnemyAttack()
    {
        return Attack;
    }
    public float GetEnemyHealth() //used to adjust damage taken/health gained
    {
        return currentHP;
    }
    public float GetEnemyMovement()
    {
        return MovementSpeed;
    }
    public int GetEnemyID()
    {
        return ID;
    }

    //only accessed within EnemyCollision
    private GameManager Manager;
    private GameObject HealthBar;
    private void Awake()
    {
        Manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        //GetChild(0) = Enemy Canvas, GetChild(2) is the health bar
        HealthBar = gameObject.transform.GetChild(0).GetChild(2).gameObject;

    }
    /// <summary>
    /// For adding to the decay of the bouncing ball.
    /// </summary>
    /// <param name="col"></param>
	private void OnTriggerEnter(Collider col)
    {
        BallCollision temp = col.gameObject.GetComponentInChildren<BallCollision>();
        temp.SetMyBounce(temp.GetMyBounce() - Attack * Multiplier);
        Manager.FloatingText(Attack * Multiplier, col.gameObject.transform.position, AddOrSubtract.Subtract);
        //safe removal of enemy.
        Manager.EnemyList.Remove(Manager.EnemyList[ID]);
        for (int i = ID; i < Manager.EnemyList.Count; i++)
        {
            Manager.EnemyList[i].DecreasedID();
            Manager.EnemiesDied += 1;
        }
        
        
        Destroy(gameObject);
    }

    //Attack the enemy with clicks.
    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            currentHP -= Manager.GetClickForce();
            Manager.FloatingText(Manager.GetClickForce(), gameObject.transform.position, AddOrSubtract.Subtract);
            HealthBar.transform.localScale = new Vector3(currentHP / HealthPoints, HealthBar.transform.localScale.y, HealthBar.transform.localScale.z);
            if (currentHP <= 0.0f)
            {
                DieFromClicks();
            }
        }
    }

    private void DieFromClicks()
    {
        //safe removal of enemy.
        Manager.EnemyList.Remove(Manager.EnemyList[ID]);
        for (int i = ID; i < Manager.EnemyList.Count; i++)
        {
            Manager.EnemyList[i].DecreasedID();
            Manager.EnemiesDied += 1;
        }
        Destroy(gameObject);
    }

    //Method for used by healer enemies to heal other enemies
    public void HealEnemy(float healing)
    {
        currentHP += healing;
        if(currentHP >= HealthPoints)
        {
            currentHP = HealthPoints;
        }
        Manager.FloatingText(healing, gameObject.transform.position, AddOrSubtract.Add);
        HealthBar.transform.localScale = new Vector3(currentHP / HealthPoints, HealthBar.transform.localScale.y, HealthBar.transform.localScale.z);
    }
}
