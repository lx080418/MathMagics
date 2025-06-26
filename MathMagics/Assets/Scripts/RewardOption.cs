public enum Rarity { Common, Rare, Epic }

public class RewardOption
{
    public string description;
    public Rarity rarity;
    public int levelIncrease;
    public string weaponName;

    public RewardOption(string weaponName, Rarity rarity, int levelIncrease)
    {
        this.weaponName = weaponName;
        this.rarity = rarity;
        this.levelIncrease = levelIncrease;
        this.description = $"Upgrade {weaponName}";
    }
}