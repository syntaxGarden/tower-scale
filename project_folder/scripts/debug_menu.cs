using Godot;
using System;

public partial class debug_menu : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_button_pressed(String name) 
	{
		if (name == "cs_test") {
			GD.Print("Should load CS_test.tscn");
			Error result = GetTree().ChangeSceneToFile("res://scenes/CS_test.tscn");
			if (result is not Error.Ok) return; else { GD.Print("Couldn't load CS_test.tscn"); }
			//Choose to return manually if the scene can be loaded because ChangeSceneToFile doesn't do that.
		} 
		
		else if (name == "multiple_keys") {
			GD.Print("Should load mutiple_keys.tscn");
			Error result = GetTree().ChangeSceneToFile("res://scenes/multiple_keys.tscn");
			if (result is not Error.Ok) return; else { GD.Print("Couldn't load multiple_keys.tscn"); }
		} 
		
		else {
			GD.Print("Have not got an if statement clause for the string " + name);
		}
	}
}
