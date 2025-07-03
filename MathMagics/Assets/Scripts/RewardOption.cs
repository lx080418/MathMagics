public enum Rarity { Common, Rare, Epic }
public enum RewardType{Weapon,Health}


public class RewardOption
{
    public string description;
    public Rarity rarity;
    public int levelIncrease;
    public string weaponName;
    public RewardType rewardType;

    public RewardOption(string weaponName, Rarity rarity, int levelIncrease)
    {
        this.weaponName = weaponName;
        this.rarity = rarity;
        this.levelIncrease = levelIncrease;
        this.description = $"Upgrade {weaponName}";
    }

    public RewardOption(string description, Rarity rarity, int healthAmount, RewardType type)
    {
        this.description = description;
        this.rarity = rarity;
        this.levelIncrease = healthAmount;
        this.rewardType = type;
    }
}