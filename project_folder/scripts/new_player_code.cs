using Godot;
using System;

public partial class new_player : CharacterBody3D
{
	const double FRICTION = 20.0;
	const double SPEED = 10.0;
	const double ACCEL = 20.0;
	const double JUMP_VELOCITY = 30.0;
	const double GRAVITY = 30.0;
    const double TERMINAL_VELOCITY = 120.0;
	const double MOUSE_SEN = 0.1; //Mouse sensitivity
	private bool paused = false; //Pause no longer pauses movement (like in Dark Souls). This is to make the game less forgiving. NO BREAKS FOR YOU
    private Vector3 move_dir = Vector3.Zero;

	//~~~~~~Dodge variables~~~~~~
	private Vector3 dodge = Vector3.Zero;
	const double DODGE_SPEED = 70.0;
	const double DODGE_TIME = 0.5;
	private double dodge_end = 0;
    
	//~~~~~~Grapple variables~~~~~~
	enum GrappleEnum {None, Grappling, Falling};
	private GrappleEnum grapple_status = GrappleEnum.None;
	const double GRAPPLE_SPEED = 70.0;
	private Vector3 grapple_to = Vector3.Zero;
	private Vector3 grapple_vel = Vector3.Zero;
	
	//~~~~~~Wall run variables~~~~~~
	enum WallRunEnum {None, Running, Falling};
	private WallRunEnum wall_run_status = WallRunEnum.None;
	const double WALL_RUN_SPEED = 70.0;
	private Area3D running_on = null;
	private PathFollow3D wall_path = null;

