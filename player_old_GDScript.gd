extends CharacterBody3D

const FRICTION = 10.0
const SPEED = 5.0
const ACCEL = 10.0
const JUMP_VELOCITY = 4.0
const ROTATE_SPEED = 12.0
const GRAVITY = 10.0

const mouse_sense = 0.1 #Mouse sensitivity
var paused = false

var move_dir = Vector3.ZERO

func _ready():
	Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED) #Will trap the mouse cursor inside the game window
	$head/Camera3D/pause.visible = false
	var ready_info = $"/root/Global".teleport_info
	if ready_info != [Vector3.ZERO,Vector3.ZERO,Vector3.ZERO]:
		position += ready_info[0]
		velocity = ready_info[1]
		rotation.y = ready_info[2].y
		$head.rotation.x = ready_info[2].x
	print(str(fmod(-114.8,360)))

func _input(event):
	if event is InputEventMouseMotion and !paused:
		rotate_y(deg_to_rad(-event.relative.x * mouse_sense))
		$head.rotate_x(deg_to_rad(-event.relative.y * mouse_sense))
		#rotation.y = clamp(rotation.y, deg2rad(-89), deg2rad(89))
		$head.rotation.x = clamp($head.rotation.x, deg_to_rad(-89), deg_to_rad(89))
		#print("Player: " + str(rotation) + " | Head: " + str($head.rotation))
	
	#look code adapted from YoutTube video called "How To Code An FPS Controller In Godot 3" by Nagi
	#                       https://youtu.be/jf_Hz0diI8Y

func pause():
	if paused: #If unpausing, hide pause_sprite and relock mouse
		paused = false
		Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED) #Will trap the mouse cursor inside the game window
		$head/Camera3D/pause.visible = false
	else:
		paused = true
		Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
		$head/Camera3D/pause.visible = true

func _physics_process(delta):	
	if Input.is_action_just_pressed("pause"):
		pause()
		
	if Input.is_action_just_pressed("toggle_fs"):  #NEEDS REMOVING BEFORE EXPORT
		if get_window().mode != Window.MODE_EXCLUSIVE_FULLSCREEN:
			get_window().mode = Window.MODE_EXCLUSIVE_FULLSCREEN
		else:
			get_window().mode = Window.MODE_WINDOWED
	# Add the gravity and jump
	if !paused:
		if Input.is_action_just_pressed("jump") and is_on_floor():
			velocity.y = JUMP_VELOCITY
		else:
			velocity.y -= GRAVITY * delta #This should be the default, because gravity is a constant
		
		move_dir = Vector3(
			Input.get_axis("a", "d"),
			0,
			Input.get_axis("w", "s")
		).normalized().rotated(Vector3.UP,rotation.y)
		
		var accel_now = ACCEL
		var speed_now = SPEED 
		if Input.is_action_pressed("run") and move_dir != Vector3.ZERO:
			#If shift is pressed and moving, play animation
			if $head/Camera3D.fov == 90:
				$head/Camera3D/FOV_anim.play("FOV")
			accel_now *= 1.7
			speed_now *= 1.7
		elif $head/Camera3D.fov != 90:
			$head/Camera3D/FOV_anim.play_backwards("FOV")
		velocity.x = lerp(velocity.x, move_dir.x * speed_now, accel_now * delta)
		velocity.z = lerp(velocity.z, move_dir.z * speed_now, accel_now * delta)

		set_velocity(velocity)
		set_up_direction(Vector3.UP)
		move_and_slide()
