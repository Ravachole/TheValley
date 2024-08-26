using Godot;
using static Metabolism;


public abstract partial class Creature : CharacterBody3D
{
    enum AnimationState {
		IDLE = 0,
		WALK = 1,
		RUN = 2,
		DRINK = 3,
		EAT = 4,
		SLEEP = 5
	}

	// Metabolism stats
	private Health health;
	private Stamina stamina;
	private Growth growth;
    private Thirst thirst;
    private Hunger hunger;
    private Energy energy;
    private Stress stress;
	// End Metabolism stats
	private Senses senses = new Senses();
	private int currentState = (int)AnimationState.IDLE;
	private NavigationAgent3D navigationAgent3D;
	protected Vector3 moveDirection;
	[Export] float wanderRadius = 5;

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
		// if (moveDirection != new Vector3(0, 0, 0)){
		Vector3 temp = moveDirection + GlobalPosition;
		// LookAt(moveDirection);
		//play move Animation
		Velocity = Velocity.MoveToward(temp * maxSpeed, acceleration);
		// GD.Print("Velo : " + Velocity);

		// }
		// else{
		// 	Idle();
		// }
		MoveAndSlide();
	}
}