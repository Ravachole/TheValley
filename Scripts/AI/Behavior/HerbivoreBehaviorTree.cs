using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
public class HerbivoreBehaviorTree
{
    private BehaviorNode _root;

    private Random _random = new Random();
    private float _wanderTimer = 0.0f;
    // time in seconds before changing direction
    private const float _wanderInterval = 2.0f;

    // public HerbivoreBehaviorTree()
    // {
    //     _root = new Selector(new List<BehaviorNode>
    //     {
    //         // new ConditionNode(IsPredatorNearby),
    //         // new Sequence(new List<BehaviorNode>
    //         // {
    //         //     new ActionNode(FleeFromPredator),
    //         //     // new ActionNode(() => { /* Additional actions */ })
    //         // }),
    //         // new ConditionNode(IsWaterNearby),
    //         // new Sequence(new List<BehaviorNode>
    //         // {
    //         //     new ActionNode(MoveToWater),
    //         //     new ActionNode(DrinkWater)
    //         // }),
    //         new ConditionNode(IsHungry),
    //           new Sequence(new List<BehaviorNode>
    //             {
    //                 new ConditionNode(IsFoodNearby),
    //                 new Sequence(new List<BehaviorNode>
    //                 {
    //                     new ActionNode(MoveToFood),
    //                     new ActionNode(EatFood)
    //                 }),
    //             }       
    //           ),
     
    //         // new Sequence(new List<BehaviorNode>
    //         // {
    //         //     new ConditionNode(IsGroupNearby),
    //         //     new ActionNode(MoveToGroupCenter)
    //         // }),
    //         new ActionNode(Wander)
    //     });
    // }

    public HerbivoreBehaviorTree()
    {
        _root = new Selector(new List<BehaviorNode>
        {
            new Sequence(new List<BehaviorNode>
            {
                new ConditionNode(IsHungry),
                new Sequence(new List<BehaviorNode>
                {
                    new ConditionNode(IsFoodNearby),
                    new Sequence(new List<BehaviorNode>
                    {
                        new ActionNode(MoveToFood),
                        new ActionNode(EatFood)
                    })
                })
            }),
            new ActionNode(Wander)  // Default action if nothing else applies
        });
    }



    public void Update(Herbivore herbivore)
    {
        bool result = _root.Execute(herbivore);
        GD.Print($"Behavior Tree Update Result: {result}");
    }

    private bool IsPredatorNearby(Herbivore herbivore) 
    { 
        // Implementation 
        return false; 
    }

    private void FleeFromPredator(Herbivore herbivore) 
    { 
        // Implementation 
    }

    private bool IsWaterNearby(Herbivore herbivore) 
    { 
        // Implementation 
        return herbivore.GetNode<Area3D>("WaterDetectionArea").GetOverlappingBodies().Any(body => body.IsInGroup("water"));
    }

    private void MoveToWater(Herbivore herbivore) 
    { 
        // Implementation
        if (herbivore.thirst.IsBelowThreshold()) {
            var water = herbivore.GetNode<Area3D>("WaterDetectionArea").GetOverlappingBodies().FirstOrDefault(body => body.IsInGroup("water"));

            if (water != null)
            {
                herbivore.Velocity = (water.GlobalTransform.Origin - herbivore.GlobalTransform.Origin).Normalized() * herbivore.Speed;
            }
        }

    }

    private void DrinkWater(Herbivore herbivore) 
    {
        // TODO : Amount depending of quality of water, so, add another object param here. (find water _> quality)
        herbivore.thirst.Current += 10.0f;
    }

    private bool IsHungry(Herbivore herbivore)
    {
        bool isHungry = herbivore.hunger.IsBelowThreshold();
        GD.Print($"{nameof(herbivore)} is Hungry: {isHungry} (Current: {herbivore.hunger.Current}, Threshold: {herbivore.hunger.Threshold})");
        return isHungry;
    }


    private bool IsFoodNearby(Herbivore herbivore) 
    { 
        // Implementation
        GD.Print(nameof(herbivore)+ " Start looking for food");
        if (herbivore.GetNode<Area3D>("FoodDetectionArea").GetOverlappingBodies().Any(body => body.IsInGroup("food"))) {
            GD.Print(nameof(herbivore) + " Food Found ");
        }
        return herbivore.GetNode<Area3D>("FoodDetectionArea").GetOverlappingBodies().Any(body => body.IsInGroup("food"));
    }

    private void MoveToFood(Herbivore herbivore) 
    { 
        // Implementation
        var food = herbivore.GetNode<Area3D>("FoodDetectionArea").GetOverlappingBodies().FirstOrDefault(body => body.IsInGroup("food"));
        if (food != null)
        {
            herbivore.Velocity = (food.GlobalTransform.Origin - herbivore.GlobalTransform.Origin).Normalized() * herbivore.Speed;
            GD.Print(nameof(herbivore)+ " Start moving to food at " + herbivore.Velocity);
        }
    }

    private void EatFood(Herbivore herbivore) 
    { 
        // Nutrition will depend of type of food later;
        GD.Print(nameof(herbivore)+ " Start eating");
        herbivore.StopMovement();
        herbivore.hunger.Current += 10.0f;
        if (HasEatenEnough(herbivore)) {
            herbivore.ResumeMovement();
        }
    }

    private bool HasEatenEnough(Herbivore herbivore)
    {
        //Check if herbi has eaten enough. Ofc change to dynamic variables later
        return herbivore.hunger.IsFull();
    }

    private bool IsGroupNearby(Herbivore herbivore) 
    { 
        // Implementation 
        return false; 
    }

    private void MoveToGroupCenter(Herbivore herbivore) 
    { 
        // Implementation 
    }

    private void Wander(Herbivore herbivore) 
    { 
        if (herbivore._isStopped)
        {
            // If the herbivore is stopped, do not wander
            return;
        }
        GD.Print(nameof(herbivore)+ " Start wandering");
        GD.Print(nameof(herbivore)+ " DELTA => " + herbivore.Delta);
        GD.Print($"{nameof(herbivore)} Timer Before Update: {_wanderTimer}");
        // Decrease the timer by the time passed since last frame
        _wanderTimer -= herbivore.Delta;
        GD.Print($"{nameof(herbivore)} Timer After Update: {_wanderTimer}");
        // Check if it's time to change direction
        if (_wanderTimer <= 0.0f)
        {
            // Reset the timer
            _wanderTimer = _wanderInterval;

            // Define a new random direction
            Vector3 randomDirection = new Vector3(
                (float)(_random.NextDouble() * 2 - 1),  // X direction between -1 and 1
                0,                                      // Keep Y axis at 0 for flat ground movement
                (float)(_random.NextDouble() * 2 - 1)   // Z direction between -1 and 1
            );

            // Normalize the direction and multiply by speed
            randomDirection = randomDirection.Normalized() * herbivore.Speed;

            // Apply the calculated direction to the herbivore's velocity
            herbivore.Velocity = randomDirection;
            GD.Print($"{nameof(herbivore)} New Velocity: {herbivore.Velocity}");
        }
        else
        {
            // Decrease the timer by the time passed since last frame
            _wanderTimer -= (float)herbivore.Delta;
        }
    }
}
