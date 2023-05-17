public class PlayerDataInventory
{
    public const string InventoryType = "PLAYER_DATA";
    public string Type = InventoryType;
    public string PlayerId = "";
    public string PlayerName = "";
    public int Experience = 0;
    public int Trophies = 0;
    public int Coins = 0;
    public int Gems = 0;
    public int[] Skins = new int[0];
    public bool IsValid()
    {
        return Type == InventoryType;
    }
}