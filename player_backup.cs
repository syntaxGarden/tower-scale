using Godot;
using System;

public partial class player : CharacterBody3D
{
	const double FRICTION = 10.0;
	const double SPEED = 10.0;
	const double ACCEL = 20.0;
	const double JUMP_VELOCITY = 30.0;
	//const double ROTATE_SPEED = 12.0;
	const double GRAVITY = 30.0;
	const double GRAPPLE_SPEED = 70.0;

	const double mouse_sense = 0.1; //Mouse sensitivity
	private bool paused = false;

	enum GrappleEnum {None, Grappling, Falling};
	private GrappleEnum grappling = GrappleEnum.None;
	private Vector3 move_dir = Vector3.Zero;
	private void RunCodeStore() {
		//The following code is only used if the game has a run button
		/*
		double accel_now = ACCEL;
		double speed_now = SPEED;
		Camera3D camera_node = (Camera3D)GetNode("head/Camera3D");
		AnimationPlayer anim_node = (AnimationPlayer)GetNode("head/Camera3D/FOV_anim");
		if (Input.IsActionPressed("run") && move_dir != Vector3.Zero) {
			//If shift is pressed and moving, play animation
			if (camera_node.Fov == 90) { //Will need to change this when an FOV slider is added
				anim_node.Play("FOV");
			}
			accel_now *= 1.7; speed_now *= 1.7;
		} else if (camera_node.Fov != 90) {
			anim_node.PlayBackwards("FOV");
		}
		vel.X = Mathf.Lerp(vel.X, move_dir.X * (float)speed_now, (float)(accel_now * delta));
		vel.Z = Mathf.Lerp(vel.Z, move_dir.Z * (float)speed_now, (float)(accel_now * delta));
		*/

	}
    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured; //Will trap the mouse cursor inside the game window

		/* These lines are only needed in Tower Scale if there is going to be teleportation between scenes. 

		var ready_info = $"/root/Global".teleport_info
		if ready_info != [Vector3.ZERO,Vector3.ZERO,Vector3.ZERO]:
			position += ready_info[0]
			velocity = ready_info[1]
			rotation.y = ready_info[2].y
			$head.rotation.x = ready_info[2].x
		*/
    }
    public override void _Input(InputEvent @event)
    {
        if (!paused) {
			InputEventMouseMotion motion = @event as InputEventMouseMotion;
			if (motion != null) {
				RotateY((float)Mathf.DegToRad(-motion.Relative.X * mouse_sense));
				Node3D head = (Node3D)GetNode("head");
				head.RotateX((float)Mathf.DegToRad(-motion.Relative.Y * mouse_sense));
				head.Rotation = new Vector3((float)Mathf.Clamp(head.Rotation.X, Mathf.DegToRad(-89.0), Mathf.DegToRad(89.0)), head.Rotation.Y, head.Rotation.Z);

				//look code adapted from YoutTube video called "How To Code An FPS Controller In Godot 3" by Nagi (https://youtu.be/jf_Hz0diI8Y)
			} 
		}
    }
    private Vector3 velocity_calculation(double delta) {
		//If grappling, then continue the grapple movement
			//Else check for grapple attempts
				//If grappling, set velocity and grapple condition
				//Else continue to other steps

		if (grappling == GrappleEnum.Grappling) { /*return new Vector3();*/ }

		Vector3 vel = Velocity;
		if (IsOnFloor() && Input.IsActionJustPressed("jump")) {
			vel.Y = (float)JUMP_VELOCITY;
		} else { 
			vel.Y -= (float)(GRAVITY * delta);
		}

		move_dir = new Vector3(	
			Input.GetAxis("a","d"), 
			0, 
			Input.GetAxis("w","s") 
		).Normalized().Rotated(Vector3.Up, Rotation.Y); 

		vel.X = Mathf.Lerp(vel.X, move_dir.X * (float)SPEED, (float)(ACCEL * delta));
		vel.Z = Mathf.Lerp(vel.Z, move_dir.Z * (float)SPEED, (float)(ACCEL * delta));

		return vel;
	}
	public override void _PhysicsProcess(double delta)
    {
		HUD HUD = (HUD)GetNode("CanvasLayer/HUD");

		//Pause code
		if (Input.IsActionJustPressed("pause")) {
			HUD.TogglePause();
			paused = !paused;
			if (paused) { 
				Input.MouseMode = Input.MouseModeEnum.Visible; //Releases the mouse cursor
			} else {
				Input.MouseMode = Input.MouseModeEnum.Captured; //Will trap the mouse cursor inside the game window
			}
			
		}

		//Fullscreen code
		if (Input.IsActionJustPressed("toggle_fs")) {
			if (GetWindow().Mode == Window.ModeEnum.ExclusiveFullscreen) {
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
			} else {
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
			}
		}

		RayCast3D GrappleRay = (RayCast3D)GetNode("head/Camera3D/GrappleRay");
		HUD.SetGrappleRayFeedback(GrappleRay);

		if (!paused) {
			Vector3 vel = velocity_calculation(delta);
			
			Set("velocity", vel);
			Set("up_direction", Vector3.Up);
			MoveAndSlide();
		}
    }
}
