using Godot;
using System;

public partial class BasicMovement : CharacterBody3D
{
	// Physics variables => MOVE TO GLOBAL CONST
	public const float Speed = 10.0f;
	public const float JumpVelocity = 4.5f;
	// End Physics variables
	// Mouse Variables for player 
	public const float lookAngle = 90.0f;
	public const float mouseSensitivity = 0.1f;
	public Vector2 mouseDelta = new Vector2();
	// End Mouse variables for player

	// References
	public Camera3D camera;

    // Loop principale / Ready

    public override void _Ready()
    {
        base._Ready();
		// getCamera
		camera = GetNode("PlayerCamera") as Camera3D;
    }

    // Handling of Camera movement with mouse

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventMouseMotion) 
		{
			// Get mouse mvmt
			mouseDelta = eventMouseMotion.Relative;
		}
    }
    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Rotation set on camera (Y)
		//camera.RotationDegrees -= new Vector3(Mathf.RadToDeg(mouseDelta.Y), 0, 0) * mouseSensitivity;
		camera.RotationDegrees = new Vector3(Mathf.Clamp(camera.RotationDegrees.X, -lookAngle, lookAngle), 
		camera.RotationDegrees.Y, camera.RotationDegrees.Z);
		// Rotation set on player (X)
		RotationDegrees -= new Vector3(0, Mathf.RadToDeg(mouseDelta.X), 0) * mouseSensitivity;
		mouseDelta = new Vector2();

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
