using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class WeaponHandlerUI : MonoBehaviour
{
    public Sprite slotSelectedSprite;
    public Sprite slotSprite;
    public Image[] images;
    public GameObject[] lockImages;
    public GameObject[] weaponImages;
    public GameObject[] labelImages;

    private void OnEnable()
    {
        
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
    }
    private void OnDisable()
    {
        GameManager.beatLevel -= HandleBeatLevel;
        WeaponHandler.Instance.weaponSelected -= HandleWeaponSelected;

    }

    private void HandleBeatLevel(int level)
    {
        lockImages[level - 1].SetActive(false);
        weaponImages[level - 1].SetActive(true);
        labelImages[level - 1].SetActive(true);
    }

    private void HandleWeaponSelected(int level)
    {
        foreach (Image img in images)
        {
            img.sprite = slotSprite;
        }
        images[level].sprite = slotSelectedSprite;
    }
    

}
