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
    [ReadOnlyInspector] public Gamestate gamestate = Gamestate.idle;
}
