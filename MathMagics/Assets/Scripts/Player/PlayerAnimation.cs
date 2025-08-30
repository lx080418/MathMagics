using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Transform _srTransform;
    [SerializeField] private SpriteRenderer _weaponSr;
    [SerializeField] private List<Sprite> weapons;
    private void Start()
    {
        PlayerInput.OnMoveInput += HandleFlipSprite;
        WeaponHandler.Instance.weaponSelected += ChangeWeaponSprite;
    }

    private void OnDisable()
    {
        PlayerInput.OnMoveInput -= HandleFlipSprite;
        WeaponHandler.Instance.weaponSelected -= ChangeWeaponSprite;
    }

    private void ChangeWeaponSprite(int index)
    {   
        Debug.Log("Changing weapon image!");
        _weaponSr.sprite = weapons[index];
    }

    private void HandleFlipSprite(Vector2 vector)
    {
        if (vector.x > 0)
        {
            _srTransform.localScale = new Vector3(1, 1, 1);
            _weaponSr.sortingOrder = 2;
        }
        else if (vector.x < 0)
        {
            _srTransform.localScale = new Vector3(-1, 1, 1);
            _weaponSr.sortingOrder = 1;
        }
    }
}
