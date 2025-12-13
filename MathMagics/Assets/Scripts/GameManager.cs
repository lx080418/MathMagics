using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

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
    private Vector2 originalWeaponSizeDelta;
    [Header("Magic Stone Reward")]
    public Image stoneImage;
    public Transform stoneTarget;
    private Vector2 originalStoneSizeDelta;



    //* --------------- Events ------------*/
    public static event Action<int> beatLevel;

    //* ---------------- Inspector Reference ---------- */
    [SerializeField] private PlayerMagicStone playerMagicStone;
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
        originalWeaponSizeDelta = weaponImage.rectTransform.sizeDelta;
        originalStoneSizeDelta = stoneImage.rectTransform.sizeDelta;
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
        PlayerPrefs.SetInt("level", stageLevel+1);
        Debug.Log($"Saved level {PlayerPrefs.GetInt("level")}");
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

        //If we have the weapon unlocked already, do something different

        if(WeaponHandler.Instance.GetWeapons()[stageLevel].getIsLocked())
        {
            yield return UnlockNextWeapon();
        }
        else
        {
            yield return GiveMagicStone();
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
            //weaponImage.color = new Color(1, 1, 1, 1 - t);
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="levelBeaten">The level the player was at when they reset</param>
    public void ResetGame()
    {
        PlayerPrefs.SetInt("level", stageLevel);
        //Set the level and all related variables back to normals
        PlayerPrefs.SetInt("magicStones", playerMagicStone.GetMagicStones());

        SceneManager.LoadScene("MainGame");
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("level");
    }

    private IEnumerator UnlockNextWeapon()
    {
        yield return new WaitForSeconds(.5f);

        //SET THE WEAPON IMAGE TO PROPER WEAPON
        weaponImage.rectTransform.sizeDelta = originalWeaponSizeDelta;
        weaponImage.sprite = weaponSprites[stageLevel];
        weaponImage.transform.localPosition = Vector2.zero;
        float elapsed = 0f;

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
        
        Vector2 startPos = weaponImage.transform.position;
        float startWidth = weaponRect.rect.width;
        float startHeight = weaponRect.rect.height;
        while (elapsed < weaponMoveTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / weaponMoveTime;
            weaponImage.transform.position = Vector2.Lerp(startPos, targetPosition.position, t);
            weaponRect.sizeDelta = new Vector2(Mathf.Lerp(startWidth, targetWidth, t), Mathf.Lerp(startHeight, targetHeight, t));
            yield return null;
        }
    }

    private IEnumerator GiveMagicStone()
    {
        yield return null;
        

        //Set the stone's initial location
        stoneImage.transform.localPosition = Vector2.zero;
        stoneImage.rectTransform.sizeDelta = originalStoneSizeDelta;

        float elapsed = 0f;

        //Fade in the new weapon
        elapsed = 0f;
        while (elapsed < weaponFadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / blackScreenFadeTime;
            stoneImage.color = new Color(1, 1, 1, t);
            yield return null;
        }

        yield return new WaitForSeconds(.5f);
        congratulationsText.color = new Color(1, 1, 1, 0);
        congratulationsText.text = $"Congratulations! You earned a magic stone!";
        elapsed = 0f;
        while (elapsed < weaponFadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / blackScreenFadeTime;
            congratulationsText.color = new Color(1, 1, 1, t);
            yield return null;
        }

         //Weapon image lerps size + position down into the slot it belongs t
        float targetWidth = stoneTarget.GetComponent<RectTransform>().rect.width;
        float targetHeight = stoneTarget.GetComponent<RectTransform>().rect.height;
        RectTransform stoneRect = stoneImage.GetComponent<RectTransform>();
        elapsed = 0f;
        
        Vector2 startPos = stoneRect.transform.position;
        float startWidth = stoneRect.rect.width;
        float startHeight = stoneRect.rect.height;
        while (elapsed < weaponMoveTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / weaponMoveTime;
            stoneRect.transform.position = Vector2.Lerp(startPos, stoneTarget.position, t);
            stoneRect.sizeDelta = new Vector2(Mathf.Lerp(startWidth, targetWidth, t), Mathf.Lerp(startHeight, targetHeight, t));
            yield return null;
        }

        playerMagicStone.GainMagicStone(1);

    }
}
