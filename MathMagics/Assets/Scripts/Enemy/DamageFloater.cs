using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageFloater : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text floaterText;

    [Header("Settings")]
    public float floatSpeed;
    public float fadeTime;

    private void Start()
    {
        StartCoroutine(FloatAndFadeCoroutine());
    }

    public void Initialize(string expression)
    {
        floaterText.text = expression;
    }

    private IEnumerator FloatAndFadeCoroutine()
    {
        float elapsed = 0f;
        while(elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = 1 - (elapsed / fadeTime); // % of how far we are through the fade
            floaterText.color = new Color(1, 1, 1, t);
            floaterText.transform.position += floatSpeed * Time.deltaTime * Vector3.up;
            yield return null;
        }    

        Destroy(gameObject);
    }

}
