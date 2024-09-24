using System;
using Godot;
using System.Linq;
using System.Collections.Generic;
using TheValley.Scripts.Models.Metabolism;
using TheValley.Scripts.Models.Senses;
using TheValley.Scripts.Models.Item;

namespace TheValley.Scripts.Models
{
    public enum CreatureState
    {
        Idle,
        Eating,
        Drinking,
        Sleeping,
        Wandering,
        Smelling,
        Running
    }
    public enum CreatureStatus
    {
        Hungry,
        Thirsty,
        Tired
    }
    public abstract partial class Creature : CharacterBody3D
    {
        public CreatureState CurrentState { get; set; } = CreatureState.Wandering;
        public List<CreatureStatus> CreatureStatuses = new List<CreatureStatus>();
        // Metabolism stats
        public Thirst Thirst { get; private set; }
        public Hunger Hunger { get; private set; }
        public Stamina Stamina { get; private set; }
        public float EatingAmount { get;set; }
        public float DringkingAmount  {get; set;}
        public float StaminaRegeneration {get; set;}
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
            DringkingAmount = 5.0f;
            StaminaRegeneration = 10.0f;
        }
        public void SetState(CreatureState state)
        {
            CurrentState = state;
            GD.Print("Herbivore is in state : " + state);
        }
        public void AddToMemory(GeneralItem item, float currentTime)
        {
            // Check if this resource was already remembered, and update it
            var existingMemory = Memory.FirstOrDefault(m => m.item == item);
            if (existingMemory != null)
            {
                existingMemory.TimeStamp = currentTime; // Update timestamp
            }
            else
            {
                Memory.Add(new MemoryEntry(item, currentTime));
            }
        }

        public Vector3? RecallRecentResource(GeneralItem item, float currentTime, float memoryExpirationTime)
        {
            var rememberedResource = Memory
                .Where(m => m.item == item && (currentTime - m.TimeStamp) < memoryExpirationTime)
                .OrderBy(m => currentTime - m.TimeStamp)  // Prefer the most recently seen or smelt
                .FirstOrDefault();

            if (rememberedResource != null)
            {
                Node3D rememberedItem = rememberedResource.item as Node3D;
                return rememberedResource.item.GlobalPosition;
            }

            return null;  // No valid memory found
        }

    }
}