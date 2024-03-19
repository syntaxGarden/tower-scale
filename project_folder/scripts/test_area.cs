using Godot;
using System;

public partial class test_area : Node3D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Input(InputEvent @event)
    {
        InputEventKey ev = @event as InputEventKey;
		if (ev != null && ev.Keycode == Key.Backspace) GetTree().Quit();
    }
}
