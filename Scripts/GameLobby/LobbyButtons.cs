using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyButtons : MonoBehaviour
{


    ///Used for containing the methods for the MainMenu/Lobby buttons.
    ///Also used for Lobby/MainMenu Labels when necessary.
    [Header("Global Lobby Variables")]
    public Text PlayerName;
    public Text HighestCompletedLevel;
    public Text HighestScoreObtained;
    public InputField QuickSearch;
    private int IsNumber; //used to ensure people are only putting numbers in QuickSearch
    private Game PlayersGame;
    private int LowestPossibleLevel = 1; //The lowest possible level you can load into.
    private int CurrentFloatedLevel; //The level currently being considered for loading.

    //[Header("Cosmetic Menu")]
    //public Dropdown SkinSelector;
    //public GameObject ImageExample;

    private void Start()
    {
        foreach (Game game in GlobalDataManager.savedGames)
        {
            if (game != null && GlobalDataManager.GetGamesId() == game.GetCharacterName())
            {
                PlayersGame = game;
                PlayerName.text = PlayersGame.GetCharacterName(); //Display the player's name.
                HighestCompletedLevel.text = "Highest Completed Level: " + PlayersGame.GetHighestCompletedLevel().ToString(); //display the highest level completed.
                CurrentFloatedLevel = PlayersGame.GetHighestCompletedLevel() + 1;
                HighestScoreObtained.text = "Highest Score Obtained: " + PlayersGame.GetMyHighScore(CurrentFloatedLevel - 1);//-1 for list index 
                Indicator.text = "Level: " + CurrentFloatedLevel.ToString(); //Get ready to immediately load current level
                GlobalDataManager.SetTheLevel(CurrentFloatedLevel);
            }

        }

    }

    private void Update()
    {
        if(QuickSearch != null && QuickSearch.text != " " && int.TryParse(QuickSearch.text, out IsNumber))
        {
            int.TryParse(QuickSearch.text, out IsNumber);
            if(IsNumber > PlayersGame.GetHighestCompletedLevel() + 1)
            {
                IsNumber = PlayersGame.GetHighestCompletedLevel() + 1;
            }
            else if (IsNumber < LowestPossibleLevel)
            {
                IsNumber = LowestPossibleLevel;
            }
            CurrentFloatedLevel = IsNumber;
            Indicator.text = "Level: " + CurrentFloatedLevel.ToString(); //Get ready to immediately load current level
            GlobalDataManager.SetTheLevel(CurrentFloatedLevel);
        }
    }

    //Main selection method
    public void PressStartButton(GameObject canvas)
    {
        if(!canvas.activeSelf)
        {
            canvas.SetActive(true);
        }
    }

    //Main close panel method
    public void CloseButton(GameObject canvas)
    {
        if(canvas.activeSelf)
        {
            canvas.SetActive(false);
        }
    }

    ///Methods and variables specific to Level Selection canvas.
    ///Work on this. Adding a way to indicate which level to load into.
    [Header("Level Select")]
    public Text Indicator;
    

    public void ChangeLevel(bool Direction)
    {
        //Check for a quick search attempt
        if(QuickSearch.text != " ")
        {
            QuickSearch.Select();
            QuickSearch.text = " ";
        }
        

        if (Direction && CurrentFloatedLevel < (PlayersGame.GetHighestCompletedLevel() + 1))//increase
        {
            Indicator.text = "Level: " + (CurrentFloatedLevel + 1).ToString();
            CurrentFloatedLevel += 1;
            GlobalDataManager.SetTheLevel(CurrentFloatedLevel);
            HighestScoreObtained.text = "Highest Score Obtained: " + PlayersGame.GetMyHighScore(CurrentFloatedLevel - 1);

        }
        if(!Direction && CurrentFloatedLevel > LowestPossibleLevel)//decrease
        {
            Indicator.text = "Level: " + (CurrentFloatedLevel - 1).ToString();
            CurrentFloatedLevel -= 1;
            GlobalDataManager.SetTheLevel(CurrentFloatedLevel);
            HighestScoreObtained.text = "Highest Score Obtained: " + PlayersGame.GetMyHighScore(CurrentFloatedLevel - 1);
        }
    }

}
