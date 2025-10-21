using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static GameStateManager;

public class GameStateManager : MonoBehaviour
{
    [System.Serializable] public class mEvent : UnityEvent { }

    //计算滑动次数，以便在游戏中为第一次滑动创建用于切换菜单的事件
    [HideInInspector] public int swipeCounter = 0;

    public static GameStateManager instance;

    public enum GameState
    {
        idle,
        gameActive,
        gameOver
    }

    [Tooltip("标识游戏当前所处的状态")]
    [ReadOnlyInspector] public GameState gamestate = GameState.idle;

    void loadGameState()
    {
        gamestate = (GameState)PlayerPrefs.GetInt("GameState");
        //枚举（enum）在 C# 中本质上是整数类型的包装，每个枚举值都会被编译器自动分配一个整数（默认从 0 开始递增）
    }

    void saveGameState()
    {
        PlayerPrefs.SetInt("GameState", (int)gamestate);
    }

    
    void Awake()
    {
        instance = this;
        loadGameState();
    }

    void Start()
    {
        StartCoroutine(OneFrameDelayStartup());
    }

    IEnumerator OneFrameDelayStartup()
    {
        // 因为 Awake 阶段的实例链接和启动时的注册操作，
        // 我们需要至少一帧的延迟来启动游戏。

        yield return null;
        yield return null;

        GameStartup();
    }

    void GameStartup()
    {
        //if we start with a gameover from the last game the game goes to idle.
        if (gamestate == GameState.gameOver)
        {
            gamestate = GameState.idle;
        }

        //if we are idle we trigger the start of a new game
        if (gamestate == GameState.idle)
        {
            StartGame();
        }
  
    }

    public void executeGameover()
    {
        gamestate = GameState.gameOver;

        //Debug.LogWarning("executeGameover");

        if (gamesPlayedCounter != null)
        {
            gamesPlayedCounter.increase(1); //log the number of played games
        }

        valueManager.instance.saveAllMinMaxValues();			//save min and max values for all values for the statistics tab
        if (HighScoreNameLinkerGroup.instance != null)
        {
            HighScoreNameLinkerGroup.instance.generateLinks();
        }
        CardStack.instance.resetCardStack();					//reset the card stack
        CardStack.instance.clearFollowUpStack();

        saveGameState();
        string currentSceneName = SceneManager.GetActiveScene().name;

        OnGameOver.Invoke();

        SceneManager.LoadScene(currentSceneName);                       //reload the scene for a clean startup of the game
    }

    public mEvent OnNewGame;
    public mEvent OnGameOver;
    public mEvent OnFirstSwipe;
    //mEvent 类型，这类事件的 “实现” 并非通过代码直接编写逻辑，而是通过 Unity 编辑器可视化绑定 来定义具体行为。

    public void swipe()
    {
        swipeCounter++;

        if (swipeCounter == 1)
        {
            OnFirstSwipe.Invoke();
        }
    }

    void StartGame()
    {
        swipeCounter = 0;
        if (gamestate == GameState.idle)
        {

            //do game start preparations
            OnNewGame.Invoke();

            if (CountryNameGenerator.instance != null)
            {
                CountryNameGenerator.instance.actualizeTexts(true);
                GenderGenerator.instance.actualizeUI();
                GameLogger.instance.clearGameLog();			//delete the last game log for the new game
            }


            gamestate = GameState.gameActive;
            saveGameState();
        }
    }

    void OnDestroy()
    {
        saveGameState();
    }

    public scoreCounter gamesPlayedCounter;
}
