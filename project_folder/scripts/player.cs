using Godot;
using System;
using System.Diagnostics.CodeAnalysis;
//using System.Numerics;

public partial class player : CharacterBody3D
{
	const double SPEED = 10.0;
	const double ACCEL = 10.0;
	const double JUMP_VELOCITY = 30.0;
	//const double ROTATE_SPEED = 12.0;
	const double GRAVITY = 30.0;
	const double TERMINAL_VELOCITY = -120.0;
	private Vector3 move_dir = Vector3.Zero;
	const double MOUSE_SEN = 0.1; //Mouse sensitivity
	private bool paused = false;
	private bool damaged = false;

	//~~~~~~Dodge variables~~~~~~
	private Vector3 dodge = Vector3.Zero;
	const double DODGE_SPEED = 45.0;
	const double DODGE_TIME = 0.3;
	private double dodge_end = 0;
    
	//~~~~~~Grapple variables~~~~~~
	enum GrappleEnum {None, Grappling, Falling};
	private GrappleEnum grapple_status = GrappleEnum.None;
	const double GRAPPLE_SPEED = 45.0;
	private Area3D grapple_to = null;
	private Area3D grapple_pass = null;
	private Vector3 grapple_vel = Vector3.Zero;

	//~~~~~~Wall run variables~~~~~~
	enum WallRunEnum {None, Running, Falling};
	private WallRunEnum wall_run_status = WallRunEnum.None;
	const double WALL_RUN_SPEED = 70.0;
	private Area3D running_on = null;
	private PathFollow3D wall_path = null;
	
	private double GetTime() { return (double)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds; }
	public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured; //Will trap the mouse cursor inside the game window
		
		//now = (grapple_to.Position - Position).Normalized();
		//vel = (obj.Position - Position).Normalized();
		/* bool[] bools_now = { now.X > 0, now.Y > 0, now.Z > 0 };
		bool[] bools_vel = { vel.X > 0, vel.Y > 0, vel.Z > 0 };
		for (int i = 0; i < 3; i++) {
			if (now[0] == 0) { continue; }
			else if (bools_now[i] != bools_vel[i]) { return true; }
		}
		return false; */

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
				RotateY((float)Mathf.DegToRad(-motion.Relative.X * MOUSE_SEN));
				Node3D head = (Node3D)GetNode("head");
				head.RotateX((float)Mathf.DegToRad(-motion.Relative.Y * MOUSE_SEN));
				head.Rotation = new Vector3((float)Mathf.Clamp(head.Rotation.X, Mathf.DegToRad(-89.0), Mathf.DegToRad(89.0)), head.Rotation.Y, head.Rotation.Z);

