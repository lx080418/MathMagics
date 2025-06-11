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
    private Weapon w3 = new Weapon("Multiply", 1, "*");
    private Weapon w4 = new Weapon("Divide", 1, "/");
    private Weapon[] weapons;
    private Weapon currentWeapon;
    private void OnEnable()
    {
        PlayerInput.OnAttackInput += PerformAttack;
    }

    private void OnDisable()
    {
        PlayerInput.OnAttackInput -= PerformAttack;
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

        Debug.Log($"[WeaponHandler] Starting weapon: {currentWeapon.getName()}");
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchWeapon(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SwitchWeapon(3);

        if (Input.GetKeyDown(KeyCode.Q)) currentWeapon.DecreaseLevel();
        if (Input.GetKeyDown(KeyCode.E)) currentWeapon.IncreaseLevel();
    }

    private void SwitchWeapon(int index)
    {
        if (index >= 0 && index < weapons.Length)
        {
            currentWeapon = weapons[index];
            Debug.Log($"[WeaponHandler] Switched to: {currentWeapon.getName()}");
        }
    }

    private void PerformAttack()
    {
        string damageExpr = currentWeapon.GetDamageExpression();
        Debug.Log($"[WeaponHandler] Attacking with: {damageExpr}");

        GameObject hitbox = Instantiate(weaponHitboxPrefab, hitboxSpawnPoint.position, Quaternion.identity);
        WeaponHitbox2D hitboxScript = hitbox.GetComponent<WeaponHitbox2D>();
        hitboxScript.damageExpression = damageExpr;

        Destroy(hitbox, hitboxLifetime);
    }

    public Weapon GetCurrentWeapon() => currentWeapon;
}
