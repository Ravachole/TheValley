using Godot;
using System;

public partial class BasicMovement : CharacterBody3D
{
	// Physics variables => MOVE TO GLOBAL CONST
	public const float Speed = 10.0f;
	public const float JumpVelocity = 4.5f;
	// End Physics variables
	// Mouse Variables for player 
	[Export] private float mouseHorizontalSens = 0.3f;
	[Export] private float mouseVerticalSens = 0.2f;
	[Export] private float[] cameraLimits = {-10, 10};
	public Vector2 mouseDelta;
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
			// Rotation player
			RotateY(Mathf.DegToRad(-mouseDelta.X*mouseHorizontalSens));
			
			// Rotation Camera
			camera.RotateX(Mathf.DegToRad(-mouseDelta.Y*mouseVerticalSens));
			ClampCamera();
		}
    }
    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

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
	// Limit the (vertical) rotation of camera
	private void ClampCamera() {
		Vector3 clampedRotation = camera.RotationDegrees;
		clampedRotation.X = Mathf.Clamp(clampedRotation.X, cameraLimits[0], cameraLimits[1]);
		camera.RotationDegrees = clampedRotation;
	}
}
