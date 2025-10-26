using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    //* ---------- Singleton Pattern -------------- */
    public static GameManager instance = null;


    //* ------------ Public Accesor Variables -------- */
    public int stageLevel = 1;
    public int highestLevel;
    public int numOfEnemy = 1;
    public bool easyMode = false;

    //* ---------------- Level Completed Settings ------- *//
    public float blackScreenFadeTime;
    public float weaponFadeTime;
    public float weaponMoveTime;
    public Image blackScreen;
    public Image weaponImage;
    public TMP_Text congratulationsText;
    public Sprite[] weaponSprites;


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
        //Call the InitGame function to initialize the first stageLevel 
        InitGame();
    }

    public int GetStageLevel()
    {
        return stageLevel;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log($"Completing Level {stageLevel}");
            LevelCompleted();
        }
    }

    public void LevelCompleted()
    {
        StartCoroutine(DoLevelCompleted());
    }

    private IEnumerator DoLevelCompleted()
    {
        //Fade into a black screen
        float elapsed = 0f;
        while (elapsed < blackScreenFadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / blackScreenFadeTime;
            blackScreen.color = new Color(0, 0, 0, t);
            yield return null;
        }

        yield return new WaitForSeconds(.5f);

        //SET THE WEAPON IMAGE TO PROPER WEAPON
        weaponImage.sprite = weaponSprites[stageLevel];

        //Fade in the new weapon
        elapsed = 0f;
        while (elapsed < weaponFadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / blackScreenFadeTime;
            weaponImage.color = new Color(1, 1, 1, t);
            yield return null;
        }

        yield return new WaitForSeconds(.5f);

        WeaponHandlerUI.Instance.lockImages[stageLevel].gameObject.SetActive(false);

        //Enable, "You unlocked the ..."
        congratulationsText.color = new Color(1, 1, 1, 0);
        congratulationsText.text = $"Congratulations!\nYou unlocked the {WeaponHandler.Instance.GetWeapons()[stageLevel].getName()} wand!";
        elapsed = 0f;
        while (elapsed < weaponFadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / blackScreenFadeTime;
            congratulationsText.color = new Color(1, 1, 1, t);
            yield return null;
        }

        //Weapon image lerps size + position down into the slot it belongs t
        Transform targetPosition = WeaponHandlerUI.Instance.lockImages[stageLevel].transform;
        float targetWidth = WeaponHandlerUI.Instance.lockImages[stageLevel].GetComponent<RectTransform>().rect.width;
        float targetHeight = WeaponHandlerUI.Instance.lockImages[stageLevel].GetComponent<RectTransform>().rect.height;
        RectTransform weaponRect = weaponImage.GetComponent<RectTransform>();
        elapsed = 0f;
        while (elapsed < weaponMoveTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / weaponMoveTime;
            weaponImage.transform.position = Vector2.Lerp(weaponImage.transform.position, targetPosition.position, t);
            weaponRect.sizeDelta = new Vector2(Mathf.Lerp(weaponRect.rect.width, targetWidth, t), Mathf.Lerp(weaponRect.rect.height, targetHeight, t));
            yield return null;
        }

        stageLevel++;
        highestLevel = Mathf.Max(stageLevel, highestLevel);
        numOfEnemy = 1;
        if (stageLevel > 4)
        {
            Debug.Log("Game is over! You won!");
            //winScreen.SetActive(true);
        }
        else
        {
            TilemapSetup.Instance.NewLevel(stageLevel);
            beatLevel?.Invoke(stageLevel);
        }

        yield return new WaitForSeconds(.25f);
        elapsed = 0f;
        while (elapsed < blackScreenFadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / blackScreenFadeTime;
            blackScreen.color = new Color(0, 0, 0, 1 - t);
            weaponImage.color = new Color(1, 1, 1, 1 - t);
            congratulationsText.color = new Color(1, 1, 1, 1 - t);
            yield return null;
        }
    }

    //Initializes the game for each stageLevel.
    void InitGame()
    {
        //Call the SetupScene function of the BoardManager script, pass it current stageLevel number.
        //boardScript.SetupScene(stageLevel);
        TilemapSetup.Instance.NewLevel(stageLevel);
    }


    public void ToggleEasyMode()
    {
        //easyMode = easyModeToggle.isOn;
    }

}
