using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //* ---------- Singleton Pattern -------------- */
    public static GameManager instance = null;


    //* ------------ Public Accesor Variables -------- */
    public int level = 1;
    public int numOfEnemy = 1;
    public bool easyMode = false;

    //* --------------- Events ------------*/
    public static event Action<int> beatLevel;

    //* ---------------- Inspector Reference ---------- */
    [SerializeField] private GameObject winScreen;
    //public Toggle easyModeToggle;

    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
    }
   
    void Start()
    {
        //Call the InitGame function to initialize the first level 
        InitGame();
    }

    public int GetCurrentLevel()
    {
        return level;
    }

    public void LevelCompleted()
    {
        level++;
        numOfEnemy = 1;
        if (level > 4)
        {
            Debug.Log("Game is over! You won!");
            //winScreen.SetActive(true);
        }
        else
        {
            TilemapSetup.Instance.NewLevel(level);
            beatLevel?.Invoke(level);
        }
    }

    //Initializes the game for each level.
    void InitGame()
    {
        //Call the SetupScene function of the BoardManager script, pass it current level number.
        //boardScript.SetupScene(level);
        TilemapSetup.Instance.NewLevel(level);
    }


    public void ToggleEasyMode()
    {
        //easyMode = easyModeToggle.isOn;
    }
    
}
