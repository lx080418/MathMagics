public enum Rarity { Common, Rare, Epic }

public class RewardOption
{
    public string description;
    public Rarity rarity;
    public int levelIncrease;

    public RewardOption(string desc, Rarity rarity, int levelIncrease)
    {
        this.description = desc;
        this.rarity = rarity;
        this.levelIncrease = levelIncrease;
    }
}