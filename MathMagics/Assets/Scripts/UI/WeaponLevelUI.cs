using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class WeaponLevelUI : MonoBehaviour
{
    public TMP_Text weaponLevel;
    public void ChangeWeaponLevelText(int num)
    {
        weaponLevel.text = num.ToString();
    }
}
