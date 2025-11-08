using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip portalSFX;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("PlayerHitbox"))
        {
            //Load the next level.

            AudioManager.Instance.PlayOneShot(portalSFX, 1f, AudioManager.Instance.sfxAMG);
            StartCoroutine(AudioManager.Instance.StopBackgroundMusic(1f));
            GameManager.instance.LevelCompleted();
        }
    }
}
