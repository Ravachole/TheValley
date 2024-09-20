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
        Wandering,
        Exploring,
        Smelling
    }
    public abstract partial class Creature : CharacterBody3D
    {
        public CreatureState CurrentState { get; set; } = CreatureState.Wandering;
        // Metabolism stats
        public Thirst Thirst { get; private set; }
        public Hunger Hunger { get; private set; }
        public Stamina Stamina { get; private set; }
        public float EatingAmount { get;set; }
        public List<MemoryEntry> Memory = new List<MemoryEntry>();
        // End Metabolism stats

        // Senses 
        public Smell Smell { get; set; }
        public Vision Vision {get;set;}
        public NavigationAgent3D NavigationAgent {get;set;}
        // End Senses

        // Public custom values
        [Export] public float Speed { get; set; } = 10f;
        [Export] public float DetectionRadius { get; set; } = 15f;
        [Export] public float RotationSpeed { get; set; } = 0.5f;
        // End public custom values

        // Delta needed in behavior context for timed actions
        public float Delta {get; set;}
        // Abstracts mandatory methods
        public abstract override void _PhysicsProcess(double delta);
        public abstract override void _Ready();
        // End Abstracts mandatory methods
        

        // Constructor
        protected Creature() {
            Thirst = new Thirst();
            Hunger = new Hunger();
            Stamina = new Stamina();
            EatingAmount = 5.0f;
        }
        public void SetState(CreatureState state)
        {
            CurrentState = state;
            GD.Print("Herbivore is in state : " + state);
        }
        public void AddToMemory(Vector3 position, string resourceType, float currentTime)
        {
            // Check if this resource was already remembered, and update it
            var existingMemory = Memory.FirstOrDefault(m => m.ResourceType == resourceType && m.Position == position);
            if (existingMemory != null)
            {
                existingMemory.TimeStamp = currentTime; // Update timestamp
            }
            else
            {
                Memory.Add(new MemoryEntry(position, resourceType, currentTime));
            }
        }

        public Vector3? RecallRecentResource(string resourceType, float currentTime, float memoryExpirationTime)
        {
            var rememberedResource = Memory
                .Where(m => m.ResourceType == resourceType && (currentTime - m.TimeStamp) < memoryExpirationTime)
                .OrderBy(m => currentTime - m.TimeStamp)  // Prefer the most recently seen
                .FirstOrDefault();

            if (rememberedResource != null)
            {
                return rememberedResource.Position;
            }

            return null;  // No valid memory found
        }

    }
}