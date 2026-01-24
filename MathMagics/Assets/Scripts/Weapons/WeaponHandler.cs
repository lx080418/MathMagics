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
using System.Linq;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [SerializeField] private WeaponHitbox2D weaponHitboxPrefab;
    [SerializeField] private Transform hitboxSpawnPoint;
    [SerializeField] private float hitboxLifetime = 0.2f;

    public static WeaponHandler Instance;
    private Weapon w1 = new Weapon("Subtraction", 1, "-");
    private Weapon w2 = new Weapon("Addition", 1, "+");
    private Weapon w3 = new Weapon("Multiplication", 2, "*");
    private Weapon w4 = new Weapon("Division", 2, "/");
    private Weapon[] weapons;
    private Weapon currentWeapon;
    public event Action<int> weaponSelected;
    public event Action<Weapon> HandleWeaponLevelChanged;
    public event Action<int> weaponForceUnlocked;
    private int currentWeaponIndex;
    private float timeSinceLastAttack;
    [SerializeField]private float attackCooldownTime;
    [SerializeField] private List<WeaponLevelUI> weaponLevelUIs;

    [Header("Audio")]
    [SerializeField] private AudioClip[] playerAttackSFX;



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
        
        weapons = new Weapon[] { w1, w2, w3, w4 };
    }

    private void Start()
    {
        
        currentWeapon = weapons[0];
        weapons[0].UnlockWeapon();

        Debug.Log($"[WeaponHandler] Starting weapon: {currentWeapon.getName()}");

        foreach(Weapon w in weapons)
        {
            w.OnWeaponLevelChanged += HandleWeaponLevelChangedWrapper;
        }


        foreach(WeaponLevelUI wlui in weaponLevelUIs)
        {
            wlui.OnWeaponSlotClicked += HandleWeaponSlotClicked;
        }
        
        StartCoroutine(DelayWeaponUnlock());

    }

    


    private void HandleWeaponLevelChangedWrapper(Weapon w)
    {
        HandleWeaponLevelChanged?.Invoke(w);

    }


    public void Update()
    {
        timeSinceLastAttack += Time.deltaTime;
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
            currentWeaponIndex = index;
        }
    }

    private void PerformAttack()
    {
        if(timeSinceLastAttack < attackCooldownTime) return;

        timeSinceLastAttack = 0f;
        string damageExpr = currentWeapon.GetDamageExpression();
        Debug.Log($"[WeaponHandler] Attacking with: {damageExpr}");

        Vector3 spawnDirection = new Vector3(PlayerInput.lastDirection.x, PlayerInput.lastDirection.y, 0f);
        Vector3 spawnPosition = hitboxSpawnPoint.position + spawnDirection;

        AudioManager.Instance.PlayOneShotVariedPitch(playerAttackSFX[currentWeaponIndex], 1f, AudioManager.Instance.sfxAMG, .03f);
        
        WeaponHitbox2D hitbox = Instantiate(weaponHitboxPrefab, spawnPosition, Quaternion.identity);
        hitbox.DoAttack(currentWeapon, .1f);
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

    public int GetWeaponIndexByName(string weaponName)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].getName() == weaponName)
            {
                return i;
            }
        }
        return -1;
    }

    public Weapon[] GetWeapons()
    {
        return weapons;
    }

    public void HandleBeatLevel(int level)
    {
        weapons[level - 1].UnlockWeapon();
    }

    public IEnumerator DelayWeaponUnlock()
    {
        yield return null;
        if(PlayerPrefs.HasKey("level"))
        {
            Debug.Log($"Key: level found! {PlayerPrefs.GetInt("level")}");
            int savedLevel = PlayerPrefs.GetInt("level");
            for(int i = 0; i < savedLevel; i++)
            {
                weapons[i].UnlockWeapon();
                weaponForceUnlocked?.Invoke(i);
            }
        }
        else
        {
            Debug.Log("Key: level not found");
        }
    }

    private void HandleWeaponSlotClicked(int index)
    {
        SwitchWeapon(index);
    }

}
