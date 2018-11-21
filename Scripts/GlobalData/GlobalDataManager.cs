using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GlobalDataManager : MonoBehaviour
{
    /// Used to track the players' Level completion, cosmetic choices, quest completion, etc.
    //used to ensure that there is only one global manager
    public static GlobalDataManager instance;
    //Player's loaded game choice
    private static string GamesID;
    public static void SetGamesId(string id)
    {
        GamesID = id;
    }
    public static string GetGamesId()
    {
        return GamesID;
    }
    //Player's level choice
    private static int TheLevel;
    public static void SetTheLevel(int level)
    {
        TheLevel = level;
    }
    public static int GetTheLevel()
    {
        return TheLevel;
    }

    [Header("Populate Saved Games Dropdown")]
    public GameObject SavedDropdown;

    //public List<Material> CurrentSkins = new List<Material>();
    void Awake()
    {
        // If the instance reference has not been set, yet, 
        if (instance == null)
        {
            // Set this instance as the instance reference.
            instance = this;
        }
        else if (instance != this)
        {
            // If the instance reference has already been set, and this is not the
            // the instance reference, destroy this game object.
            Destroy(gameObject);
        }

        // Do not destroy this object, when we load a new scene.
        DontDestroyOnLoad(gameObject);

        //pulled all current saved games.
        Load();
        PopulateDropDown();
        
    }

    //private void Update()
    //{
    //    for(int i = 1; i < savedGames.Count; i++)
    //    {
    //        Debug.Log(savedGames[i].GetCharacterName());
    //    }
        
    //}
    public static List<Game> savedGames = new List<Game>();

    //it's static so we can call it from anywhere
    public static void Save()
    {
        savedGames.Add(Game.Current);
        BinaryFormatter bf = new BinaryFormatter();
        //Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located
        FileStream file = File.Create(Application.persistentDataPath + "/savedGames.gd"); //you can call it anything you want
        bf.Serialize(file, savedGames);
        file.Close();
    }
    public static void SaveCurrent()
    {
        BinaryFormatter bf = new BinaryFormatter();
        //Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located
        FileStream file = File.Create(Application.persistentDataPath + "/savedGames.gd"); //you can call it anything you want
        bf.Serialize(file, savedGames);
        file.Close();
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/savedGames.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
            savedGames = (List<Game>)bf.Deserialize(file);
            file.Close();
        }
    }
    /// <summary>
    /// Happens right after load. Fill the saved game drop down with our saved games once they are loaded.
    /// </summary>
    private void PopulateDropDown()
    {
        List<string> SavedNames = new List<string>();
        for (int i = 0; i < savedGames.Count; i++)
        {
            if(savedGames[i] != null && savedGames[i].GetCharacterName() != null)
            {
                SavedNames.Add(savedGames[i].GetCharacterName());
            }
        }
        SavedDropdown.GetComponent<Dropdown>().AddOptions(SavedNames);
    }

}
/// <summary>
/// Used to create a saved game, where game completion, rewards, etc. are all saved.
/// </summary>
[System.Serializable]
public class Game
{
    /// <summary>
    /// Player stats related.
    /// </summary>
    public static Game Current;
    private int Completed_Level;
    private string CharacterName;
    private float GameMultiplier; //starts off as 1 (No multiplier for having not completed a single level yet)
    private float Level1GameDuration; //Level duration for the lowest possible level
    private List<float> MyHighScore; //Save the highest score achieved by the player.


    /// <summary>
    /// Cosmetic related
    /// </summary>
    //private List<Material> MySkins; 

    public Game(string name)
    {
        //Player stats
        Completed_Level = 0;
        Level1GameDuration = 180.0f;
        GameMultiplier = 1.01f;
        CharacterName = name;
        MyHighScore = new List<float>();
        MyHighScore.Add(0.0f);

        //Cosmetic
        //MySkins = new List<Material>();
        //MySkins.Add();
    }

    //getters/setters for Game
    public string GetCharacterName()
    {
        return CharacterName;
    }
    public int GetHighestCompletedLevel()
    {
        return Completed_Level;
    }
    public float GetCurrentMultiplier()
    {
        return GameMultiplier;
    }
    public float GetLevelDuration()
    {
        return Level1GameDuration; 
    }
    public float GetMyHighScore(int whichLevel)
    {
        return MyHighScore[whichLevel];
        
    }

    //general methods related to Game
    public void UpdateLevel()
    {
        Completed_Level += 1;;
    }
    public void AttemptToUpdateHighScore(float newScore, int whichLevel)
    {
        if(newScore > MyHighScore[whichLevel])
        {
            MyHighScore[whichLevel] = newScore;
        }
    }
    //called when a player completes a level to update the list containing high scores.
    public void AddNewScore(float newScore)
    {
        MyHighScore.Add(newScore);
    }

    public float CalculateUsedMultiplier()
    {
        //Multiplier is defined as 1.01 ^ (the level of the player)
        float temp = Mathf.Pow(GetCurrentMultiplier(), GlobalDataManager.GetTheLevel());
        return temp;
    }
    

}