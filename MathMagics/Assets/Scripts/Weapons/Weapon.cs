using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon
{
    private string name;
    private int level;
    private int minLevel;
    private int maxLevel;
    private string operation;
    private bool islocked;

    public Weapon(string name, int level, string operation, bool isLocked = true)
    {
        this.name = name;
        this.level = level;
        this.minLevel = level;
        this.maxLevel = level;
        this.operation = operation;
        this.islocked = isLocked;
    }

    public string getName() => name;
    public int getLevel() => level;
    public int getMinLevel() => minLevel;
    public int getMaxLevel() => maxLevel;
    public string getOperation() => operation;
    public bool getIsLocked() => islocked;

    public string GetDamageExpression()
    {
        return operation + level.ToString();
    }

    public void IncreaseLevel()
    {
        if (level == maxLevel)
        {
            level = minLevel;
        }
        else
        {
            level++;
        }
    }

    public void IncreaseLevelBy(int amount)
    {
        maxLevel += amount;
    }

    public void DecreaseLevel()
    {
        if (level == minLevel)
        {
            level = maxLevel;
        }
        else
        {
            level--;
        }
    }

    public void UnlockNextLevel()
    {
        maxLevel++;
    }

    public void UnlockWeapon()
    {
        this.islocked = false;
    }
}
