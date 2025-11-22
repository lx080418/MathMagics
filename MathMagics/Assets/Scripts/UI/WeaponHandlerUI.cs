using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class WeaponHandlerUI : MonoBehaviour
{
    public static WeaponHandlerUI Instance;
    public Sprite slotSelectedSprite;
    public Sprite slotSprite;
    public Image[] images;
    public GameObject[] lockImages;
    public GameObject[] weaponImages;
    public GameObject[] labelImages;
    public WeaponLevelUI[] weaponLevelUIs;
    public Button[] weaponChangeArrows; //1 + 2 are Weapon 1, Subtract and Add, pattern continues

    private int currentWeaponIndex;

    private void OnEnable()
    {
        Instance = this;

        foreach (GameObject go in lockImages)
        {
            go.SetActive(true);
        }
        foreach (GameObject go in weaponImages)
        {
            go.SetActive(false);
        }
        foreach (GameObject go in labelImages)
        {
            go.SetActive(false);
        }

        lockImages[0].SetActive(false);
        weaponImages[0].SetActive(true);
        labelImages[0].SetActive(true);
    }
    private void Start()
    {
        GameManager.beatLevel += HandleBeatLevel;
        WeaponHandler.Instance.weaponSelected += HandleWeaponSelected;
        WeaponHandler.Instance.HandleWeaponLevelChanged += HandleWeaponLevelChanged;
        WeaponHandler.Instance.weaponForceUnlocked += HandleWeaponForceUnlocked;

        SetupLevelChangingArrows();
    }
    private void OnDisable()
    {
        GameManager.beatLevel -= HandleBeatLevel;
        WeaponHandler.Instance.weaponSelected -= HandleWeaponSelected;
        WeaponHandler.Instance.HandleWeaponLevelChanged -= HandleWeaponLevelChanged;

    }

    private void HandleBeatLevel(int level)
    {
        lockImages[level - 1].SetActive(false);
        weaponImages[level - 1].SetActive(true);
        labelImages[level - 1].SetActive(true);
    }

    private void HandleWeaponForceUnlocked(int level)
    {
        lockImages[level].SetActive(false);
        weaponImages[level].SetActive(true);
        labelImages[level].SetActive(true);
    }
    private void HandleWeaponSelected(int level)
    {
        foreach (Image img in images)
        {
            img.sprite = slotSprite;
        }
        images[level].sprite = slotSelectedSprite;
        currentWeaponIndex = level;
    }

    private void HandleWeaponLevelChanged(Weapon weapon)
    {
        weaponLevelUIs[WeaponHandler.Instance.GetWeaponIndexByName(weapon.getName())].ChangeWeaponLevelText(weapon.getLevel());
    }

    
    private void SetupLevelChangingArrows()
    {
        Debug.Log("Adding Button Logic!");
        Weapon[] weapons = WeaponHandler.Instance.GetWeapons();
        weaponChangeArrows[0].onClick.AddListener(weapons[0].DecreaseLevel);
        weaponChangeArrows[1].onClick.AddListener(weapons[0].IncreaseLevel);
        weaponChangeArrows[2].onClick.AddListener(weapons[1].DecreaseLevel);
        weaponChangeArrows[3].onClick.AddListener(weapons[1].IncreaseLevel);
        weaponChangeArrows[4].onClick.AddListener(weapons[2].DecreaseLevel);
        weaponChangeArrows[5].onClick.AddListener(weapons[2].IncreaseLevel);
        weaponChangeArrows[6].onClick.AddListener(weapons[3].DecreaseLevel);
        weaponChangeArrows[7].onClick.AddListener(weapons[3].IncreaseLevel);
        
    }

  
}
