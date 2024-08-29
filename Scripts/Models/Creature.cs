using System;
using Godot;
using static Metabolism;
using System.Linq;

public enum CreatureState
{
    Idle,
    Eating,
    Drinking,
    Sleeping,
    Wandering
}
public abstract partial class Creature : CharacterBody3D
{
    public CreatureState CurrentState { get; set; } = CreatureState.Wandering;
	// Metabolism stats (update everything to protected => private make little sense in an abstract class)
    // TODO : Syntax, majuscules pour public
    public Thirst thirst;
    public Hunger hunger;
    public Stamina stamina;
	// End Metabolism stats

	// TODO : REFACTOR AREA DETECTION IN SENSES
	// private Senses senses = new Senses();

	// Public custom values
	[Export] public float Speed = 10f;
	[Export] public float DetectionRadius = 15f;
	// End public custom values
    protected Area3D _waterDetectionArea;
    protected Area3D _foodDetectionArea;
	// Abstracts mandatory methods
	public abstract override void _PhysicsProcess(double delta);
	public abstract override void _Ready();
	// End Abstracts mandatory methods

    // Delta needed in behavior context for timed actions
    public float Delta {get; set;}
    

    // Constructor
    public Creature() {
        thirst = new Thirst();
        hunger = new Hunger();
        stamina = new Stamina();
    }
    // TODO : CHECK IF THESE METHODS ARE STILL NEEDED. I dont think so because herbehavtree check from godot, this is never used.
	protected void OnWaterEntered(Node3D body)
    {
        if (body.IsInGroup("water"))
        {
            // Moving to water logic
            Velocity = (body.GlobalTransform.Origin - GlobalTransform.Origin).Normalized() * Speed;
        }
    }

    protected void OnFoodEntered(Node3D body)
    {
        if (body.IsInGroup("food"))
        {
            // Moving to food logic
            Velocity = (body.GlobalTransform.Origin - GlobalTransform.Origin).Normalized() * Speed;
        }
    }
    public void SetState(CreatureState state)
    {
        CurrentState = state;
    }
}