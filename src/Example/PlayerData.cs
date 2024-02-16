
using System;

[Serializable]
public class PlayerData
{
    public int CurrentLevel;

    public PlayerData(int currentLevel)
    {
        CurrentLevel = currentLevel;
    }
}