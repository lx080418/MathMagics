using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MagicStoneUI : MonoBehaviour
{
    //* -------- Player Magic Stones
    public PlayerMagicStone playerMagicStone;

    [Header("UI References")]
    [SerializeField]private TMP_Text magicStoneAmountText;

    private void Awake()
    {
        playerMagicStone.onMagicStonesChanged += HandleMagicStonesChanged;
    }

    private void Oestroy()
    {
        if(playerMagicStone != null)
        playerMagicStone.onMagicStonesChanged -= HandleMagicStonesChanged;

    }

    private void HandleMagicStonesChanged(int amount)
    {
        magicStoneAmountText.text = amount.ToString();
    }
}
