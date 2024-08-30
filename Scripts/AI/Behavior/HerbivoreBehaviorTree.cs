using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class HerbivoreBehaviorTree : BaseBehaviorTree<Herbivore>
{
    private Random _random = new Random();
    private float _wanderTimer = 0.0f;
    private const float _wanderInterval = 2.0f;

    public HerbivoreBehaviorTree() : base()
    {
    }

    // Implement the abstract methods from BaseBehaviorTree to create behavior sequences
    protected override BehaviorNode<Herbivore> CreateHungerSequence()
    {
        return new Sequence<Herbivore>(new List<BehaviorNode<Herbivore>>
        {
            new ConditionNode<Herbivore>(IsHungry),
            new ConditionNode<Herbivore>(IsFoodNearby),
            new Sequence<Herbivore>(new List<BehaviorNode<Herbivore>>
            {
                new ActionNode<Herbivore>(MoveToFood),
                new ConditionNode<Herbivore>(herbivore => IsNearTarget(herbivore, "FoodDetectionArea")),
                new ActionNode<Herbivore>(EatFood)
            })
        });
    }

    protected override BehaviorNode<Herbivore> CreateThirstSequence()
    {
        return new Sequence<Herbivore>(new List<BehaviorNode<Herbivore>>
        {
            new ConditionNode<Herbivore>(IsThirsty),
            new ConditionNode<Herbivore>(IsWaterNearby),
            new Sequence<Herbivore>(new List<BehaviorNode<Herbivore>>
            {
                new ActionNode<Herbivore>(MoveToWater),
                new ConditionNode<Herbivore>(herbivore => IsNearTarget(herbivore, "WaterDetectionArea")),
                new ActionNode<Herbivore>(DrinkWater)
            })
        });
    }

    protected override BehaviorNode<Herbivore> CreateTiredSequence()
    {
        return new Sequence<Herbivore>(new List<BehaviorNode<Herbivore>>
        {
            new ConditionNode<Herbivore>(IsTired),
            new ActionNode<Herbivore>(IsSleeping)
        });
    }

    protected override BehaviorNode<Herbivore> CreateIdleSequence()
    {
        return new Sequence<Herbivore>(new List<BehaviorNode<Herbivore>>
        {
            new ConditionNode<Herbivore>(IsIdle, true),
            new ActionNode<Herbivore>(Idle)
        });
    }

    protected override BehaviorNode<Herbivore> CreateWanderSequence()
    {
        return new Sequence<Herbivore>(new List<BehaviorNode<Herbivore>>
        {
            new ConditionNode<Herbivore>(IsWandering, true),
            new ActionNode<Herbivore>(Wander) // This can use the default from BaseBehaviorTree
        });
    }

    // Methods defining the specific behaviors
    private bool IsIdle(Herbivore herbivore)
    {
        return herbivore.CurrentState == CreatureState.Idle;
    }

    private bool IsWandering(Herbivore herbivore)
    {
        return herbivore.CurrentState == CreatureState.Wandering;
    }

    private void Idle(Herbivore herbivore)
    {
        GD.Print($"{nameof(herbivore)} is idling");
    }

    private bool IsHungry(Herbivore herbivore)
    {
        bool isHungry = herbivore.hunger.IsBelowThreshold();
        GD.Print($"{nameof(herbivore)} is Hungry: {isHungry} (Current: {herbivore.hunger.Current}, Threshold: {herbivore.hunger.Threshold})");
        return isHungry;
    }

    private bool IsFoodNearby(Herbivore herbivore)
    {
        GD.Print($"{nameof(herbivore)} Start looking for food");
        return herbivore.GetNode<Area3D>("FoodDetectionArea").GetOverlappingBodies().Any(body => body.IsInGroup("food"));
    }

    private void MoveToFood(Herbivore herbivore)
    {
        var food = herbivore.GetNode<Area3D>("FoodDetectionArea").GetOverlappingBodies().FirstOrDefault(body => body.IsInGroup("food"));
        if (food != null)
        {
            herbivore.SetState(CreatureState.Wandering);
            herbivore.Velocity = (food.GlobalTransform.Origin - herbivore.GlobalTransform.Origin).Normalized() * herbivore.Speed;
            GD.Print($"{nameof(herbivore)} Start moving to food at {herbivore.Velocity}");
        }
    }

    private void EatFood(Herbivore herbivore)
    {
        GD.Print($"{nameof(herbivore)} Start eating");
        herbivore.SetState(CreatureState.Eating);
        herbivore.hunger.CurrentDrain = 0.0f;
        while (!HasEatenEnough(herbivore))
        {
            herbivore.hunger.Current += 10.0f;
        }
        herbivore.SetState(CreatureState.Wandering);
        GD.Print($"{nameof(herbivore)} has eaten enough and will resume normal behavior.");
        herbivore.hunger.CurrentDrain = herbivore.hunger.Drain;
    }

    private bool HasEatenEnough(Herbivore herbivore)
    {
        return herbivore.hunger.IsFull();
    }

    private bool IsThirsty(Herbivore herbivore)
    {
        bool isThirsty = herbivore.thirst.IsBelowThreshold();
        GD.Print($"{nameof(herbivore)} is Thirsty: {isThirsty} (Current: {herbivore.thirst.Current}, Threshold: {herbivore.thirst.Threshold})");
        return isThirsty;
    }

    private bool IsWaterNearby(Herbivore herbivore)
    {
        GD.Print($"{nameof(herbivore)} Start looking for water");
        return herbivore.GetNode<Area3D>("WaterDetectionArea").GetOverlappingBodies().Any(body => body.IsInGroup("water"));
    }

    private void MoveToWater(Herbivore herbivore)
    {
        var water = herbivore.GetNode<Area3D>("WaterDetectionArea").GetOverlappingBodies().FirstOrDefault(body => body.IsInGroup("water"));
        if (water != null)
        {
            herbivore.SetState(CreatureState.Wandering);
            herbivore.Velocity = (water.GlobalTransform.Origin - herbivore.GlobalTransform.Origin).Normalized() * herbivore.Speed;
            GD.Print($"{nameof(herbivore)} Start moving to water at {herbivore.Velocity}");
        }
    }

    private void DrinkWater(Herbivore herbivore)
    {
        GD.Print($"{nameof(herbivore)} Start drinking");
        herbivore.SetState(CreatureState.Drinking);
        herbivore.thirst.CurrentDrain = 0.0f;
        while (!HasDrunkEnough(herbivore))
        {
            herbivore.thirst.Current += 5.0f;
        }
        herbivore.SetState(CreatureState.Wandering);
        GD.Print($"{nameof(herbivore)} has drunk enough and will resume normal behavior.");
        herbivore.thirst.CurrentDrain = herbivore.thirst.Drain;
    }

    private bool HasDrunkEnough(Herbivore herbivore)
    {
        return herbivore.thirst.IsFull();
    }

    private bool IsTired(Herbivore herbivore)
    {
        bool isTired = herbivore.stamina.IsBelowThreshold();
        GD.Print($"{nameof(herbivore)} is Tired: {isTired} (Current: {herbivore.stamina.Current}, Threshold: {herbivore.stamina.Threshold})");
        return isTired;
    }

    private void IsSleeping(Herbivore herbivore)
    {
        GD.Print($"{nameof(herbivore)} Start sleeping");
        herbivore.SetState(CreatureState.Sleeping);
        herbivore.stamina.CurrentDrain = 0.0f;
        while (!HasSleptEnough(herbivore))
        {
            herbivore.stamina.Current += 15.0f;
        }
        herbivore.SetState(CreatureState.Wandering);
        GD.Print($"{nameof(herbivore)} has Slept enough and will resume normal behavior.");
        herbivore.stamina.CurrentDrain = herbivore.stamina.Drain;
    }

    private bool HasSleptEnough(Herbivore herbivore)
    {
        return herbivore.stamina.IsFull();
    }
}
