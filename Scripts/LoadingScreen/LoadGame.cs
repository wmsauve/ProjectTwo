using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoadGame : MonoBehaviour
{
    ///Used to manage the buttons on the main screen.
    ///The main button is starting the game, which goes to the player's lobby.
    ///While going to the lobby via starting the game buttons, check save or load from the GlobalDataManager

    ///
    ///Currently
    ///0: MainMenu scene
    ///1: Lobby scene
    ///2: Main game scene
    ///

    [Header("Various loading screens")]
    public GameObject LoadingScreen; //used for setting active.
    public GameObject ScoreScreen; //used for setting active. Only used for main game.
    public Text EndGameMessage; //Display stats to the player after a match.
    private string endGameString; //The message.
    public Button ScoreScreenButton; //allow loading from score screen into game lobby.
    public GameObject LoadingBar; //set in the inspector
    private RectTransform LoadingBarTransform;
    private AsyncOperation operation;

    [Header ("Starting Game Buttons")]
    //Buttons deciding whether to continue game or start a new game
    public GameObject ContinueGame;
    public GameObject NewGame;
    //Buttons verifying which saved game or confirming your newgamename
    public GameObject FinishLoadingGame;
    public GameObject FinishMakingNewGame;
    //public GameObject SavedGameList;
    //public GameObject CreateNewGamePanel;

    [Header("Make or Load Game")]
    public Text NewGameName;
    public Dropdown SavedDropdown;

    private Game thisGame;
    private GameManager GameManager;

    private void Awake()
    {
        LoadingBarTransform = LoadingBar.GetComponent<RectTransform>();
        if (GameObject.FindGameObjectWithTag("GameManager") != null)
        {
            GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            thisGame = GameManager.thisGame;
        }

        
    }
    //reset saved games
    public void ResetSavedGames()
    {
        GlobalDataManager.savedGames = new List<Game>();
        GlobalDataManager.Save();
    }
    //Create a new saved game with name = NewGameName.text
    public void MakeNewGame()
    {
        if(NewGameName.text != "" && NewGameName.text != null && NoDuplicates())
        {
            Game.Current = new Game(NewGameName.text);
            GlobalDataManager.Save();
            GlobalDataManager.SetGamesId(NewGameName.text);
            if (!LoadingScreen.activeSelf)
            {
                LoadingScreen.SetActive(true);
            }
            StartCoroutine(LoadAsync(1));
        }
    }
    //Method for checking if there is a duplicate character name
    private bool NoDuplicates()
    {

        foreach(Game game in GlobalDataManager.savedGames)
        {
            if(game != null && NewGameName.text == game.GetCharacterName())
            {
                return false;
            }
        }
        return true;
    }

    //After picking which load game you want, load into the game lobby.
    public void LoadMatchContinueGame()
    {
        if (GlobalDataManager.savedGames.Count > 1) //check to see if at least one game is saved. Fix this.
        {
            GlobalDataManager.SetGamesId(SavedDropdown.options[SavedDropdown.value].text);
            if (!LoadingScreen.activeSelf)
            {
                LoadingScreen.SetActive(true);
            }
            StartCoroutine(LoadAsync(1));
        }
    }
    //Load match from the lobby. 
    public void LoadMatchLobby()
    {
        if (!LoadingScreen.activeSelf)
        {
            LoadingScreen.SetActive(true);
        }

        StartCoroutine(LoadAsync(2));
    }
    //End match from score screen.
    public void EndScoreScreen()
    {
        if (!ScoreScreen.activeSelf)
        {
            ScoreScreen.SetActive(true);
        }
        StartCoroutine(LoadScoreScreen(1));
    }

    //Loading menu
    IEnumerator LoadAsync(int whichScene)
    {
        operation = SceneManager.LoadSceneAsync(whichScene);
        while(!operation.isDone)
        {
            //fix progress bar
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            LoadingBarTransform.localScale = new Vector3(progress, LoadingBarTransform.localScale.y, LoadingBarTransform.localScale.z); 

            yield return null;
        }
    }
    /// <summary>
    /// The following two methods specifically handle the score screen that pops up after a player completes a match.
    /// </summary>
    /// <param name="whichScene"></param>
    /// <returns></returns>
    //Score screen
    IEnumerator LoadScoreScreen(int whichScene)
    {
        operation = SceneManager.LoadSceneAsync(whichScene);
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            //fix progress bar
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            LoadingBarTransform.localScale = new Vector3(progress, LoadingBarTransform.localScale.y, LoadingBarTransform.localScale.z);

            if (operation.progress >= 0.9f && ScoreScreenButton.gameObject.activeSelf == false)
            {
                ScoreScreenButton.gameObject.SetActive(true);
                endGameString = "Game Stats: \n " +
                    "Number of Enemies Killed: " + GameManager.EnemiesDied + "\n" +
                    "Points Gathered: " + GameManager.PlayerScore;
                    //"Fuck yourself: 3 \n " +
                    //"Eat a dick: 4 \n " +
                    //"Light your dick on fire you bitch: 5";
                EndGameMessage.text = endGameString;
            }

            yield return null;
        }
        
    }
    //press score screen load-lobby button
    public void FinishedLoadingScoreScreen()
    {
        //Update list to include new current level with 0 points gathered.
        thisGame.AddNewScore(0.0f);
        //Attempt to update High score
        //use -1 to get the proper index in the list, which starts at 0
        thisGame.AttemptToUpdateHighScore(GameManager.PlayerScore, GlobalDataManager.GetTheLevel() - 1);
        //Update completed levels
        thisGame.UpdateLevel();
        GlobalDataManager.SaveCurrent();//Update the player's completed level.
        if(!operation.allowSceneActivation)
        {
            operation.allowSceneActivation = true;
        }
    }
}
