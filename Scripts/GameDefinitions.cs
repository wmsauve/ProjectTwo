using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI relevant enums - 
/// 1) ButtonTypes
/// 2) AddOrSubtract
/// 3) UITypes
/// 4) Minimap
/// </summary>
public enum ButtonTypes
{
    Bounce_Add_Type,
    AntiBounce_Add_Type,
}
public enum AddOrSubtract
{
    Add,
    Subtract,
    Buff,
    CountDown,
}
public enum UITypes
{
    Joystick,
    Panel,
    Text,
    Minimap,
}

/// <summary>
/// Enemy relevant enums -
/// 1)EnemyTypes
/// </summary>

public enum EnemyTypes
{
    BasicEnemy,
    HealerEnemy,
}

public enum EnemyGroupingTypes
{
    BasicGroup,

}

public partial class GameManager 
{

    /// Game Definitions
    /// Current Definitions:
    /// GameActive - main bool to determine if main methods should be running
    /// BounceForce - The force at which the ball is bounced off of its floor
    /// ClickForce - The amount of additional bounce force the player adds to the ball by clicking on it
    /// DecayForce - The amount of passive decay of the ball (loosely simulating air resistance)
    /// FloorForce - The amount of bounce force lost when the ball strikes the floor (loosely simulates a real bouncing ball)
    /// AntiBounceForce - The amount of shielding against DecayForce. The higher the number, the lower the "air resistance"
    /// inVector - Used to determine the joystick position
    /// BoundaryX - Used for the dimensions of the play area.
    /// BoundaryZ - Same as BoundaryX (Used in the event the play area is not a square)
    /// BeginCamera - bool for determining if the camera is moving
    /// DraggingButton - bool for determining if a button is being dragged by the player
    /// DraggedButton - The current button being dragged by the player
    /// 
    /// DecayCooldownText - Used to cooldown the text popup which displays the amount of Decay (air resistance) on the bouncing balls
    /// 
    /// Playing the Game related Definitions:
    /// CooldownForBalls - The time between spawning a ball for the player to click on
    /// BallCoolingDown - The variable that counts down the ball spawn.
    /// CooldownForEnemies - The time between enemies spawning
    /// EnemyCoolingDown - The variable that counts down the enemy spawn.
    /// BallIndex - Counter for the number of balls that were spawned in a match.
    /// EnemyIndex - Counter for the number of enemies that were spawned in a match.
    /// EnemiesDied - int used to track the number of mobs that have died/collided with a ball during a match.
    /// WhichWall - The location near which enemies will spawn
    /// WinCondition - The amount of points a player needs to reach certain reward thresholds.
    /// 
    /// Related to spawning balls:
    /// AddMoreBalls - Important bool for determining if the game should add more balls.
    /// SpawnABall - bool for running the position check.
    /// spawnDist - Currently Arbitrarily calculated distance between balls to stop them from colliding with each other.
    /// bufferZoneX/bufferZoneZ - Used to keep balls from spawning too close to walls.
    /// AttemptedPosition - position where we will spawn the floor.


    ///Current Prefabs: (These are subject to change in the future)
    ///TempBall - The basic ball being instantiated
    ///TempFloor - The basic floor being instantiated
    ///FloatText - The basic text being shown to indicate increases or decreases in health/bounce force/etc.
    ///TempEnemy - The basic enemy being instantiated

    ///Current GameObjects: (Some of these are subject to change in the future for aesthetic purposes)
    ///CameraMain - Reference the the main camera of the player (controlled by the joystick)
    ///MinimapCamera - Reference to the camera that creates the minimap.
    ///FixedJoystick - The left joystick used to move the camera 
    ///UserPanel - Panel for arranging and holding the players' buttons
    ///UserButton - The button used by the player to interact with the game
    ///Minimap - The minimap in the top left corner
    ///WallContainer - GameObject that contains the game's play area
    ///WallContainerTemp - Instantiated WallContainer
    ///ScoreScreen - Game object containing the score screen.


    ///Current Lists:
    ///BallList - 1) Main Ball
    ///EnemyList - Any Enemies in play
    ///UIButtons - Any Buttons in play
    ///UIDisplay - 1) PlayerScore (text) - Subject to change in the future for adding new UI elements/aesthetics/etc.

    ///Current Materials
    ///Glow - Used for outlining an object in game that the player wants to interact with using a button
    ///Not Glow - The basic outline of in game objects
    ///

    ///Current Player related variables
    ///PlayerScore - Used to track the player's score during a match
    ///GameMultiplier - Used to scale the game according to the player's defeated level.
    ///thisGame - Used to contain the information regarding the currently played game, loaded from GlobalDataManager

    private bool GameActive;

    private float BounceForce;
    private float ClickForce;
    private float DecayForce;
    private float FloorForce;
    private float AntiBounceForce;
    public FixedJoystick inVector;//Left joystick

    //Boundary of the map
    private float BoundaryX;
    private float BoundaryZ;
    public bool BeginCamera;

    //Related to creating the floating text for the decay.
    private float DecayCooldownText;
    private float TextTracker;

    //Buttons related
    [Header("Buttons Related")]
    public bool DraggingButton;
    public GameObject DraggedButton;
    private bool OneAtATime; 

    [Header("In Game Objects")]
    public GameObject TempBall;
    public GameObject TempFloor;
    public GameObject FloatText;
    public GameObject TempEnemy;
    public GameObject WallContainer;
    private GameObject WallContainerTemp;
    public GameObject ScoreScreen;


    [Header("UIObjects")]
    public GameObject CameraMain;
    public GameObject MinimapCamera;
    public GameObject FixedJoystick;
    public GameObject UserPanel;
    public GameObject UserButton;
    public GameObject UserText;
    public GameObject Minimap;


    [Header("Lists")]
    public List<Ball> BallList;
    public List<UIObject> UIButtons;
    public List<Enemy> EnemyList;
    public List<UIObject> UIDisplay;

    [Header("Materials")]
    public Material Glow;
    public Material NotGlow;

    //global variables
    public float PlayerScore;
    private float GameMultiplier;
    public Game thisGame;

    //Game playing related variables.
    private float CooldownForBalls;
    private float BallCoolingDown;
    private float CooldownForEnemies;
    private float EnemyCoolingDown;
    private int BallIndex;
    private int EnemyIndex;
    public int EnemiesDied; //used EnemyCollision
    private Vector3 WhichWall;
    private bool AddMoreBalls; //bool used for spawning balls
    private bool SpawnABall; //bool used for spwning balls
    private Vector3 AttemptedPosition;
    private float spawnDist = 12.0f;//5.0f is arbitrary for now, test later for ideal value.
    private float bufferZoneX;
    private float bufferZoneZ;
    private float WinCondition;

    void Awake()
    {
        GameActive = true;

        //temp values
        DecayCooldownText = 1.0f;
        TextTracker = 0.0f;
        BounceForce = 500.0f;
        ClickForce = 1.0f;
        AntiBounceForce = 5.0f;

        //work on this next...change texture based on player's choice.
        //NotGlow.mainTexture = ;
        //Glow.mainTexture = ;

        bufferZoneX = BoundaryX - spawnDist;
        bufferZoneZ = BoundaryZ - spawnDist;
        BeginCamera = false;
        DraggingButton = false;

        BallIndex = 0;

        BallList = new List<Ball>();
        UIButtons = new List<UIObject>();
        EnemyList = new List<Enemy>();
        UIDisplay = new List<UIObject>();

        //Initialization methods.
        UIInitialize();
        InitiateGame();
    }
}
