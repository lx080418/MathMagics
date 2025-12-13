using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PlayerMagicStone : MonoBehaviour
{

    //* --------- Magic Stones ------- */
    private int _magicStones;

    //* ---------- Events ----------- */
    public event Action<int> onMagicStonesChanged;

    private void Awake()
    {
        //TODO load the amount of magic stones from playerprefs
        if(PlayerPrefs.HasKey("magicStones"))
        {
            GainMagicStone(PlayerPrefs.GetInt("magicStones"));
        }
        else
        {
            _magicStones = 0;
        }
    }

    private void Start()
    {
        onMagicStonesChanged?.Invoke(_magicStones);
    }


    public void GainMagicStone(int amount)
    {
        _magicStones += amount;
        onMagicStonesChanged?.Invoke(_magicStones);
    }

    public int GetMagicStones()
    {
        return _magicStones;
    }
}
