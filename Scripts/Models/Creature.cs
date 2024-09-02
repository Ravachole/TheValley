using System;
using Godot;
using System.Linq;

namespace TheValley.Scripts.Models
{
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
        private Thirst _thirst;
        private Hunger _hunger;
        private Stamina _stamina;

        public Thirst Thirst 
        {
            get => _thirst;
            set => _thirst = value;
        }
        public Hunger Hunger 
        {
            get => _hunger;
            set => _hunger = value;
        }
        public Stamina Stamina 
        {
            get => _stamina;
            set => _stamina = value;
        }
        // End Metabolism stats

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
        protected Creature() {
            Thirst = new Thirst();
            Hunger = new Hunger();
            Stamina = new Stamina();
        }

        // Method used to detect water
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
            GD.Print("Herbivore is in state : " + state);
        }
    }
}