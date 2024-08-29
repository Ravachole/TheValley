using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class HerbivoreBehaviorTree
{
    private BehaviorNode _root;

    private Random _random = new Random();
    private float _wanderTimer = 0.0f;
    private const float _wanderInterval = 2.0f;

    public HerbivoreBehaviorTree()
    {
        _root = new Selector(new List<BehaviorNode>
        {
            new Sequence(new List<BehaviorNode>
            {
                new ConditionNode(IsHungry),
                new ConditionNode(IsFoodNearby), // Check if food is nearby
                new Sequence(new List<BehaviorNode>
                {
                    new ActionNode(MoveToFood),
                    new ConditionNode(IsNearFood), // Check if we are near the food
                    new ActionNode(EatFood)
                })
            }),
            new Sequence(new List<BehaviorNode>
            {
                new ConditionNode(IsThirsty),
                new ConditionNode(IsWaterNearby), // Check if water is nearby
                new Sequence(new List<BehaviorNode>
                {
                    new ActionNode(MoveToWater),
                    new ConditionNode(IsNearWater), // Check if we are near the water
                    new ActionNode(DrinkWater)
                })
            }),
            new Sequence(new List<BehaviorNode>
            {
                new ConditionNode(IsTired),
                new ActionNode(IsSleeping)
            }),
            new Sequence(new List<BehaviorNode>
            {
                new ConditionNode(IsIdle, true),
                new ActionNode(Idle) // Actions for idle state
            }),
            new Sequence(new List<BehaviorNode>
            {
                new ConditionNode(IsWandering, true),
                new ActionNode(Wander) // Actions for wandering state
            })
        });
    }

    public void Update(Herbivore herbivore)
    {
        bool result = _root.Execute(herbivore);
        GD.Print($"Behavior Tree Update Result: {result}");
    }

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
        // Optionally, trigger idle animations or actions
    }

    private bool IsHungry(Herbivore herbivore)
    {
        bool isHungry = herbivore.hunger.IsBelowThreshold();
        GD.Print($"{nameof(herbivore)} is Hungry: {isHungry} (Current: {herbivore.hunger.Current}, Threshold: {herbivore.hunger.Threshold})");
        return isHungry;
    }

    private bool IsFoodNearby(Herbivore herbivore) 
    { 
        GD.Print(nameof(herbivore) + " Start looking for food");
        return herbivore.GetNode<Area3D>("FoodDetectionArea").GetOverlappingBodies().Any(body => body.IsInGroup("food"));
    }

    private bool IsNearFood(Herbivore herbivore)
    {
        // Check if the herbivore is close enough to the food
        var food = herbivore.GetNode<Area3D>("FoodDetectionArea").GetOverlappingBodies().FirstOrDefault(body => body.IsInGroup("food"));
        if (food != null)
        {
            float distance = herbivore.GlobalTransform.Origin.DistanceTo(food.GlobalTransform.Origin);
            return distance < 2.0f; // Example distance threshold; adjust as needed
        }
        return false;
    }

    private void MoveToFood(Herbivore herbivore)
    { 
        var food = herbivore.GetNode<Area3D>("FoodDetectionArea").GetOverlappingBodies().FirstOrDefault(body => body.IsInGroup("food"));
        if (food != null)
        {
            herbivore.SetState(CreatureState.Wandering);
            herbivore.Velocity = (food.GlobalTransform.Origin - herbivore.GlobalTransform.Origin).Normalized() * herbivore.Speed;
            GD.Print(nameof(herbivore) + " Start moving to food at " + herbivore.Velocity);
        }
    }

    private void EatFood(Herbivore herbivore)
    { 
        GD.Print(nameof(herbivore) + " Start eating");
        herbivore.SetState(CreatureState.Eating);
        herbivore.hunger.CurrentDrain = 0.0f;
        while (!HasEatenEnough(herbivore)) {
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
        GD.Print(nameof(herbivore) + " Start looking for water");
        return herbivore.GetNode<Area3D>("WaterDetectionArea").GetOverlappingBodies().Any(body => body.IsInGroup("water"));
    }

    private bool IsNearWater(Herbivore herbivore)
    {
        // Check if the herbivore is close enough to the water
        var water = herbivore.GetNode<Area3D>("WaterDetectionArea").GetOverlappingBodies().FirstOrDefault(body => body.IsInGroup("water"));
        if (water != null)
        {
            float distance = herbivore.GlobalTransform.Origin.DistanceTo(water.GlobalTransform.Origin);
            return distance < 2.0f; // Example distance threshold; adjust as needed
        }
        return false;
    }

    private void MoveToWater(Herbivore herbivore)
    { 
        var water = herbivore.GetNode<Area3D>("WaterDetectionArea").GetOverlappingBodies().FirstOrDefault(body => body.IsInGroup("water"));
        if (water != null)
        {
            herbivore.SetState(CreatureState.Wandering);
            herbivore.Velocity = (water.GlobalTransform.Origin - herbivore.GlobalTransform.Origin).Normalized() * herbivore.Speed;
            GD.Print(nameof(herbivore) + " Start moving to water at " + herbivore.Velocity);
        }
    }

    private void DrinkWater(Herbivore herbivore) 
    {
        GD.Print(nameof(herbivore) + " Start drinking");
        herbivore.SetState(CreatureState.Drinking);
        herbivore.thirst.CurrentDrain = 0.0f;
        while (!HasDrunkEnough(herbivore)) {
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
        GD.Print(nameof(herbivore) + " Start sleeping");
        herbivore.SetState(CreatureState.Sleeping);
        herbivore.stamina.CurrentDrain = 0.0f;
        while (!HasSleptEnough(herbivore)) {
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

    private bool IsInAnyOtherState(Herbivore herbivore)
    {
        return herbivore.CurrentState != CreatureState.Idle && herbivore.CurrentState != CreatureState.Wandering;
    }

    private void Wander(Herbivore herbivore)
    {
        if (herbivore.CurrentState != CreatureState.Wandering)
        {
            return;
        }
        GD.Print(nameof(herbivore) + " Start wandering");
        GD.Print(nameof(herbivore) + " DELTA => " + herbivore.Delta);
        GD.Print($"{nameof(herbivore)} Timer Before Update: {_wanderTimer}");
        _wanderTimer -= herbivore.Delta;
        GD.Print($"{nameof(herbivore)} Timer After Update: {_wanderTimer}");
        if (_wanderTimer <= 0.0f)
        {
            _wanderTimer = _wanderInterval;

            Vector3 randomDirection = new Vector3(
                (float)(_random.NextDouble() * 2 - 1),
                0,
                (float)(_random.NextDouble() * 2 - 1)
            );

            randomDirection = randomDirection.Normalized() * herbivore.Speed;
            herbivore.Velocity = randomDirection;
            GD.Print($"{nameof(herbivore)} New Velocity: {herbivore.Velocity}");
        }
        else
        {
            _wanderTimer -= (float)herbivore.Delta;
        }
    }
}
