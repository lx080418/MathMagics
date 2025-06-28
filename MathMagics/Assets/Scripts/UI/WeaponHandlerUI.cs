using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandlerUI : MonoBehaviour
{
    public GameObject[] lockImages;
    public GameObject[] weaponImages;

    private void OnEnable()
    {
        GameManager.beatLevel += HandleBeatLevel;
        foreach (GameObject go in lockImages)
        {
            go.SetActive(true);
        }
        foreach (GameObject go in weaponImages)
        {
            go.SetActive(false);
        }

        lockImages[0].SetActive(false);
        weaponImages[0].SetActive(true);
    }
    private void OnDisable()
    {
        GameManager.beatLevel -= HandleBeatLevel;
    }

    private void HandleBeatLevel(int level)
    {
        lockImages[level - 1].SetActive(false);
        weaponImages[level - 1].SetActive(true);
    }
    

}
