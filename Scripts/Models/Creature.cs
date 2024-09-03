using System;
using Godot;
using System.Linq;
using System.Collections.Generic;
using TheValley.Scripts.Models.Metabolism;
using TheValley.Scripts.Models.Senses;

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
        // Metabolism stats
        public Thirst Thirst { get; private set; }
        public Hunger Hunger { get; private set; }
        public Stamina Stamina { get; private set; }
        public List<Need> Needs { get; set; }
        public SensesHandler Senses { get; set; }

        // End Metabolism stats

        // Public custom values
        [Export] public float Speed = 10f;
        [Export] public float DetectionRadius = 15f;
        // End public custom values
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
        public void SetState(CreatureState state)
        {
            CurrentState = state;
            GD.Print("Herbivore is in state : " + state);
        }
    }
}