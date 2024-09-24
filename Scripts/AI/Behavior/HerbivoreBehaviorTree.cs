using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using TheValley.Scripts.Models;
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
            var herbivore = (Herbivore)creature;
            bool isHungry = herbivore.Hunger.IsBelowThreshold();

            if (isHungry)
            {
                if (!creature.CreatureStatuses.Contains(CreatureStatus.Hungry))
                {
                    creature.CreatureStatuses.Add(CreatureStatus.Hungry);
                }
            }
            return isHungry;
        }

        private bool IsFoodNearby(Creature creature)
        {
            GD.Print($"{creature.Name} Start looking for food");
            CurrentFood = FindClosestNodeInGroup(creature.Vision.GetVisibleObjects(), "food", creature) as Food;
            if (CurrentFood != null)
            {
                return true;
            }
            CurrentFood = FindClosestNodeInGroup(creature.Smell.GetSmeltItems(), "food", creature) as Food;
            if (CurrentFood != null)
            {
                return true;
            }
            CurrentFood = RememberClosestElementInGroup(creature.Memory, "food", creature) as Food;;
            if (CurrentFood == null) 
            {
                return true;
            }

            return false;
        }

        private void MoveToFood(Creature creature)
        {
            var herbivore = (Herbivore)creature;
            if (CurrentFood != null)
            {
                herbivore.NavigationAgent.TargetPosition = CurrentFood.GlobalTransform.Origin;
                Vector3 nextPosition = herbivore.NavigationAgent.GetNextPathPosition();
                Vector3 direction = (nextPosition - herbivore.GlobalPosition).Normalized();
                herbivore.Velocity = direction * herbivore.Speed;
                GD.Print($"{herbivore.Name} Start moving to Food at {herbivore.Velocity}");
            }
        }

        private void EatFood(Creature creature)
        {
            var herbivore = (Herbivore)creature;
            if (herbivore.NavigationAgent.IsTargetReached())
            {
                GD.Print($"{herbivore.Name} Start eating");
                herbivore.SetState(CreatureState.Eating);
                herbivore.Hunger.CurrentDrain = 0.0f;
                while (!HasEatenEnough(herbivore))
                {
                    herbivore.Hunger.Current += herbivore.EatingAmount;
                    CurrentFood.Value -= herbivore.EatingAmount;
                    CurrentFood.Update();
                }
                creature.CreatureStatuses.Remove(CreatureStatus.Hungry);
                herbivore.SetState(CreatureState.Wandering);
                GD.Print($"{herbivore.Name} has eaten enough and will resume normal behavior.");
                herbivore.Hunger.CurrentDrain = herbivore.Hunger.Drain;
            }
        }

        private static bool HasEatenEnough(Herbivore herbivore)
        {
            return herbivore.Hunger.IsFull();
        }

        private bool IsThirsty(Creature creature)
        {
            var herbivore = (Herbivore)creature;
            bool isThirsty = herbivore.Thirst.IsBelowThreshold();
            if (isThirsty)
            {
                  if (!creature.CreatureStatuses.Contains(CreatureStatus.Thirsty))
                {
                    creature.CreatureStatuses.Add(CreatureStatus.Thirsty);
                }
            }
            return isThirsty;
        }

        private bool IsWaterNearby(Creature creature)
        {
            GD.Print($"{creature.Name} Start looking for water");
            CurrentWater = FindClosestNodeInGroup(creature.Vision.GetVisibleObjects(), "water", creature) as Water;
            if (CurrentFood != null)
            {
                return true;
            }
            CurrentWater = FindClosestNodeInGroup(creature.Smell.GetSmeltItems(), "water", creature) as Water;
            if (CurrentFood != null)
            {
                return true;
            }
            CurrentWater = RememberClosestElementInGroup(creature.Memory, "water", creature) as Water;;
            if (CurrentFood == null) 
            {
                return true;
            }

            return false;
        }

        private void MoveToWater(Creature creature)
        {
            var herbivore = (Herbivore)creature;
            if (CurrentWater != null)
            {
                herbivore.NavigationAgent.TargetPosition = CurrentWater.GlobalTransform.Origin;
                Vector3 nextPosition = herbivore.NavigationAgent.GetNextPathPosition();
                Vector3 direction = (nextPosition - herbivore.GlobalPosition).Normalized();
                herbivore.Velocity = direction * herbivore.Speed;
                GD.Print($"{herbivore.Name} Start moving to water at {herbivore.Velocity}");
            }
        }

        private void DrinkWater(Creature creature)
        {
            var herbivore = (Herbivore)creature;
            if (herbivore.NavigationAgent.IsTargetReached())
            {
                GD.Print($"{herbivore.Name} Start drinking");
                herbivore.SetState(CreatureState.Drinking);
                herbivore.Thirst.CurrentDrain = 0.0f;
                while (!HasDrunkEnough(herbivore))
                {
                    herbivore.Thirst.Current += herbivore.EatingAmount;
                    CurrentWater.Value -= herbivore.EatingAmount;
                    CurrentWater.Update();
                }
                creature.CreatureStatuses.Remove(CreatureStatus.Thirsty);
                herbivore.SetState(CreatureState.Wandering);
                GD.Print($"{herbivore.Name} has drunk enough and will resume normal behavior.");
                herbivore.Thirst.CurrentDrain = herbivore.Thirst.Drain;
            }
        }

        private static bool HasDrunkEnough(Herbivore herbivore)
        {
            return herbivore.Thirst.IsFull();
        }

        private bool IsTired(Creature creature)
        {
            var herbivore = (Herbivore)creature;
            bool isTired = herbivore.Stamina.IsBelowThreshold();
            if (isTired)
            {
                if (!creature.CreatureStatuses.Contains(CreatureStatus.Tired))
                {
                    creature.CreatureStatuses.Add(CreatureStatus.Tired);
                }
            }
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
            creature.CreatureStatuses.Remove(CreatureStatus.Tired);
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
