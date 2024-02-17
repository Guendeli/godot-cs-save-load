# Simple Save Load System - Godot 4.2 C#

### Dependencies
Newtonsoft.Json is already declared in the csproj

### Usage

```CS

[Serializable]
public PlayerData
{
  public int SomeData;
}

public partial class Bootstrap : Node3D
{
	private SaveLoadSystem _saveManager;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_saveManager = new SaveLoadSystem();

		PlayerData playerData = _saveManager.GetData<PlayerData>();
		if (playerData == null)
		{
			GD.Print("Save Data not found, creating new save");
			playerData = new PlayerData();
			_saveManager.SaveData(playerData);
		}
		else
		{
			GD.Print("Successfully loaded save file");
		}
	}
}


```

