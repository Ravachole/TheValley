using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using TheValley.Scripts.Models;

namespace TheValley.Scripts.AI.Behavior
{
    public class HerbivoreBehaviorTree : BaseBehaviorTree
    {
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
                    new ConditionNode(creature => IsNearTarget(creature, "FoodDetectionArea")),
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
                    new ConditionNode(creature => IsNearTarget(creature, "WaterDetectionArea")),
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
            var herbivore = (Herbivore)creature;
            bool isHungry = herbivore.Hunger.IsBelowThreshold();
            // GD.Print($"{creature.Name} is Hungry: {isHungry} (Current: {herbivore.Hunger.Current}, Threshold: {herbivore.Hunger.Threshold})");
            return isHungry;
        }

        private bool IsFoodNearby(Creature creature)
        {
            GD.Print($"{creature.Name} Start looking for food");
            return creature.GetNode<Area3D>("FoodDetectionArea").GetOverlappingBodies().Any(body => body.IsInGroup("food"));
        }

        private void MoveToFood(Creature creature)
        {
            var herbivore = (Herbivore)creature;
            var food = herbivore.GetNode<Area3D>("FoodDetectionArea").GetOverlappingBodies().FirstOrDefault(body => body.IsInGroup("food"));
            if (food != null)
            {
                herbivore.SetState(CreatureState.Wandering);
                herbivore.Velocity = (food.GlobalTransform.Origin - herbivore.GlobalTransform.Origin).Normalized() * herbivore.Speed;
                GD.Print($"{herbivore.Name} Start moving to food at {herbivore.Velocity}");
            }
        }

        private void EatFood(Creature creature)
        {
            var herbivore = (Herbivore)creature;
            GD.Print($"{herbivore.Name} Start eating");
            herbivore.SetState(CreatureState.Eating);
            herbivore.Hunger.CurrentDrain = 0.0f;
            while (!HasEatenEnough(herbivore))
            {
                herbivore.Hunger.Current += 10.0f;
            }
            herbivore.SetState(CreatureState.Wandering);
            GD.Print($"{herbivore.Name} has eaten enough and will resume normal behavior.");
            herbivore.Hunger.CurrentDrain = herbivore.Hunger.Drain;
        }

        private static bool HasEatenEnough(Herbivore herbivore)
        {
            return herbivore.Hunger.IsFull();
        }

        private bool IsThirsty(Creature creature)
        {
            var herbivore = (Herbivore)creature;
            bool isThirsty = herbivore.Thirst.IsBelowThreshold();
            // GD.Print($"{herbivore.Name} is Thirsty: {isThirsty} (Current: {herbivore.Thirst.Current}, Threshold: {herbivore.Thirst.Threshold})");
            return isThirsty;
        }

        private bool IsWaterNearby(Creature creature)
        {
            GD.Print($"{creature.Name} Start looking for water");
            return creature.GetNode<Area3D>("WaterDetectionArea").GetOverlappingBodies().Any(body => body.IsInGroup("water"));
        }

        private void MoveToWater(Creature creature)
        {
            var herbivore = (Herbivore)creature;
            var water = herbivore.GetNode<Area3D>("WaterDetectionArea").GetOverlappingBodies().FirstOrDefault(body => body.IsInGroup("water"));
            if (water != null)
            {
                herbivore.SetState(CreatureState.Wandering);
                herbivore.Velocity = (water.GlobalTransform.Origin - herbivore.GlobalTransform.Origin).Normalized() * herbivore.Speed;
                GD.Print($"{herbivore.Name} Start moving to water at {herbivore.Velocity}");
            }
        }

        private void DrinkWater(Creature creature)
        {
            var herbivore = (Herbivore)creature;
            GD.Print($"{herbivore.Name} Start drinking");
            herbivore.SetState(CreatureState.Drinking);
            herbivore.Thirst.CurrentDrain = 0.0f;
            while (!HasDrunkEnough(herbivore))
            {
                herbivore.Thirst.Current += 5.0f;
            }
            herbivore.SetState(CreatureState.Wandering);
            GD.Print($"{herbivore.Name} has drunk enough and will resume normal behavior.");
            herbivore.Thirst.CurrentDrain = herbivore.Thirst.Drain;
        }

        private static bool HasDrunkEnough(Herbivore herbivore)
        {
            return herbivore.Thirst.IsFull();
        }

        private bool IsTired(Creature creature)
        {
            var herbivore = (Herbivore)creature;
            bool isTired = herbivore.Stamina.IsBelowThreshold();
            // GD.Print($"{herbivore.Name} is Tired: {isTired} (Current: {herbivore.Stamina.Current}, Threshold: {herbivore.Stamina.Threshold})");
            return isTired;
        }

        private void IsSleeping(Creature creature)
        {
            var herbivore = (Herbivore)creature;
            GD.Print($"{creature.Name} is sleeping");
            ((Herbivore)creature).SetState(CreatureState.Sleeping);
            creature.Stamina.CurrentDrain = 0.0f;
            while (!HasSleptEnough(herbivore)) 
            {
                herbivore.Stamina.Current += 10.0f;
            }
            herbivore.SetState(CreatureState.Wandering);
            GD.Print($"{herbivore.Name} has Slept enough and will resume normal behavior.");
            herbivore.Stamina.CurrentDrain = herbivore.Stamina.Drain;
        }

        private static bool HasSleptEnough(Herbivore herbivore)
        {
            return herbivore.Stamina.IsFull();
        }
    }
}
