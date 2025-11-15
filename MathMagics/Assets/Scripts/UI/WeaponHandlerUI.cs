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

  
}
