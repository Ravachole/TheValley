using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using TheValley.Scripts.Models;
using TheValley.Scripts.Models.Item;
using TheValley.Scripts.Models.Item.Consumable;

namespace TheValley.Scripts.AI.Behavior
{
    public class HerbivoreBehaviorTree : BaseBehaviorTree
    {
        private Food CurrentFood;
        private Water CurrentWater;
        public HerbivoreBehaviorTree() : base() { 
            Initialize();
        }

        protected override IBehaviorNode CreateHungerSequence()
        {
            return new Sequence(new List<IBehaviorNode>
            {
                new ConditionNode(IsHungry),
                new ConditionNode(IsFoodNearby),
                new Sequence(new List<IBehaviorNode>
                {
                    new ActionNode(MoveToFood),
                    // new ConditionNode(creature => IsNearTarget(creature, "FoodDetectionArea")),
                    new ActionNode(EatFood)
                })
            });
        }

        protected override IBehaviorNode CreateThirstSequence()
        {
            return new Sequence(new List<IBehaviorNode>
            {
                new ConditionNode(IsThirsty),
                new ConditionNode(IsWaterNearby),
                new Sequence(new List<IBehaviorNode>
                {
                    new ActionNode(MoveToWater),
                    // new ConditionNode(creature => IsNearTarget(creature, "WaterDetectionArea")),
                    new ActionNode(DrinkWater)
                })
            });
        }

        protected override IBehaviorNode CreateTiredSequence()
        {
            return new Sequence(new List<IBehaviorNode>
            {
                new ConditionNode(IsTired),
                new ActionNode(IsSleeping)
            });
        }

        protected override IBehaviorNode CreateIdleSequence()
        {
            return new Sequence(new List<IBehaviorNode>
            {
                new ConditionNode(IsIdle, true),
                new ActionNode(Idle)
            });
        }

        protected override IBehaviorNode CreateWanderSequence()
        {
            return new Sequence(new List<IBehaviorNode>
            {
                new ConditionNode(IsWandering, true),
                new ActionNode(Wander)
            });
        }

        private bool IsIdle(Creature creature)
        {
            return creature.CurrentState == CreatureState.Idle;
        }

        private bool IsWandering(Creature creature)
        {
            return creature.CurrentState == CreatureState.Wandering;
        }

        private void Idle(Creature creature)
        {
            GD.Print($"{creature.Name} is idling");
        }

        private bool IsHungry(Creature creature)
        {
            return CheckAndSetStatus(creature, c => ((Herbivore)c).Hunger.IsBelowThreshold(), CreatureStatus.Hungry);
        }

        private bool IsFoodNearby(Creature creature)
        {
            CurrentFood = FindClosestResource<Food>(creature, "food");
            return CurrentFood != null;
        }

        private void MoveToFood(Creature creature)
        {
            MoveToTarget((Herbivore)creature, CurrentFood);
        }

        private void EatFood(Creature creature)
        {
            var herbivore = (Herbivore)creature;
    
            // Call the ConsumeResource method with food-specific logic
            ConsumeResource(herbivore, CurrentFood, 
            herbivore => herbivore.Hunger.IsFull(),  // Condition for hunger satisfaction
            herbivore => herbivore.Hunger.CurrentDrain = 0.0f,
            herbivore => herbivore.Hunger.CurrentDrain = herbivore.Hunger.Drain,
            herbivore => herbivore.Hunger.Current += herbivore.EatingAmount,  // Increment hunger
            herbivore => CurrentFood.Value -= herbivore.EatingAmount,
            CreatureStatus.Hungry);
        }

        private static bool HasEatenEnough(Herbivore herbivore)
        {
            return herbivore.Hunger.IsFull();
        }

        private bool IsThirsty(Creature creature)
        {
           return CheckAndSetStatus(creature, c => ((Herbivore)c).Thirst.IsBelowThreshold(), CreatureStatus.Thirsty);
        }

        private bool IsWaterNearby(Creature creature)
        {
            CurrentWater = FindClosestResource<Water>(creature, "water");
            return CurrentWater != null;
        }

        private void MoveToWater(Creature creature)
        {
            MoveToTarget((Herbivore)creature, CurrentWater);
        }

        private void DrinkWater(Creature creature)
        {
            var herbivore = (Herbivore)creature;
            
            ConsumeResource(herbivore, CurrentWater, 
            herbivore => herbivore.Thirst.IsFull(),
            herbivore => herbivore.Thirst.CurrentDrain = 0.0f,
            herbivore => herbivore.Thirst.CurrentDrain = herbivore.Thirst.Drain,
            herbivore => herbivore.Thirst.Current += herbivore.DringkingAmount,
            herbivore => CurrentWater.Value -= herbivore.DringkingAmount,
            CreatureStatus.Thirsty);
        }

        private static bool HasDrunkEnough(Herbivore herbivore)
        {
            return herbivore.Thirst.IsFull();
        }

        private bool IsTired(Creature creature)
        {
            return CheckAndSetStatus(creature, c => ((Herbivore)c).Stamina.IsBelowThreshold(), CreatureStatus.Tired);
        }

        private void IsSleeping(Creature creature)
        {
            var herbivore = (Herbivore)creature;

            herbivore.SetState(CreatureState.Sleeping);
            herbivore.Stamina.CurrentDrain = 0.0f;
            while (!HasSleptEnough(herbivore)) 
            {
                herbivore.Stamina.Current += herbivore.StaminaRegeneration;
            }
            herbivore.CreatureStatuses.Remove(CreatureStatus.Tired);
            herbivore.SetState(CreatureState.Wandering);
            herbivore.Stamina.CurrentDrain = herbivore.Stamina.Drain;
        }

        private static bool HasSleptEnough(Herbivore herbivore)
        {
            return herbivore.Stamina.IsFull();
        }
    }
}
