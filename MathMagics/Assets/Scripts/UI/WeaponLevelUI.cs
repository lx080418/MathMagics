using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponLevelUI : MonoBehaviour, IPointerDownHandler
{
    public TMP_Text weaponLevel;
    public int weaponIndex;
    public event Action<int> OnWeaponSlotClicked;
    public void ChangeWeaponLevelText(int num)
    {
        weaponLevel.text = num.ToString();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnWeaponSlotClicked?.Invoke(weaponIndex);
    }
        
    
}
