using Godot;
using System;

public partial class Herbivore : CharacterBody3D
{
	// Enum for animations & states
	enum AnimationState {
		IDLE = 0,
		WALK = 1,
		RUN = 2,
		DRINK = 3,
		EAT = 4,
		SLEEP = 5
	}

	[Export] private float thirst = 100.0f;
	[Export] private float hunger = 100.0f;
	[Export] private float energy = 100.0f;
	[Export] private float stress = 0.0f;

	private NavigationAgent3D navigationAgent3D;
	private int currentState = (int)AnimationState.IDLE;
	private Vector3 moveDirection;

// Example method of state handling
	private void Idle() {
		currentState = (int)AnimationState.IDLE;
		//Reset speed when idle
		Velocity = new Vector3(0, 0, 0);
		//Handle Sound
	}
	private void Drink() {
		currentState = (int)AnimationState.DRINK;
	}

	private void Move(Vector3 direction){
		moveDirection = direction;
	}
	// Add animation in params
	private void MoveState(float maxSpeed, float acceleration){
		if (moveDirection != new Vector3(0, 0, 0)){
			LookAt(moveDirection);
			//play move Animation
			Velocity = Velocity.MoveToward(moveDirection * maxSpeed, acceleration);
		else 
			Idle();
		}
		MoveAndSlide();
	}

    public override void _Ready(){
        navigationAgent3D = GetNode("NavigationAgent3D") as NavigationAgent3D;
    }
    // public override void _PhysicsProcess(double delta){
	// }




}
