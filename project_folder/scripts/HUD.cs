using Godot;
using System;

public partial class HUD : Node2D
{
	/*A series of variables are declared that define what things are shown on the 
	*/

	private bool fps; //Decides if the FPS counter should contain text
	public override void _Ready()
	{
		fps = true; //Edit this when you learn how to import from a global file
		Node2D pause_node = (Node2D)GetNode("pause");
		pause_node.Set("visible",false);
	}
	public void ToggleFPS() { fps = !fps; }
	public void SetGrey(bool slow_active, double slow_meter, double SLOW_DURATION) {
		ColorRect grey = (ColorRect)GetNode("greyscale");
		ColorRect bg = (ColorRect)GetNode("slowdown/bg");
		grey.Visible = slow_active;
		bg.Scale = new Vector2(bg.Scale.X, (float)(slow_meter / SLOW_DURATION));
	}
	public void TogglePause() { 
		Node2D pause_node = (Node2D)GetNode("pause");
		pause_node.Set("visible",!(bool)pause_node.Get("visible"));
	}
	public void SetGrappleRayFeedback(RayCast3D ray) {
		Label node = (Label)GetNode("grapple_view");
		Area3D obj = ray.GetCollider() as Area3D;
		node.Text = "";
		if (obj == null) { 
			node.Text += "null"; 
		} else { 
			node.Text +=  obj.ToString() + "\n" + obj.Name; 
			if (obj.Name.ToString().Substr(0,7) == "grapple") {
				node.Text += "\n" + "This is indeed a grapple point";
				node.Text += "\n" + ray.GetCollisionNormal();
			}
		}
	}
	public override void _Process(double delta)
	{
		if (fps) { 
			Label fps_node = (Label)GetNode("FPS"); 
			fps_node.Text = "FPS:   " + Math.Round(1/delta) + "\nDelta: " + Math.Round(delta, 4); 
		}
	}
}
