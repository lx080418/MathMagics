using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHitbox2D : MonoBehaviour
{

    [SerializeField] private Animator _anim;


    public void PlayAnimation(Weapon w)
    {
        if(WeaponHandler.Instance != null)
        {
            int index = WeaponHandler.Instance.GetWeaponIndexByName(w.getName());
            switch(index)
            {
                case 0:
                    _anim.SetTrigger("Subtraction");

                    break;
                
                case 1:
                    _anim.SetTrigger("Addition");
                    
                    break;
                
                case 2:
                    _anim.SetTrigger("Multiplication");
                    
                    break;
                
                case 3:
                    _anim.SetTrigger("Division");
                    
                    break;
                default:
                    _anim.SetTrigger("Addition");
                    break;

            }
        }
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            Debug.Log("Hit an enemy!");
            EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null && WeaponHandler.Instance != null)
            {
                String damageExpression = WeaponHandler.Instance.GetCurrentWeapon().GetDamageExpression();
                Debug.Log($"[Hitbox2D] Hit enemy with: {damageExpression}");
                enemyHealth.ApplyDamageExpression(damageExpression);
            }
        }
    }
}
    