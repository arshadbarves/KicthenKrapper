
public enum GameMode
{
    None,
    Tutorial,
    SinglePlayer,
    MultiPlayer
}


public enum InputType
{
    Keyboard,
    Gamepad,
    Touch,
    VR,
    None
}

public enum ShopItemCostType
{
    Free,
    Coins,
    Gems
}

public enum ShopItemType
{
    Skin,
    Resource
}

public enum ShopSection
{
    Offers,
    DailyFreebies,
    Skins,
    Resources
}

public enum ResourceType
{
    Coin,
    Gem
}

public enum SceneType
{
    Startup,
    Tutorial,
    MainMenu,
    MultiplayerMenu,
    Lobby,
    Map_City_001,
}

public enum MapType
{
    Desert,
    Snow,
    Space,
    Jungle,
    Ship,
    Forest,
    City,
    Beach,
    Farm,
    Castle,
    Island,
    Dungeon
}

public enum GameStatus
{
    None,
    GameOver,
    GameWon,
    GameStarted,
    GameRestarted,
    GameQuit,
    GameMainMenu
}

public enum GameState
{
    WaitingToStart,
    CountdownToStart,
    Playing,
    GameOver
}