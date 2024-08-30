using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class HerbivoreBehaviorTree : BaseBehaviorTree
{
    public HerbivoreBehaviorTree() : base()
    {
    }

    // Implement the abstract methods from BaseBehaviorTree to create behavior sequences
    protected override IBehaviorNode CreateHungerSequence()
    {
        return new Sequence(new List<IBehaviorNode>
        {
            new ConditionNode(creature => IsHungry(creature)),
            new ConditionNode(creature => IsFoodNearby(creature)),
            new Sequence(new List<IBehaviorNode>
            {
                new ActionNode(creature => MoveToFood(creature)),
                new ConditionNode(creature => IsNearTarget(creature, "FoodDetectionArea")),
                new ActionNode(creature => EatFood(creature))
            })
        });
    }

    protected override IBehaviorNode CreateThirstSequence()
    {
        return new Sequence(new List<IBehaviorNode>
        {
            new ConditionNode(creature => IsThirsty(creature)),
            new ConditionNode(creature => IsWaterNearby(creature)),
            new Sequence(new List<IBehaviorNode>
            {
                new ActionNode(creature => MoveToWater(creature)),
                new ConditionNode(creature => IsNearTarget(creature, "WaterDetectionArea")),
                new ActionNode(creature => DrinkWater(creature))
            })
        });
    }

    protected override IBehaviorNode CreateTiredSequence()
    {
        return new Sequence(new List<IBehaviorNode>
        {
            new ConditionNode(creature => IsTired(creature)),
            new ActionNode(creature => IsSleeping(creature))
        });
    }

    protected override IBehaviorNode CreateIdleSequence()
    {
        return new Sequence(new List<IBehaviorNode>
        {
            new ConditionNode(creature => IsIdle(creature), true),
            new ActionNode(creature => Idle(creature))
        });
    }

    protected override IBehaviorNode CreateWanderSequence()
    {
        return new Sequence(new List<IBehaviorNode>
        {
            new ConditionNode(creature => IsWandering(creature), true),
            new ActionNode(creature => Wander(creature)) // This uses the default Wander method from BaseBehaviorTree
        });
    }

    // Methods defining the specific behaviors
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
        bool isHungry = creature.hunger.IsBelowThreshold();
        GD.Print($"{creature.Name} is Hungry: {isHungry} (Current: {creature.hunger.Current}, Threshold: {creature.hunger.Threshold})");
        return isHungry;
    }

    private bool IsFoodNearby(Creature creature)
    {
        GD.Print($"{creature.Name} Start looking for food");
        return creature.GetNode<Area3D>("FoodDetectionArea").GetOverlappingBodies().Any(body => body.IsInGroup("food"));
    }

    private void MoveToFood(Creature creature)
    {
        var food = creature.GetNode<Area3D>("FoodDetectionArea").GetOverlappingBodies().FirstOrDefault(body => body.IsInGroup("food"));
        if (food != null)
        {
            creature.SetState(CreatureState.Wandering);
            creature.Velocity = (food.GlobalTransform.Origin - creature.GlobalTransform.Origin).Normalized() * creature.Speed;
            GD.Print($"{creature.Name} Start moving to food at {creature.Velocity}");
        }
    }

    private void EatFood(Creature creature)
    {
        GD.Print($"{creature.Name} Start eating");
        creature.SetState(CreatureState.Eating);
        creature.hunger.CurrentDrain = 0.0f;
        while (!HasEatenEnough(creature))
        {
            creature.hunger.Current += 10.0f;
        }
        creature.SetState(CreatureState.Wandering);
        GD.Print($"{creature.Name} has eaten enough and will resume normal behavior.");
        creature.hunger.CurrentDrain = creature.hunger.Drain;
    }

    private bool HasEatenEnough(Creature creature)
    {
        return creature.hunger.IsFull();
    }

    private bool IsThirsty(Creature creature)
    {
        bool isThirsty = creature.thirst.IsBelowThreshold();
        GD.Print($"{creature.Name} is Thirsty: {isThirsty} (Current: {creature.thirst.Current}, Threshold: {creature.thirst.Threshold})");
        return isThirsty;
    }

    private bool IsWaterNearby(Creature creature)
    {
        GD.Print($"{creature.Name} Start looking for water");
        return creature.GetNode<Area3D>("WaterDetectionArea").GetOverlappingBodies().Any(body => body.IsInGroup("water"));
    }

    private void MoveToWater(Creature creature)
    {
        var water = creature.GetNode<Area3D>("WaterDetectionArea").GetOverlappingBodies().FirstOrDefault(body => body.IsInGroup("water"));
        if (water != null)
        {
            creature.SetState(CreatureState.Wandering);
            creature.Velocity = (water.GlobalTransform.Origin - creature.GlobalTransform.Origin).Normalized() * creature.Speed;
            GD.Print($"{creature.Name} Start moving to water at {creature.Velocity}");
        }
    }

    private void DrinkWater(Creature creature)
    {
        GD.Print($"{creature.Name} Start drinking");
        creature.SetState(CreatureState.Drinking);
        creature.thirst.CurrentDrain = 0.0f;
        while (!HasDrunkEnough(creature))
        {
            creature.thirst.Current += 5.0f;
        }
        creature.SetState(CreatureState.Wandering);
        GD.Print($"{creature.Name} has drunk enough and will resume normal behavior.");
        creature.thirst.CurrentDrain = creature.thirst.Drain;
    }

    private bool HasDrunkEnough(Creature creature)
    {
        return creature.thirst.IsFull();
    }

    private bool IsTired(Creature creature)
    {
        bool isTired = creature.stamina.IsBelowThreshold();
        GD.Print($"{creature.Name} is Tired: {isTired} (Current: {creature.stamina.Current}, Threshold: {creature.stamina.Threshold})");
        return isTired;
    }

    private void IsSleeping(Creature creature)
    {
        GD.Print($"{creature.Name} Start sleeping");
        creature.SetState(CreatureState.Sleeping);
        creature.stamina.CurrentDrain = 0.0f;
        while (!HasSleptEnough(creature))
        {
            creature.stamina.Current += 15.0f;
        }
        creature.SetState(CreatureState.Wandering);
        GD.Print($"{creature.Name} has Slept enough and will resume normal behavior.");
        creature.stamina.CurrentDrain = creature.stamina.Drain;
    }

    private bool HasSleptEnough(Creature creature)
    {
        return creature.stamina.IsFull();
    }
}
