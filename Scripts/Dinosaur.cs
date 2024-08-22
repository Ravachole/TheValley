using Godot;
using static Status;


abstract class Dinosaur
{
    enum AnimationState {
		IDLE = 0,
		WALK = 1,
		RUN = 2,
		DRINK = 3,
		EAT = 4,
		SLEEP = 5
	}

    private Thirst thirst;
    private Hunger hunger;
    private Energy energy;
    private Stress stress;
	private int currentState = (int)AnimationState.IDLE;

}