	private double GetTime() { return (double)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds; }
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
		Global global = (Global)GetNode("/root/Global");
		GD.Print(global.ScriptID);
    }
    public override void _Input(InputEvent @event)
    {
        if (!paused || dodge == Vector3.Zero) {
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
	private Vector3 old_velocity_code_DO_NOT_USE(double delta, RayCast3D ray) {
		//if (damaged) {
			//If the player has been damaged, process the knockback velocity
			//This also requires 
		//}

		/*
		If grappling,
			If grapple button pressed again or passed grapple point, cancel grapple
			Else keep flying towards grapple point
		Else check for grapple attempts
			If point found, set velocity and grapple condition
			Else continue to other steps
		*/
		
		if (grapple_status == GrappleEnum.Grappling) { 
			if (Input.IsActionJustPressed("grapple")) { //If grapple button pressed while grappling, cancel grapple
				grapple_status = GrappleEnum.Falling; 
				grapple_to = Vector3.Zero;
				//grapple_vel = Vector3.Zero; //[NEED TO THINK ABOUT THIS LINE BEFORE USING IT]
			} else if (Input.IsActionJustPressed("dodge")) {
				grapple_status = GrappleEnum.Falling;
				grapple_to = Vector3.Zero;
				//Do the normal dodge code
			} else { //If still grappling but not interupted. 

				//Check if you'll pass through the grapple point on the next move_and_collide [APPLY DELTA]
				//If yes, change to falling but return grapple_vel to continue past the grapple point
				//If no, just return grapple_vel 
				Vector3 next = Position += grapple_vel * (float)delta; 

				//This is the most horrendous boolean logic and NEEDS to be fixed
				bool[] b = {false, false, false};
				for (int i = 0; i < 3; i++) { b[i] = (grapple_to[i] <= Position[i] && grapple_to[i] >= next[i]) || (grapple_to[i] >= Position[i] && grapple_to[i] <= next[i]); }
				if (!(b[0] || b[1] || b[2])) { 
					grapple_status = GrappleEnum.Falling; 
					grapple_to = Vector3.Zero;
					//grapple_vel = Vector3.Zero; //[NEED TO THINK ABOUT THIS LINE BEFORE USING IT]
				} 
				return grapple_vel;			
			}
		} else { //If not grappling
			if (Input.IsActionJustPressed("grapple")) { //If attempting to grapple
				//Check for point in RayCast
				//If grapple found, change grappling variable, set grapple vel start moving
				if (ray.IsColliding()) { //If Area3D found
					Area3D obj = ray.GetCollider() as Area3D;
					if (obj.Name.ToString().Substr(0,7) == "grapple") { //If it's a grapple point, start grappling
						grapple_status = GrappleEnum.Grappling;
						grapple_to = obj.Position;
						grapple_vel = (Position - obj.Position).Normalized() * (float)GRAPPLE_SPEED;
						return grapple_vel;
					}
				}
			}
		}	

		if (dodge != Vector3.Zero) { //If currently dodging
			if (GetTime() < dodge_end) {
				return dodge;
			} else {
				
			}
		}

		Vector3 vel = Velocity;
		if (IsOnFloor()) {
			grapple_status = GrappleEnum.None;
			if (Input.IsActionJustPressed("jump")) {
				vel.Y =  (float)JUMP_VELOCITY;
			} else {
				vel.Y -= (float)(GRAVITY * delta);
				vel.Y =  (float)Mathf.Clamp(vel.Y, TERMINAL_VELOCITY, double.MaxValue);
			}
		} else {
			vel.Y -= (float)(GRAVITY * delta);
			vel.Y =  (float)Mathf.Clamp(vel.Y, TERMINAL_VELOCITY, double.MaxValue);
		} //Should figure out way to refactor this so it's more legible

		move_dir = new Vector3(	
			Input.GetAxis("a","d"), 
			0, 
			Input.GetAxis("w","s") 
		).Normalized().Rotated(Vector3.Up, Rotation.Y); 

		vel.X = Mathf.Lerp(vel.X, move_dir.X * (float)SPEED, (float)(ACCEL * delta));
		vel.Z = Mathf.Lerp(vel.Z, move_dir.Z * (float)SPEED, (float)(ACCEL * delta));

		return vel;
	}
    private Vector3 original_velocity_calculation(double delta) {

		if (grapple_status == GrappleEnum.Grappling) { /*return new Vector3();*/ }

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
	private bool passing_grapple_point(double delta) {
		return false;
	}
	private Vector3 new_velocity_calc(double delta) {
		Vector3 vel = Velocity;

		//If GrapplingEnum.None make it so that not pressing any keys slows down the player's X and Z
		//After this, keep the first movement code

		//GrapplingEnum.None only happens once the player has touched the normal ground [MAYBE ADD SLIPPY FLOORS LATER]
		if (grapple_status == GrappleEnum.Grappling) {				
			if (Input.IsActionJustPressed("grapple")) {
				//Cancel grapple and go to the falling code
				grapple_status = GrappleEnum.Falling;
				grapple_to = Vector3.Zero;
			} else if (Input.IsActionJustPressed("dodge")) {
				//Cancel grapple and start the dodging movement
				grapple_status = GrappleEnum.Falling;
				grapple_to = Vector3.Zero;
			} else if (passing_grapple_point(delta)) {
				//Cancel grapple and start falling, but return grapple_vel for a frame
				grapple_status = GrappleEnum.Falling;
				grapple_to = Vector3.Zero;
				return grapple_vel;
			} else {
				return grapple_vel;
			}
		} 

		if (wall_run_status == WallRunEnum.Running) {
			//If running on a wall, do code
			//Design later when run able walls have been made. 
		}
		
		if (grapple_status == GrappleEnum.Falling || wall_run_status == WallRunEnum.Falling) {
			//This also applies to the momentum of coming off a wall run
			//Have gravity affecting the momentum
			//For the X and Z movement, add the key movement to the velocity and reduce it so that its length is less than the highest speed reached. 
			//POTENTIALLY, if the player's X and Z velocities reach a low enough level then they will lose all of the momentum and will be sent to GrapplingEnum.None and WallRunEnum.Falling

			//This part of the code will require a much more thorough look at Ultrakill.
			
			//For the sake of testing the first part of the grappling code, this will end and go to the normal walking and falling code. So delete the following code afterwards
			grapple_vel = Vector3.Zero;
		}
		//May not need an else case because there will be a return statement. [REMEMBER THIS]

		//Normal walking/falling code
		if (IsOnFloor() && Input.IsActionJustPressed("jump")) {
			vel.Y = (float)JUMP_VELOCITY;
		} else { 
			vel.Y -= (float)(GRAVITY * delta);
			vel.Y = Mathf.Clamp(vel.Y, -(float)TERMINAL_VELOCITY, float.MaxValue);
		}

		move_dir = new Vector3(	
			Input.GetAxis("left","right"), 
			0, 
			Input.GetAxis("forward","backward") 
		).Normalized().Rotated(Vector3.Up, Rotation.Y); 

		//The lerp statements are fine for now but may spell trouble later
		vel.X = Mathf.Lerp(vel.X, move_dir.X * (float)SPEED, (float)(ACCEL * delta));
		vel.Z = Mathf.Lerp(vel.Z, move_dir.Z * (float)SPEED, (float)(ACCEL * delta));
		
		return vel;
	}
	
	public override void _PhysicsProcess(double delta)
    {
		HUD HUD = (HUD)GetNode("CanvasLayer/HUD");

		//Pause code. Despite not pausing movement. Pause still pauses all looking and opens the menu. 
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

		//Vector3 vel = velocity_calculation(delta);
		Vector3 vel = Velocity;
			
		Set("velocity", vel);
		Set("up_direction", Vector3.Up);
		MoveAndSlide();
	}
}
