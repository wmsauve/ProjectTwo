using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager
{

    /// Things related the the player playing the game.
    /// i.e. Player prefs, score, completed levels, etc.

    public void UpdatePlayerScore(float points)
    {
        PlayerScore += points;//temporary
    }
    //Display player's score 
    private void ScoreDisplayManager()
    {
        //update Score
        UIDisplay[0].GetUIText().text = "Player score: " + PlayerScore.ToString();
    }

    //Get the max allowed time for the player to complete the level and display it to the player.
    //Called in initialization function.
    private float placeholder_Timer;
    private void MainGameTimer()
    {
        placeholder_Timer = thisGame.GetLevelDuration() * GameMultiplier;
        //Timer
        UIDisplay[1].GetUIText().text = placeholder_Timer.ToString();
    }
    //Countdown method for the game
    private void CountDownGameTimer()
    {
        placeholder_Timer -= Time.deltaTime; 
        UIDisplay[1].GetUIText().text = placeholder_Timer.ToString();
    }


    //This method is used to adjust the state of the game based on how many levels have been completed. Change this to balance the game.
    private void RetrieveGameMultiplier()
    {
        //first, retrieve information regarding this currently loaded game (completed level)
        foreach (Game game in GlobalDataManager.savedGames)
        {
            if (game != null && game.GetCharacterName() == GlobalDataManager.GetGamesId())
            {

                thisGame = game;

                GameMultiplier = thisGame.CalculateUsedMultiplier();
                Debug.Log(GameMultiplier);
                //GameMultiplier variables.
                BoundaryX = 40.0f + GameMultiplier; //manipulate these two variables to grow/shrink the map
                BoundaryZ = 40.0f + GameMultiplier;
                CooldownForBalls = 10.0f * GameMultiplier;
                CooldownForEnemies = 10.0f / GameMultiplier;
                DecayForce = 10.0f * GameMultiplier;
                FloorForce = 20.0f * GameMultiplier;

                WinCondition = GlobalDataManager.GetTheLevel() * GameMultiplier; //temporary, find a way to make this tier'd after testing.
                //after lunch, add ending level with certain amount of points and a score screen.
            }
        }

    }

    //method for completing a level. Check for points, and load score screen
    //Next time, work on a high score of some kind, doot doot doot
    private void LevelCompleted()
    {
        if(PlayerScore >= 2000)
        {
            GameActive = false;
            ScoreScreen.GetComponent<LoadGame>().EndScoreScreen();
        }
    }
}
