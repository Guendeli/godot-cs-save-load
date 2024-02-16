using Godot;
using System;
using SimpleSaveLoadSystem;
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
			playerData = new PlayerData(0);
			_saveManager.SaveData(playerData);
		}
		else
		{
			GD.Print("Successfully loaded save file");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
