/// <summary>
/// WeaponHandler.cs
/// 
/// This script manages the player's current weapon, switching between available weapons,
/// tracking weapon level, and performing attacks.
/// 
/// Features:
/// - Listens for input to switch weapons (1–4 keys)
/// - Tracks weapon levels and operations (e.g., "+", "-", "*", "/")
/// - Spawns a weapon hitbox in the direction of the player's last movement when attacking
/// - Automatically destroys the hitbox after a short duration
/// 
/// Requires:
/// - A hitbox prefab (e.g., with a 2D collider and damage logic)
/// - A child transform to use as the spawn origin for hitboxes
/// - PlayerInput.cs with a static lastDirection and OnAttackInput event
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [SerializeField] private GameObject weaponHitboxPrefab;
    [SerializeField] private Transform hitboxSpawnPoint;
    [SerializeField] private float hitboxLifetime = 0.2f;

    public static WeaponHandler Instance;
    private Weapon w1 = new Weapon("Subtract", 1, "-");
    private Weapon w2 = new Weapon("Add", 1, "+");
    private Weapon w3 = new Weapon("Multiply", 2, "*");
    private Weapon w4 = new Weapon("Divide", 2, "/");
    private Weapon[] weapons;
    private Weapon currentWeapon;
    public event Action<int> weaponSelected;
    public event Action<Weapon> HandleWeaponLevelChanged;

    private void OnEnable()
    {
        PlayerInput.OnAttackInput += PerformAttack;
        GameManager.beatLevel += HandleBeatLevel;
    }

    private void OnDisable()
    {
        PlayerInput.OnAttackInput -= PerformAttack;
        GameManager.beatLevel -= HandleBeatLevel;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        weapons = new Weapon[] { w1, w2, w3, w4 };
        currentWeapon = weapons[0];
        weapons[0].UnlockWeapon();

        Debug.Log($"[WeaponHandler] Starting weapon: {currentWeapon.getName()}");

        foreach(Weapon w in weapons)
        {
            w.OnWeaponLevelChanged += HandleWeaponLevelChangedWrapper;
        }
    }

    private void HandleWeaponLevelChangedWrapper(Weapon w)
    {
        HandleWeaponLevelChanged?.Invoke(w);

    }


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchWeapon(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SwitchWeapon(3);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentWeapon.DecreaseLevel();
            HandleWeaponLevelChanged?.Invoke(currentWeapon);

        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentWeapon.IncreaseLevel();
            HandleWeaponLevelChanged?.Invoke(currentWeapon);
        } 
    }

    private void SwitchWeapon(int index)
    {
        if (index >= 0 && index < weapons.Length)
        {
            //if we haven't unlocked that weapon yet, return.
            if (weapons[index].getIsLocked()) return;
            //if (weapons[index].getIsLocked()) return;
            currentWeapon = weapons[index];
            Debug.Log($"[WeaponHandler] Switched to: {currentWeapon.getName()}");
            weaponSelected?.Invoke(index);
        }
    }

    private void PerformAttack()
    {
        string damageExpr = currentWeapon.GetDamageExpression();
        Debug.Log($"[WeaponHandler] Attacking with: {damageExpr}");

        Vector3 spawnDirection = new Vector3(PlayerInput.lastDirection.x, PlayerInput.lastDirection.y, 0f);
        Vector3 spawnPosition = hitboxSpawnPoint.position + spawnDirection;

        GameObject hitbox = Instantiate(weaponHitboxPrefab, spawnPosition, Quaternion.identity);

        Destroy(hitbox, hitboxLifetime);
        TurnManager.Instance.EndPlayerTurn();
    }

    public Weapon GetCurrentWeapon() => currentWeapon;

    public Weapon GetWeaponByName(string weaponName)
    {
        foreach (Weapon weapon in weapons)
        {
            if (weapon.getName() == weaponName)
            {
                return weapon;
            }
        }
        Debug.LogWarning($"[WeaponHandler] Weapon with name '{weaponName}' not found.");
        return null;
    }

    public void HandleBeatLevel(int level)
    {
        weapons[level - 1].UnlockWeapon();
    }
}
