using System;
using Godot;
using static Metabolism;
using System.Linq;


public abstract partial class Creature : CharacterBody3D
{
	// Metabolism stats (update everything to protected => private make little sense in an abstract class)
	private Health health;
	private Stamina stamina;
	private Growth growth;
    private Thirst thirst;
    private Hunger hunger;
    private Energy energy;
    private Stress stress;
	// End Metabolism stats

	// TODO : REFACTOR AREA DETECTION IN SENSES
	// private Senses senses = new Senses();

	// Public custom values
	[Export] public float Speed = 10f;
	[Export] public float DetectionRadius = 15f;
	// End public custom values
   	protected Vector3 _velocity = Vector3.Zero;
    protected Area3D _waterDetectionArea;
    protected Area3D _foodDetectionArea;
	// Abstracts mandatory methods
	public abstract override void _PhysicsProcess(double delta);
	public abstract override void _Ready();
	// End Abstracts mandatory methods
	protected void OnWaterEntered(Node3D body)
    {
        if (body.IsInGroup("water"))
        {
            // Moving to water logic
            _velocity = (body.GlobalTransform.Origin - GlobalTransform.Origin).Normalized() * Speed;
        }
    }

    protected void OnFoodEntered(Node3D body)
    {
        if (body.IsInGroup("food"))
        {
            // Moving to food logic
            _velocity = (body.GlobalTransform.Origin - GlobalTransform.Origin).Normalized() * Speed;
        }
    }
}