				//look code adapted from YoutTube video called "How To Code An FPS Controller In Godot 3" by Nagi (https://youtu.be/jf_Hz0diI8Y)
			} 
		}
    }
    private void damage_process() { /*This is where the code to process the damage will go*/ }
	private bool passed_grapple_point() {
		Area3D player_area = (Area3D)GetNode("body_area"); 
		if (player_area.GetOverlappingAreas().Contains(grapple_pass)) { return true; } //If at the grapple point, stop grappling

		//Use basic algebra:
		//G = grapple_to.Position, P = Position, V = grapple_vel, C = what you're looking for
		//G = P + V*C
		//(G - P)/V = C
		//If C is negative then the player has overshot the grapple point
		Vector3 C = Vector3.Zero;
		C.X = (grapple_to.Position.X - Position.X) / grapple_vel.X;
		C.Y = (grapple_to.Position.Y - Position.Y) / grapple_vel.Y;
		C.Z = (grapple_to.Position.Z - Position.Z) / grapple_vel.Z;
		
		return (C.X < 0 && C.Y < 0 && C.Z < 0);
	}
	private Vector3 velocity_calc(double delta, RayCast3D grapple_ray) {
		Vector3 vel = Velocity;

		//If GrapplingEnum.None make it so that not pressing any keys slows down the player's X and Z
		//After this, keep the first movement code

		//GrapplingEnum.None only happens once the player has touched the normal ground [MAYBE ADD SLIPPY FLOORS LATER]
		if (grapple_status == GrappleEnum.Grappling) {				
			if (Input.IsActionJustPressed("grapple") || Input.IsActionJustPressed("dodge") || passed_grapple_point()) {
				//Cancel grapple and start falling if grapple pressed, dodge pressed, or you've passed the dodge point
				grapple_status = GrappleEnum.Falling;
				grapple_to = null;
				grapple_pass = null;
				grapple_vel = Vector3.Zero;
			} else {
				return grapple_vel;
			}
		} 

		if (dodge != Vector3.Zero) { 
			//The dodge code is here so that nothing can be done while dodging
			//If this proves to get twisted with the wall running and the slippy floors then it will need changing
			if (GetTime() > dodge_end) { //If dodge is over
				dodge = Vector3.Zero;
				dodge_end = 0.0;
			} else { //Keep dodging
				return dodge;
			}
		}

		if (wall_run_status == WallRunEnum.Running) {
			//If running on a wall, do code
			//Design later when run able walls have been made. 
			//When wall running is implemented then it's paramount that dodging while wall running is implemented 
			//This will be difficult and may require more variables
		}

		//Here goes the normal controls section
		if (Input.IsActionJustPressed("grapple")) {
			//Can go here even while currently grappling, which means potentially chaining grapples together
			if (grapple_ray.IsColliding()) {
				Area3D obj = (Area3D)grapple_ray.GetCollider();
				if (obj.Name.ToString().Substr(0,7) == "grapple") {
					grapple_status = GrappleEnum.Grappling;
					grapple_to = obj;
					grapple_pass = (Area3D)obj.GetNode("physical_point");
					grapple_vel = (obj.Position - Position).Normalized() * (float)GRAPPLE_SPEED;
					return grapple_vel;
				}
			}
		} 
		
		if (Input.IsActionJustPressed("dodge")) {
			//The following 2 lines make it so that the momentum is lost once you dodge
			//However it may be a good idea to get rid of these lines because you can throw yourself through the air with the dodge, which could be used for some challenges.
			grapple_status = GrappleEnum.None;
			wall_run_status = WallRunEnum.None;
			
			move_dir = new Vector3(	
				Input.GetAxis("left","right"), 
				0, 
				Input.GetAxis("forward","backward") 
			).Normalized().Rotated(Vector3.Up, Rotation.Y); 

			if (move_dir == Vector3.Zero) { 
				//If no keys being pressed, then dodge in camera direction
				move_dir = new Vector3(0,0,-1).Rotated(Vector3.Up, Rotation.Y); 
			} 
			dodge = move_dir * (float)DODGE_SPEED;
			dodge_end = GetTime() + DODGE_TIME;

			return dodge;
		}

		//Normal walking/falling code
		if (IsOnFloor()) { //If on normal floor 
			grapple_status = GrappleEnum.None;
			wall_run_status = WallRunEnum.None;
			if (Input.IsActionJustPressed("jump")) {
				vel.Y = (float)(JUMP_VELOCITY + GRAVITY * delta);
			}
		}
		vel.Y -= (float)(GRAVITY * delta);
		vel.Y = Mathf.Clamp(vel.Y, (float)TERMINAL_VELOCITY, float.MaxValue);

		move_dir = new Vector3(	
			Input.GetAxis("left","right"), 
			0, 
			Input.GetAxis("forward","backward") 
		).Normalized().Rotated(Vector3.Up, Rotation.Y); 

		if (grapple_status == GrappleEnum.Falling || wall_run_status == WallRunEnum.Falling) {
			/*
			This also applies to the momentum of coming off a wall run
			Have gravity affecting the momentum
			For the X and Z movement, add the key movement to the velocity and reduce it so that its length is less than the highest speed reached. 
			POTENTIALLY, if the player's X and Z velocities reach a low enough level then they will lose all of the momentum and will be sent to GrapplingEnum.None and WallRunEnum.Falling

			This part of the code will require a much more thorough look at Ultrakill.
			
			For the sake of testing the first part of the grappling code, this will end and go to the normal walking and falling code. So delete the following code afterwards
			*/

			//[THIS NEEDS THE UTMOST ATTENTION]
			if (move_dir != Vector3.Zero) {
				//Old code that crushed the velocity
				/* vel.X = Mathf.Lerp(vel.X, move_dir.X * (float)SPEED, (float)(ACCEL * delta));
				vel.Z = Mathf.Lerp(vel.Z, move_dir.Z * (float)SPEED, (float)(ACCEL * delta)); */

				//Newer code that is weird and may not work (attempts to preserve gravity)
				/* float before_length = Mathf.Sqrt((vel.X * vel.X) + (vel.Z * vel.Z));
				vel -= move_dir * (float)delta;
				float after_length = Mathf.Sqrt((vel.X * vel.X) + (vel.Z * vel.Z));
				if (before_length < after_length) { vel = vel.Normalized() * before_length; } */

				//Basic code that may yield wierd results in the long run
				float y = vel.Y;
				vel.Y = 0;
				float l = vel.Length(); //It may be a good idea later to save this value so that you can return to essentially float fast, IF (VERY BIG IF) you can't affect your movement very much when falling.
				vel += move_dir * (float)(4 * SPEED * delta); //Multiplied because SPEED * delta was way too slow
				if (vel.Length() > l) { vel = vel.Normalized() * l; }
				if (vel.Length() <= SPEED) {
					grapple_status = GrappleEnum.None;
					wall_run_status = WallRunEnum.None;
				}
				vel.Y = y;
				//Field test result: You do not gain a speed boost from pushing forwards, the left or right movement could be strung into challenged, and pushing backwards slows you down properly
			}
		} else {
			//The lerp statements are fine for now but may spell trouble later
			vel.X = Mathf.Lerp(vel.X, move_dir.X * (float)SPEED, (float)(ACCEL * delta));
			vel.Z = Mathf.Lerp(vel.Z, move_dir.Z * (float)SPEED, (float)(ACCEL * delta));
		}
		
		return vel;
	}
	public override void _PhysicsProcess(double delta)
    {
		HUD HUD = (HUD)GetNode("CanvasLayer/HUD");
		RayCast3D grapple_ray = (RayCast3D)GetNode("head/Camera3D/GrappleRay");
		HUD.SetGrappleRayFeedback(grapple_ray);

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
		
		if (!paused) {
			Vector3 vel = velocity_calc(delta, grapple_ray);
			//GD.Print(Velocity);
			
			Set("velocity", vel);
			Set("up_direction", Vector3.Up);
			MoveAndSlide();
		}
    }
}
